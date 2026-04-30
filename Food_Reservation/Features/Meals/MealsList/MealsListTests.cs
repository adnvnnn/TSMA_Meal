using global::TSMA_Meal.Database.Entities;
using global::TSMA_Meal.Features.Account;
using global::TSMA_Meal.Shared;
using global::TSMA_Meal.Tools;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using TSMA_Meal.Database.Entities;
using TSMA_Meal.Features.Account;
using TSMA_Meal.Shared;
using TSMA_Meal.Tools;
using Xunit;
using Assert = Xunit.Assert;
namespace TSMA_Meal.Features.Meals.MealsList;

public class MealsListHandlerTests
{
    private readonly Mock<IMealsListRepository> _mockRepository;
    private readonly Mock<IOffDaysRepository> _mockOffDaysRepository;
    private readonly Mock<IPersonnelRepository> _mockPersonnelRepository;
    private readonly MealsListHandler _handler;

    public MealsListHandlerTests()
    {
        _mockRepository = new Mock<IMealsListRepository>();
        _mockOffDaysRepository = new Mock<IOffDaysRepository>();
        _mockPersonnelRepository = new Mock<IPersonnelRepository>();

        _handler = new MealsListHandler(
            _mockRepository.Object,
            _mockOffDaysRepository.Object,
            _mockPersonnelRepository.Object);
    }

    #region Validation Tests

    [Fact]
    public async Task Handle_ValidRequest_ReturnsMealsList()
    {
        // Arrange
        var pageId = 0;
        var request = new MealsListRequest(12345, pageId);
        var discount = 10m;
        var meals = GetSampleMeals();
        var reserves = new List<Reserve>();
        var offDays = new List<string>();

        SetupSuccessfulMocks(request.PersonnelCode, discount, meals, reserves, offDays,pageId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("صبحانه", result[0].Name);
        Assert.Equal(90, result[0].DiscountedPrice);
    }

    [Theory]
    [InlineData(5)]   // PageId > 1
    [InlineData(-2)]  // PageId < -1
    public async Task Handle_InvalidPageId_ResetsToZero(int invalidPageId)
    {
        // Arrange
        var request = new MealsListRequest(12345, invalidPageId);
        var discount = 10m;
        var meals = GetSampleMeals();
        var reserves = new List<Reserve>();
        var offDays = new List<string>();

        SetupSuccessfulMocks(request.PersonnelCode, discount, meals, reserves, offDays);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        // PageId should be reset to 0 so dates are calculated with PageId=0
        Assert.Equal(3, result.Count);
    }

    #endregion

    #region Personnel Validation Tests

    [Fact]
    public async Task Handle_PersonnelNotActive_ThrowsPersonnelNotExistException()
    {
        // Arrange
        var request = new MealsListRequest(12345, 0);

        _mockPersonnelRepository.Setup(x => x.IsActive(request.PersonnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(It.IsAny<int>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<PersonnelNotExistException>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_PersonnelNoAccess_ThrowsNotPermittedException()
    {
        // Arrange
        var request = new MealsListRequest(12345, 0);

        _mockPersonnelRepository.Setup(x => x.IsActive(request.PersonnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotPermittedException>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    #endregion

    #region Data Tests

    [Fact]
    public async Task Handle_CalculatesDiscountedPriceCorrectly()
    {
        // Arrange
        var request = new MealsListRequest(12345, 0);
        var discount = 20m;  // 20% discount
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = 1,
                Price = "100000",
                OrderDay = 1,
                Name = "غذا",
                Day = "شنبه",
                Restaurant = "رستوران",
                ImageUrl = "img.jpg",
                ImageUrl2 = "img2.jpg"
            }
        };
        var reserves = new List<Reserve>();
        var offDays = new List<string>();

        SetupSuccessfulMocks(request.PersonnelCode, discount, meals, reserves, offDays);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var expectedPrice = 100000 * (100 - 20) / 100;
        Assert.Equal(expectedPrice, result[0].DiscountedPrice);
    }

    [Fact]
    public async Task Handle_WhenMealHasTwoPartName_SelectsCorrectPartBasedOnWeekParity()
    {
        // Arrange
        var discount = 0m;
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = 1,
                Price = "50000",
                OrderDay = 1,
                Name = "غرفه یک/غرفه دو",  // هفته زوج => غرفه دو, هفته فرد => غرفه یک
                Day = "شنبه",
                Restaurant = "رستوران",
                ImageUrl = "img1.jpg",
                ImageUrl2 = "img2.jpg"
            }
        };
        var reserves = new List<Reserve>();
        var offDays = new List<string>();

        // Test with even week (PageId=2)
        var evenWeekRequest = new MealsListRequest(12345, 0);
        SetupSuccessfulMocks(evenWeekRequest.PersonnelCode, discount, meals, reserves, offDays,0);
        var evenWeekResult = await _handler.Handle(evenWeekRequest, CancellationToken.None);

        // Test with odd week (PageId=1)
        var oddWeekRequest = new MealsListRequest(12345, 1);
        SetupSuccessfulMocks(oddWeekRequest.PersonnelCode, discount, meals, reserves, offDays,1);
        var oddWeekResult = await _handler.Handle(oddWeekRequest, CancellationToken.None);

        // Assert
        Assert.Equal("غرفه دو", evenWeekResult[0].Name);
        Assert.Equal("غرفه یک", oddWeekResult[0].Name);
    }

    [Fact]
    public async Task Handle_WhenDateIsOffDay_SetsIsAvailableFalse()
    {
        // Arrange
        var request = new MealsListRequest(12345, 0);
        var discount = 0m;
        var meals = GetSampleMeals();
        var reserves = new List<Reserve>();

        // Get the reserve date for a meal and mark it as off day
        var mealDate = PersianDateTime.GetPersianDateByWeekDay(meals[0].OrderDay, request.PageId);
        var offDays = new List<string> { mealDate };

        SetupSuccessfulMocks(request.PersonnelCode, discount, meals, reserves, offDays);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result[0].IsAvailable);
    }

    [Fact]
    public async Task Handle_WhenMealIsReservedAndNotCanceled_SetsIsReservedTrue()
    {
        // Arrange
        var pageId = 1;
        var request = new MealsListRequest(12345, pageId);
        var discount = 0m;
        var meals = GetSampleMeals();
        var reserves = new List<Reserve>
        {
            new Reserve { Id = 100, MealId = meals[0].Id, CanceledReserve = false }
        };
        var offDays = new List<string>();

        SetupSuccessfulMocks(request.PersonnelCode, discount, meals, reserves, offDays, pageId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result[0].IsReserved);
        Assert.Equal(100, result[0].ReserveId);
    }

    [Fact]
    public async Task Handle_WhenMealIsReservedButCanceled_SetsIsReservedFalse()
    {
        // Arrange
        var pageId = 1;
        var request = new MealsListRequest(12345, pageId);
        var discount = 0m;
        var meals = GetSampleMeals();
        var reserves = new List<Reserve>
        {
            new Reserve { Id = 100, MealId = meals[0].Id, CanceledReserve = true }  // Canceled
        };
        var offDays = new List<string>();

        SetupSuccessfulMocks(request.PersonnelCode, discount, meals, reserves, offDays, pageId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result[0].IsReserved);
        Assert.Equal(100, result[0].ReserveId);  // ReserveId still exists
    }

    [Fact]
    public async Task Handle_WhenMultipleReserves_MatchesCorrectMeal()
    {
        // Arrange
        var pageId = 1;
        var request = new MealsListRequest(12345, pageId);
        var discount = 0m;
        var meals = GetSampleMeals();
        var reserves = new List<Reserve>
        {
            new Reserve { Id = 100, MealId = meals[0].Id, CanceledReserve = false },
            new Reserve { Id = 200, MealId = meals[2].Id, CanceledReserve = false }
        };
        var offDays = new List<string>();

        SetupSuccessfulMocks(request.PersonnelCode, discount, meals, reserves, offDays, pageId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result[0].IsReserved);  // Meal 1 is reserved
        Assert.False(result[1].IsReserved); // Meal 2 is not reserved
        Assert.True(result[2].IsReserved);  // Meal 3 is reserved
        Assert.Equal(100, result[0].ReserveId);
        Assert.Equal(200, result[2].ReserveId);
    }

    #endregion

    #region Expiration Tests

    [Fact]
    public async Task Handle_WhenPageIdNegative_SetsIsExpiredTrue()
    {
        // Arrange
        var pageId = -1;
        var request = new MealsListRequest(12345, pageId);  // Past week
        var discount = 0m;
        var meals = GetSampleMeals();
        var reserves = new List<Reserve>();
        var offDays = new List<string>();

        SetupSuccessfulMocks(request.PersonnelCode, discount, meals, reserves, offDays, pageId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result[0].IsExpired);
        Assert.True(result[1].IsExpired);
        Assert.True(result[2].IsExpired);
    }

    [Fact]
    public async Task Handle_WhenOrderDayIsTodayOrPast_SetsIsExpiredTrueForCurrentWeek()
    {
        // Arrange
        var request = new MealsListRequest(12345, 0);  // Current week
        var discount = 0m;
        var currentDayOfWeek = (int)DateTime.Now.DayOfWeek;

        // Create meals for past days relative to current time
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = 1,
                Price = "50000",
                OrderDay = currentDayOfWeek - 1,  // Yesterday
                Name = "غذا دیروز",
                Day = "دیروز",
                Restaurant = "رستوران",
                ImageUrl = "img.jpg",
                ImageUrl2 = "img2.jpg"
            },
            new Meal
            {
                Id = 2,
                Price = "50000",
                OrderDay = currentDayOfWeek + 1,  // Tomorrow
                Name = "غذا فردا",
                Day = "فردا",
                Restaurant = "رستوران",
                ImageUrl = "img.jpg",
                ImageUrl2 = "img2.jpg"
            }
        };
        var reserves = new List<Reserve>();
        var offDays = new List<string>();

        SetupSuccessfulMocks(request.PersonnelCode, discount, meals, reserves, offDays);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result[0].IsExpired);  // Yesterday's meal should be expired
        Assert.False(result[1].IsExpired); // Tomorrow's meal should not be expired
    }

    #endregion

    #region Image Selection Tests

    [Fact]
    public async Task Handle_WhenWeekIsEven_UsesImageUrl2()
    {
        // Arrange
        var request = new MealsListRequest(12345, 2);  // Even week
        var discount = 0m;
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = 1,
                Price = "50000",
                OrderDay = 1,
                Name = "غذا",
                Day = "شنبه",
                Restaurant = "رستوران",
                ImageUrl = "img1.jpg",
                ImageUrl2 = "img2.jpg"
            }
        };
        var reserves = new List<Reserve>();
        var offDays = new List<string>();

        SetupSuccessfulMocks(request.PersonnelCode, discount, meals, reserves, offDays);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal("img2.jpg", result[0].ImageUrl);
    }

    [Fact]
    public async Task Handle_WhenWeekIsOdd_UsesImageUrl()
    {
        // Arrange
        var pageId = 1;
        var request = new MealsListRequest(12345, pageId);  // Odd week
        var discount = 0m;
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = 1,
                Price = "50000",
                OrderDay = 1,
                Name = "غذا",
                Day = "شنبه",
                Restaurant = "رستوران",
                ImageUrl = "img1.jpg",
                ImageUrl2 = "img2.jpg"
            }
        };
        var reserves = new List<Reserve>();
        var offDays = new List<string>();

        SetupSuccessfulMocks(request.PersonnelCode, discount, meals, reserves, offDays, pageId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal("img1.jpg", result[0].ImageUrl);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task Handle_WhenPersonnelNotFoundInDiscount_ThrowsException()
    {
        // Arrange
        var request = new MealsListRequest(99999, 0);

        _mockPersonnelRepository.Setup(x => x.IsActive(request.PersonnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
            .ReturnsAsync(true);

        _mockRepository.Setup(x => x.GetPersonnelDiscountAsync(request.PersonnelCode, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Personnel not found"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var request = new MealsListRequest(12345, 0);

        _mockPersonnelRepository.Setup(x => x.IsActive(request.PersonnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
            .ReturnsAsync(true);

        _mockRepository.Setup(x => x.GetMealsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenOffDaysRepositoryThrows_PropagatesException()
    {
        // Arrange
        var request = new MealsListRequest(12345, 0);

        _mockPersonnelRepository.Setup(x => x.IsActive(request.PersonnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(request.PersonnelCode))
            .ReturnsAsync(true);

        _mockOffDaysRepository.Setup(x => x.GetOffDaysAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Off days service error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public async Task Handle_WhenMealNameHasNoSlash_ReturnsFullName()
    {
        // Arrange
        var pageId = 1;
        var request = new MealsListRequest(12345, pageId);
        var discount = 0m;
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = 1,
                Price = "50000",
                OrderDay = 1,
                Name = "غذا ساده",  // No slash
                Day = "شنبه",
                Restaurant = "رستوران",
                ImageUrl = "img.jpg",
                ImageUrl2 = "img2.jpg"
            }
        };
        var reserves = new List<Reserve>();
        var offDays = new List<string>();

        SetupSuccessfulMocks(request.PersonnelCode, discount, meals, reserves, offDays, pageId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal("غذا ساده", result[0].Name);
    }

    [Fact]
    public async Task Handle_WhenNoReserves_ReserveIdIsZero()
    {
        // Arrange
        var pageId = 1;
        var request = new MealsListRequest(12345, pageId);
        var discount = 0m;
        var meals = GetSampleMeals();
        var reserves = new List<Reserve>();  // Empty reserves
        var offDays = new List<string>();

        SetupSuccessfulMocks(request.PersonnelCode, discount, meals, reserves, offDays, pageId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(0, result[0].ReserveId);
        Assert.False(result[0].IsReserved);
    }

    [Fact]
    public async Task Handle_WhenMealAvailable_IsAvailableTrue()
    {
        // Arrange
        var request = new MealsListRequest(12345, 0);
        var discount = 0m;
        var meals = GetSampleMeals();
        var reserves = new List<Reserve>();
        var offDays = new List<string>();  // No off days

        SetupSuccessfulMocks(request.PersonnelCode, discount, meals, reserves, offDays);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result[0].IsAvailable);
    }

    #endregion

    #region Helper Methods

    private void SetupSuccessfulMocks(
        int personnelCode,
        decimal discount,
        List<Meal> meals,
        List<Reserve> reserves,
        List<string> offDays,
        int pageId = 0)
    {
        _mockPersonnelRepository.Setup(x => x.IsActive(personnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockPersonnelRepository.Setup(x => x.PersonnelHasAccess(personnelCode))
            .ReturnsAsync(true);

        _mockRepository.Setup(x => x.GetPersonnelDiscountAsync(personnelCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(discount);

        var startOfWeek = PersianDateTime.GetPersianDateByWeekDay((int)DayOfWeek.Saturday, pageId);
        var endOfWeek = PersianDateTime.GetPersianDateByWeekDay((int)DayOfWeek.Friday, pageId);
        _mockRepository.Setup(x => x.GetPersonnelReservesIdAsync(
                personnelCode,
                startOfWeek,
                endOfWeek,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(reserves);

        _mockRepository.Setup(x => x.GetMealsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(meals);

        _mockOffDaysRepository.Setup(x => x.GetOffDaysAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(offDays);
    }

    private List<Meal> GetSampleMeals()
    {
        return new List<Meal>
        {
            new Meal
            {
                Id = 1,
                Name = "صبحانه",
                Price = "100",
                OrderDay = 1,
                Day = "شنبه",
                Restaurant = "رستوران مرکزی",
                ImageUrl = "breakfast.jpg",
                ImageUrl2 = "breakfast2.jpg"
            },
            new Meal
            {
                Id = 2,
                Name = "ناهار",
                Price = "200",
                OrderDay = 2,
                Day = "یکشنبه",
                Restaurant = "رستوران شرق",
                ImageUrl = "lunch.jpg",
                ImageUrl2 = "lunch2.jpg"
            },
            new Meal
            {
                Id = 3,
                Name = "شام",
                Price = "150",
                OrderDay = 3,
                Day = "دوشنبه",
                Restaurant = "رستوران غرب",
                ImageUrl = "dinner.jpg",
                ImageUrl2 = "dinner2.jpg"
            }
        };
    }

    #endregion
}