using MediatR;
using TSMA_Meal.Features.Account;
using TSMA_Meal.Shared;
using TSMA_Meal.Tools;

namespace TSMA_Meal.Features.Meals.MealsList;

internal class MealsListHandler(IMealsListRepository repository, IOffDaysRepository offDaysRepository, IPersonnelRepository personnelRepository)
    : IRequestHandler<MealsListRequest, List<MealsListViewModel>>
{
    public async Task<List<MealsListViewModel>> Handle(MealsListRequest request, CancellationToken cancellationToken)
    {

        if (request.PageId is > 1 or < -1) request = request with { PageId = 0 };


        //IS ACTIVE  

        var isActive = await personnelRepository.IsActive(request.PersonnelCode, cancellationToken);

        if (!isActive) throw new PersonnelNotExistException();


        //CHECK PERSONNEL ACCESS    

        if (!await personnelRepository.PersonnelHasAccess(request.PersonnelCode)) throw new NotPermittedException();


        //GET PERSONNEL MEALS

        var discount = await repository.GetPersonnelDiscountAsync(request.PersonnelCode, cancellationToken);

        var startOfWeek = PersianDateTime.GetPersianDateByWeekDay((int)DayOfWeek.Saturday, request.PageId);

        var endOfWeek = PersianDateTime.GetPersianDateByWeekDay((int)DayOfWeek.Friday, request.PageId);


        var personnelReserves = await repository.GetPersonnelReservesIdAsync(request.PersonnelCode, startOfWeek, endOfWeek, cancellationToken);

        var weekMeals = await repository.GetMealsAsync(cancellationToken);

        var invalidDays = await offDaysRepository.GetOffDaysAsync(cancellationToken);

        var isWeekEven = PersianDateTime.IsEvenWeek(request.PageId);

        return weekMeals.Select(f => new MealsListViewModel
        {
            Id = f.Id,
            OrderDay = f.OrderDay,
            Name = isWeekEven
                ? f.Name.Split('/').Length > 1 ? f.Name.Split('/').Skip(1).First() : f.Name
                : f.Name.Split('/').FirstOrDefault() ?? f.Name,
            Day = f.Day,
            Restaurant = f.Restaurant,
            ImageUrl = isWeekEven ? f.ImageUrl2 : f.ImageUrl,
            Price = f.Price,

            ReserveId = personnelReserves.FirstOrDefault(r => r.MealId == f.Id) != null ?
                personnelReserves.FirstOrDefault(r => r.MealId == f.Id).Id : 0,

            DiscountedPrice = Convert.ToInt32(f.Price) * (100 - discount) / 100,

            Date = PersianDateTime.GetPersianDateByWeekDay(f.OrderDay, request.PageId),

            IsExpired = request.PageId < 0 ||
                        (request.PageId == 0 && PersianDateTime.GetPersianDayOfWeek(f.OrderDay)
                            <= PersianDateTime.GetPersianDayOfWeek((int)DateTime.Now.DayOfWeek)),

            IsAvailable = !invalidDays.Contains(PersianDateTime.GetPersianDateByWeekDay(f.OrderDay, request.PageId)),

            IsReserved = personnelReserves.Select(p => p.MealId).Contains(f.Id) &&
                         personnelReserves.FirstOrDefault(r => r.MealId == f.Id) != null ?
                !personnelReserves.FirstOrDefault(r => r.MealId == f.Id).CanceledReserve : false

        }).ToList();

    }
}