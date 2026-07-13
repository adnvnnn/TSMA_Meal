using MediatR;
using TSMA.Meal.Web.Features.Account.SendOtp.Exceptions;
using TSMA.Meal.Web.Shared.Exceptions;
using TSMA.Meal.Web.Shared.Repositories.OffDays;
using TSMA.Meal.Web.Shared.Repositories.Personnel;
using TSMA.Meal.Web.Tools;

namespace TSMA.Meal.Web.Features.Meals.MealsList;

public class MealsListHandler(IMealsListRepository repository, IOffDaysRepository offDaysRepository, IPersonnelRepository personnelRepository)
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

        return weekMeals.Select(f => {
            var reserve = personnelReserves.FirstOrDefault(r => r.MealId == f.Id);
            var nameParts = f.Name?.Split('/') ?? new[] { f.Name ?? "" };

            return new MealsListViewModel
            {
                Id = f.Id,
                OrderDay = f.OrderDay,
                Name = isWeekEven
                    ? (nameParts.Length > 1 ? nameParts[1] : nameParts[0])
                    : nameParts[0],
                Day = f.Day,
                Restaurant = f.Restaurant,
                ImageUrl = isWeekEven ? f.ImageUrl2 : f.ImageUrl,
                Price = f.Price,
                ReserveId = reserve?.Id ?? 0,
                DiscountedPrice = Convert.ToInt32(f.Price) * (100 - discount) / 100,
                Date = PersianDateTime.GetPersianDateByWeekDay(f.OrderDay, request.PageId),
                IsExpired = request.PageId < 0 ||
                            (request.PageId == 0 && PersianDateTime.GetPersianDayOfWeek(f.OrderDay)
                                <= PersianDateTime.GetPersianDayOfWeek((int)DateTime.Now.DayOfWeek)),
                IsAvailable = !invalidDays.Contains(PersianDateTime.GetPersianDateByWeekDay(f.OrderDay, request.PageId)),
                IsReserved = reserve != null && !reserve.CanceledReserve
            };
        }).ToList();

    }

    private string GetMealName(string name, bool isWeekEven)
    {
        if (string.IsNullOrEmpty(name)) return string.Empty;

        var parts = name.Split('/');
        if (isWeekEven)
            return parts.Length > 1 ? parts[1] : parts[0];
        else
            return parts[0];
    }
}