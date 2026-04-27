using global::TSMA_Meal.Database.Entities;
using global::TSMA_Meal.Features.Account;
using global::TSMA_Meal.Shared;
using global::TSMA_Meal.Tools;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace TSMA_Meal.Features.Meals.CancelReserve
{




    public class CancelReserveHandlerTests
    {
        private readonly Mock<ICancelReserveRepository> _mockRepository;
        private readonly Mock<IPersonnelRepository> _mockPersonnelRepository;
        private readonly Mock<ILogger<CancelReserveHandler>> _mockLogger;
        private readonly CancelReserveHandler _handler;

        public CancelReserveHandlerTests()
        {
            _mockRepository = new Mock<ICancelReserveRepository>();
            _mockPersonnelRepository = new Mock<IPersonnelRepository>();
            _mockLogger = new Mock<ILogger<CancelReserveHandler>>();

            _handler = new CancelReserveHandler(
                _mockRepository.Object,
                _mockPersonnelRepository.Object,
                _mockLogger.Object);
        }

        #region Validation Tests

        [Fact]
        public async Task Handle_ValidRequest_CancelsReserveSuccessfully()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 12345);
            var personnel = new Personnel { PersonnelCode = 12345, Discount = 10 };
            var reserve = new Reserve
            {
                Id = 100,
                PersonnelCode = 12345,
                CanceledReserve = false,
                ReserveDate = GetFutureDate()  // تاریخ آینده
            };

            SetupSuccessfulMocks(request, personnel, reserve);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(reserve.CanceledReserve);
            _mockRepository.Verify(x => x.UpdateReserveAsync(reserve, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenReserveIdIsZero_ReturnsWithoutAnyAction()
        {
            // Arrange
            var request = new CancelReserveRequest(0, 12345);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mockPersonnelRepository.Verify(x => x.GetPersonnelAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockRepository.Verify(x => x.GetReserveAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockRepository.Verify(x => x.UpdateReserveAsync(It.IsAny<Reserve>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region Personnel Validation Tests

        [Fact]
        public async Task Handle_PersonnelNotFound_ThrowsPersonnelNotExistException()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 99999);

            _mockPersonnelRepository.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Personnel?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<PersonnelNotExistException>(
                () => _handler.Handle(request, CancellationToken.None));

            // Verify logger was called
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(request.PersonnelCode.ToString())),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_PersonnelNoAccess_ThrowsNotPermittedException()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 12345);
            var personnel = new Personnel { PersonnelCode = 12345 };

            _mockPersonnelRepository.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(personnel);

            _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<NotPermittedException>(
                () => _handler.Handle(request, CancellationToken.None));
        }

        #endregion

        #region Reserve Validation Tests

        [Fact]
        public async Task Handle_ReserveNotFound_ThrowsUnknownException()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 12345);
            var personnel = new Personnel { PersonnelCode = 12345 };

            _mockPersonnelRepository.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(personnel);

            _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
                .ReturnsAsync(true);

            _mockRepository.Setup(x => x.GetReserveAsync(request.ReserveId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Reserve?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnknownException>(
                () => _handler.Handle(request, CancellationToken.None));

            // Verify logger was called
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(request.PersonnelCode.ToString())),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ReserveBelongsToAnotherPerson_ThrowsUnknownException()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 12345);
            var personnel = new Personnel { PersonnelCode = 12345 };
            var reserve = new Reserve
            {
                Id = 100,
                PersonnelCode = 99999,  // Different personnel
                CanceledReserve = false
            };

            _mockPersonnelRepository.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(personnel);

            _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
                .ReturnsAsync(true);

            _mockRepository.Setup(x => x.GetReserveAsync(request.ReserveId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reserve);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnknownException>(
                () => _handler.Handle(request, CancellationToken.None));

            // Verify logger was called with both personnel codes
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(request.PersonnelCode.ToString()) &&
                                                  o.ToString()!.Contains(reserve.PersonnelCode.ToString())),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region Expiration Tests

        [Fact]
        public async Task Handle_WhenReserveDateIsExpired_ThrowsCancelReserveOutOfDateException()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 12345);
            var personnel = new Personnel { PersonnelCode = 12345 };
            var reserve = new Reserve
            {
                Id = 100,
                PersonnelCode = 12345,
                CanceledReserve = false,
                ReserveDate = GetPastDate()  // تاریخ گذشته
            };

            _mockPersonnelRepository.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(personnel);

            _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
                .ReturnsAsync(true);

            _mockRepository.Setup(x => x.GetReserveAsync(request.ReserveId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reserve);

            // Act & Assert
            await Assert.ThrowsAsync<CancelReserveOutOfDateException>(
                () => _handler.Handle(request, CancellationToken.None));

            // Verify reserve was not updated
            _mockRepository.Verify(x => x.UpdateReserveAsync(It.IsAny<Reserve>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenReserveDateIsToday_ThrowsCancelReserveOutOfDateException()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 12345);
            var personnel = new Personnel { PersonnelCode = 12345 };
            var today = PersianDateTime.GetPersianDateByWeekDay((int)DateTime.Now.DayOfWeek);
            var reserve = new Reserve
            {
                Id = 100,
                PersonnelCode = 12345,
                CanceledReserve = false,
                ReserveDate = today  // تاریخ امروز
            };

            _mockPersonnelRepository.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(personnel);

            _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
                .ReturnsAsync(true);

            _mockRepository.Setup(x => x.GetReserveAsync(request.ReserveId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reserve);

            // Act & Assert
            await Assert.ThrowsAsync<CancelReserveOutOfDateException>(
                () => _handler.Handle(request, CancellationToken.None));
        }

        #endregion

        #region Already Canceled Tests

        [Fact]
        public async Task Handle_WhenReserveAlreadyCanceled_ReturnsWithoutUpdate()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 12345);
            var personnel = new Personnel { PersonnelCode = 12345 };
            var reserve = new Reserve
            {
                Id = 100,
                PersonnelCode = 12345,
                CanceledReserve = true,  // Already canceled
                ReserveDate = GetFutureDate()
            };

            _mockPersonnelRepository.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(personnel);

            _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
                .ReturnsAsync(true);

            _mockRepository.Setup(x => x.GetReserveAsync(request.ReserveId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reserve);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            // Reserve should still be canceled (no change)
            Assert.True(reserve.CanceledReserve);
            // Update should not be called because it's already canceled
            _mockRepository.Verify(x => x.UpdateReserveAsync(It.IsAny<Reserve>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public async Task Handle_MultipleCancelsOnSameReserve_OnlyFirstOneUpdates()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 12345);
            var personnel = new Personnel { PersonnelCode = 12345 };
            var reserve = new Reserve
            {
                Id = 100,
                PersonnelCode = 12345,
                CanceledReserve = false,
                ReserveDate = GetFutureDate()
            };

            SetupSuccessfulMocks(request, personnel, reserve);

            // Act - First cancel
            await _handler.Handle(request, CancellationToken.None);

            // Reset mocks but keep reserve state
            _mockRepository.Invocations.Clear();

            // Try to cancel again
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            // Update should be called only once (first time)
            // Second time should detect AlreadyCanceled and return without update
            _mockRepository.Verify(x => x.UpdateReserveAsync(It.IsAny<Reserve>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenReserveDateIsExactlyTomorrow_AllowsCancellation()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 12345);
            var personnel = new Personnel { PersonnelCode = 12345 };
            var tomorrow = GetFutureDate();  // فردا یا روزهای آینده
            var reserve = new Reserve
            {
                Id = 100,
                PersonnelCode = 12345,
                CanceledReserve = false,
                ReserveDate = tomorrow
            };

            _mockPersonnelRepository.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(personnel);

            _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
                .ReturnsAsync(true);

            _mockRepository.Setup(x => x.GetReserveAsync(request.ReserveId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reserve);

            _mockRepository.Setup(x => x.UpdateReserveAsync(It.IsAny<Reserve>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(reserve.CanceledReserve);
            _mockRepository.Verify(x => x.UpdateReserveAsync(reserve, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenPersonnelCodeIsValidButEdgeCase_LargeNumber_Succeeds()
        {
            // Arrange
            var request = new CancelReserveRequest(100, int.MaxValue);
            var personnel = new Personnel { PersonnelCode = int.MaxValue, Discount = 10 };
            var reserve = new Reserve
            {
                Id = 100,
                PersonnelCode = int.MaxValue,
                CanceledReserve = false,
                ReserveDate = GetFutureDate()
            };

            SetupSuccessfulMocks(request, personnel, reserve);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(reserve.CanceledReserve);
            _mockRepository.Verify(x => x.UpdateReserveAsync(reserve, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Database Exception Tests

        [Fact]
        public async Task Handle_WhenUpdateThrowsException_PropagatesException()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 12345);
            var personnel = new Personnel { PersonnelCode = 12345 };
            var reserve = new Reserve
            {
                Id = 100,
                PersonnelCode = 12345,
                CanceledReserve = false,
                ReserveDate = GetFutureDate()
            };

            _mockPersonnelRepository.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(personnel);

            _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
                .ReturnsAsync(true);

            _mockRepository.Setup(x => x.GetReserveAsync(request.ReserveId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reserve);

            _mockRepository.Setup(x => x.UpdateReserveAsync(It.IsAny<Reserve>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(
                () => _handler.Handle(request, CancellationToken.None));

            // Reserve should still be marked as canceled even though update failed
            Assert.True(reserve.CanceledReserve);
        }

        [Fact]
        public async Task Handle_WhenGetPersonnelThrowsException_PropagatesException()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 12345);

            _mockPersonnelRepository.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(request, CancellationToken.None));
        }

        #endregion

        #region Logger Verification Tests

        [Fact]
        public async Task Handle_WhenPersonnelNotFound_LogsCriticalError()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 99999);

            _mockPersonnelRepository.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Personnel?)null);

            // Act
            await Assert.ThrowsAsync<PersonnelNotExistException>(
                () => _handler.Handle(request, CancellationToken.None));

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(request.PersonnelCode.ToString())),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenReserveNotFound_LogsCriticalError()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 12345);
            var personnel = new Personnel { PersonnelCode = 12345 };

            _mockPersonnelRepository.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(personnel);

            _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
                .ReturnsAsync(true);

            _mockRepository.Setup(x => x.GetReserveAsync(request.ReserveId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Reserve?)null);

            // Act
            await Assert.ThrowsAsync<UnknownException>(
                () => _handler.Handle(request, CancellationToken.None));

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(request.PersonnelCode.ToString())),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCancelSomeoneElsesReserve_LogsCriticalError()
        {
            // Arrange
            var request = new CancelReserveRequest(100, 12345);
            var personnel = new Personnel { PersonnelCode = 12345 };
            var reserve = new Reserve
            {
                Id = 100,
                PersonnelCode = 88888,  // Different person
                CanceledReserve = false
            };

            _mockPersonnelRepository.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(personnel);

            _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
                .ReturnsAsync(true);

            _mockRepository.Setup(x => x.GetReserveAsync(request.ReserveId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reserve);

            // Act
            await Assert.ThrowsAsync<UnknownException>(
                () => _handler.Handle(request, CancellationToken.None));

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(request.PersonnelCode.ToString()) &&
                                                  o.ToString()!.Contains(reserve.PersonnelCode.ToString())),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region Helper Methods

        private void SetupSuccessfulMocks(CancelReserveRequest request, Personnel personnel, Reserve reserve)
        {
            _mockPersonnelRepository.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(personnel);

            _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
                .ReturnsAsync(true);

            _mockRepository.Setup(x => x.GetReserveAsync(request.ReserveId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reserve);

            _mockRepository.Setup(x => x.UpdateReserveAsync(It.IsAny<Reserve>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        private string GetFutureDate()
        {
            // Get a date that is definitely in the future (next week)
            var futureDate = DateTime.Now.AddDays(7);
            var persianDate = PersianDateTime.GetPersianDateByWeekDay((int)futureDate.DayOfWeek, 2);
            return persianDate;
        }

        private string GetPastDate()
        {
            // Get a date that is definitely in the past (last week)
            var pastDate = DateTime.Now.AddDays(-7);
            var persianDate = PersianDateTime.GetPersianDateByWeekDay((int)pastDate.DayOfWeek, -1);
            return persianDate;
        }

        #endregion
    }
}
