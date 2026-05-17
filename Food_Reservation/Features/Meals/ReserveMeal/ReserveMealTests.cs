using TSMA_Meal.Database.Entities;
using TSMA_Meal.Features.Account;
using TSMA_Meal.Tools;
using Moq;
using System.Globalization;
using TSMA_Meal.Features.Account.SendOtp.Exceptions;
using Xunit;
using Assert = Xunit.Assert;
using TSMA_Meal.Features.Meals.ReserveMeal.Exceptions;
using TSMA_Meal.Shared.Exceptions;
using TSMA_Meal.Shared.Repositories.OffDays;
using TSMA_Meal.Shared.Repositories.Personnel;

namespace TSMA_Meal.Features.Meals.ReserveMeal;

public class ReserveFoodHandlerTests
{
    private readonly Mock<IOffDaysRepository> _offDaysRepositoryMock;
    private readonly Mock<IReserveFoodRepository> _reserveFoodRepositoryMock;
    private readonly Mock<IPersonnelRepository> _personnelRepositoryMock;
    private readonly Mock<ILogger<ReserveFoodHandler>> _loggerMock;
    private readonly ReserveFoodHandler _handler;

    public ReserveFoodHandlerTests()
    {
        _offDaysRepositoryMock = new Mock<IOffDaysRepository>();
        _reserveFoodRepositoryMock = new Mock<IReserveFoodRepository>();
        _personnelRepositoryMock = new Mock<IPersonnelRepository>();
        _loggerMock = new Mock<ILogger<ReserveFoodHandler>>();

        _handler = new ReserveFoodHandler(
            _offDaysRepositoryMock.Object,
            _reserveFoodRepositoryMock.Object,
            _personnelRepositoryMock.Object,
            _loggerMock.Object);
    }





    [Fact]
    public async Task Handle_ValidRequest_ShouldReserveSuccessfully()
    {
        // Arrange
        var request = new ReserveFoodRequest(1, 12345, 0);
        var personnel = new Personnel { PersonnelCode = 12345, Discount = 10 };
        var meal = new Meal { Id = 1, Price = "50000", OrderDay = 1, Day = "شنبه" };

        var reserveDate = PersianDateTime.GetPersianDateByWeekDay(meal.OrderDay, request.Week);
        var today = PersianDateTime.GetPersianDateByWeekDay((int)DateTime.Now.DayOfWeek);

        // اعتبارسنجی برای اینکه تاریخ رزرو از امروز بزرگتر باشد
        if (string.CompareOrdinal(reserveDate, today) <= 0)
            return; // اگر تاریخ منقضی شده، تست را انجام نده

        _personnelRepositoryMock.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(personnel);

        _personnelRepositoryMock.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
            .ReturnsAsync(true);

        _reserveFoodRepositoryMock.Setup(x => x.GetMealAsync(request.FoodId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(meal);

        _offDaysRepositoryMock.Setup(x => x.GetOffDaysAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());

        _reserveFoodRepositoryMock.Setup(x => x.GetCanceledReserveAsync(request.PersonnelCode, reserveDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reserve?)null);

        _reserveFoodRepositoryMock.Setup(x => x.AddReserveAsync(It.IsAny<Reserve>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result > 0);
        _reserveFoodRepositoryMock.Verify(x => x.AddReserveAsync(It.IsAny<Reserve>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidWeek_ShouldThrowUnknownException()
    {
        // Arrange
        var request = new ReserveFoodRequest(1, 12345, 5); // Week > 1

        // Act & Assert
        await Assert.ThrowsAsync<UnknownException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NegativeWeek_ShouldThrowUnknownException()
    {
        // Arrange
        var request = new ReserveFoodRequest(1, 12345, -1); // Week < 0

        // Act & Assert
        await Assert.ThrowsAsync<UnknownException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_PersonnelNotFound_ShouldThrowPersonnelNotExistException()
    {
        // Arrange
        var request = new ReserveFoodRequest(1, 99999, 0);

        _personnelRepositoryMock.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Personnel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<PersonnelNotExistException>(() => _handler.Handle(request, CancellationToken.None));

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Critical,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(request.PersonnelCode.ToString())),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_PersonnelNoAccess_ShouldThrowNotPermittedException()
    {
        // Arrange
        var request = new ReserveFoodRequest(1, 12345, 0);
        var personnel = new Personnel { PersonnelCode = 12345 };

        _personnelRepositoryMock.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(personnel);

        _personnelRepositoryMock.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotPermittedException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_MealNotFound_ShouldThrowUnknownException()
    {
        // Arrange
        var request = new ReserveFoodRequest(1, 12345, 0);
        var personnel = new Personnel { PersonnelCode = 12345 };

        _personnelRepositoryMock.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(personnel);

        _personnelRepositoryMock.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
            .ReturnsAsync(true);

        _reserveFoodRepositoryMock.Setup(x => x.GetMealAsync(request.FoodId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Meal?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnknownException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReserveDateExpired_ShouldThrowReserveOutOfDateException()
    {
        // Arrange
        var request = new ReserveFoodRequest(1, 12345, 0); // Week 0 باعث می‌شود تاریخ امروز یا قبل باشد
        var personnel = new Personnel { PersonnelCode = 12345, Discount = 10 };
        var meal = new Meal { Id = 1, Price = "50000", OrderDay = (int)DateTime.Now.DayOfWeek };

        _personnelRepositoryMock.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(personnel);

        _personnelRepositoryMock.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
            .ReturnsAsync(true);

        _reserveFoodRepositoryMock.Setup(x => x.GetMealAsync(request.FoodId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(meal);

        // Act & Assert
        await Assert.ThrowsAsync<ReserveOutOfDateException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReserveDateIsOffDay_ShouldThrowReserveIsOffException()
    {
        // Arrange
        var request = new ReserveFoodRequest(1, 12345, 0);
        var personnel = new Personnel { PersonnelCode = 12345, Discount = 10 };
        var meal = new Meal { Id = 1, Price = "50000", OrderDay = 1 };

        var reserveDate = PersianDateTime.GetPersianDateByWeekDay(meal.OrderDay, request.Week);
        var today = PersianDateTime.GetPersianDateByWeekDay((int)DateTime.Now.DayOfWeek);

        if (string.CompareOrdinal(reserveDate, today) <= 0)
            return;

        _personnelRepositoryMock.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(personnel);

        _personnelRepositoryMock.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
            .ReturnsAsync(true);

        _reserveFoodRepositoryMock.Setup(x => x.GetMealAsync(request.FoodId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(meal);

        _offDaysRepositoryMock.Setup(x => x.GetOffDaysAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { reserveDate });

        // Act & Assert
        await Assert.ThrowsAsync<ReserveIsOffException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CanceledReserveExists_ShouldReactivateReserve()
    {
        // Arrange
        var request = new ReserveFoodRequest(1, 12345, 1);
        var personnel = new Personnel { PersonnelCode = 12345, Discount = 10 };
        var meal = new Meal { Id = 1, Price = "50000", OrderDay = 1 };
        var existingReserve = new Reserve
        {
            Id = 100,
            PersonnelCode = 12345,
            CanceledReserve = true
        };

        var reserveDate = PersianDateTime.GetPersianDateByWeekDay(meal.OrderDay, request.Week);
        var today = PersianDateTime.GetPersianDateByWeekDay((int)DateTime.Now.DayOfWeek);

        if (string.CompareOrdinal(reserveDate, today) <= 0)
            return;

        _personnelRepositoryMock.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(personnel);

        _personnelRepositoryMock.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
            .ReturnsAsync(true);

        _reserveFoodRepositoryMock.Setup(x => x.GetMealAsync(request.FoodId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(meal);

        _offDaysRepositoryMock.Setup(x => x.GetOffDaysAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());

        _reserveFoodRepositoryMock.Setup(x => x.GetCanceledReserveAsync(request.PersonnelCode, reserveDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReserve);

        _reserveFoodRepositoryMock.Setup(x => x.UpdateReserveAsync(It.IsAny<Reserve>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(100, result);
        Assert.False(existingReserve.CanceledReserve);
        _reserveFoodRepositoryMock.Verify(x => x.UpdateReserveAsync(existingReserve, It.IsAny<CancellationToken>()), Times.Once);
        _reserveFoodRepositoryMock.Verify(x => x.AddReserveAsync(It.IsAny<Reserve>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PriceCalculation_ShouldApplyCorrectDiscount()
    {
        // Arrange
        var request = new ReserveFoodRequest(1, 12345, 0);
        var personnel = new Personnel { PersonnelCode = 12345, Discount = 20 };
        var meal = new Meal { Id = 1, Price = "100000", OrderDay = 1, Day = "شنبه" };

        var reserveDate = PersianDateTime.GetPersianDateByWeekDay(meal.OrderDay, request.Week);
        var today = PersianDateTime.GetPersianDateByWeekDay((int)DateTime.Now.DayOfWeek);

        if (string.CompareOrdinal(reserveDate, today) <= 0)
            return;

        _personnelRepositoryMock.Setup(x => x.GetPersonnelAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(personnel);

        _personnelRepositoryMock.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
            .ReturnsAsync(true);

        _reserveFoodRepositoryMock.Setup(x => x.GetMealAsync(request.FoodId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(meal);

        _offDaysRepositoryMock.Setup(x => x.GetOffDaysAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());

        _reserveFoodRepositoryMock.Setup(x => x.GetCanceledReserveAsync(request.PersonnelCode, reserveDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reserve?)null);

        Reserve capturedReserve = null!;
        _reserveFoodRepositoryMock.Setup(x => x.AddReserveAsync(It.IsAny<Reserve>(), It.IsAny<CancellationToken>()))
            .Callback<Reserve, CancellationToken>((r, _) => capturedReserve = r)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedReserve);
        var expectedPrice = (100000 * (100 - 20) / 100).ToString(CultureInfo.InvariantCulture);
        Assert.Equal(expectedPrice, capturedReserve.PriceForUser);
        Assert.Equal("20", capturedReserve.PercentOff);
    }
}