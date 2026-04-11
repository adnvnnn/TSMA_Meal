using MediatR;
using Microsoft.EntityFrameworkCore;
using TSMA_Meal.Database;
using TSMA_Meal.Entities;
using TSMA_Meal.Shared;
using TSMA_Meal.Tools;

namespace TSMA_Meal.Features.Reserves;

public record MealsListRequest(int PersonnelCode, int PageId = 0) : IRequest<List<MealsViewModel>>;

internal class MealsListHandler(IMealsListRepository repository, IOffDaysRepository offDaysRepository, IPersonnelRepository personnelRepository)
    : IRequestHandler<MealsListRequest, List<MealsViewModel>>
{
    public async Task<List<MealsViewModel>> Handle(MealsListRequest request, CancellationToken cancellationToken)
    {

        if (request.PageId is > 1 or < -1) request = request with { PageId = 0 };

        //FOR TEST
        //request = request with { PageId =+ 2};


        //IS ACTIVE  

        var isActive = await personnelRepository.IsActive(request.PersonnelCode, cancellationToken);

        if (!isActive) throw new PersonnelFiredException();


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

        return weekMeals.Select(f => new MealsViewModel
        {
            Id = f.Id,
            OrderDay = f.OrderDay,
            Name = isWeekEven ? f.Name.Split('/').First() : f.Name.Split('/').Skip(1).First(),
            Day = f.Day,
            Restaurant = f.Restaurant,
            ImageUrl = isWeekEven ? f.ImageUrl : f.ImageUrl2,
            Price = f.Price,

            ReserveId = personnelReserves.FirstOrDefault(r => r.MealId == f.Id) != null ?
                personnelReserves.FirstOrDefault(r => r.MealId == f.Id).Id : 0,

            DiscountedPrice = Convert.ToInt32(f.Price) * (100 - discount) / 100,

            Date = PersianDateTime.GetPersianDateByWeekDay(f.OrderDay, request.PageId),

            IsExpired = request.PageId < 0 ||
                        (request.PageId == 0 && PersianDateTime.GetPersianDayOfWeek(f.OrderDay) 
                            <= PersianDateTime.GetPersianDayOfWeek((int)DateTime.Now.DayOfWeek/* + 1 FOR TEST*/)),

            IsAvailable = !invalidDays.Contains(PersianDateTime.GetPersianDateByWeekDay(f.OrderDay,request.PageId)),

            IsReserved = personnelReserves.Select(p => p.MealId).Contains(f.Id) &&
                         personnelReserves.FirstOrDefault(r => r.MealId == f.Id) != null ?
                            !personnelReserves.FirstOrDefault(r => r.MealId == f.Id).CanceledReserve : false

        }).ToList();

    }
}

public class MealsViewModel
{
    public bool IsReserved { get; set; }
    public bool IsExpired { get; set; }
    public bool IsAvailable { get; set; }
    public int ReserveId { get; set; }


    public int Id { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string Restaurant { get; set; }
    public int OrderDay { get; set; }
    public string Day { get; set; }
    public string Date { get; set; }
    public string Price { get; set; }
    public decimal DiscountedPrice { get; set; }
}



internal interface IMealsListRepository
{
    Task<decimal> GetPersonnelDiscountAsync(int personnelCode, CancellationToken cancellationToken);

    Task<List<Reserve>> GetPersonnelReservesIdAsync(int personnelCode, string startDate, string endDate, CancellationToken cancellationToken);

    Task<List<Meal>> GetMealsAsync(CancellationToken cancellationToken);
}

internal class MealsListRepository(AppDbContext context) : IMealsListRepository
{
    public async Task<decimal> GetPersonnelDiscountAsync(int personnelCode, CancellationToken cancellationToken)
    {
        var personnel = await context.Personnel.AsNoTracking()
            .FirstOrDefaultAsync(p => p.PersonnelCode == personnelCode, cancellationToken);

        if (personnel == null) throw new Exception();

        return personnel.Discount;
    }

    public async Task<List<Reserve>> GetPersonnelReservesIdAsync(int personnelCode, string startDate, string endDate, CancellationToken cancellationToken)
    {
        return await context.Reserve
            .Where(r => r.PersonnelCode == personnelCode)
            .Where(r => string.Compare(r.ReserveDate, startDate) >= 0 && string.Compare(r.ReserveDate, endDate) <= 0)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Meal>> GetMealsAsync(CancellationToken cancellationToken)
    {
        return await context.Meals
            .OrderBy(f => f.OrderListDay)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}




public static class MealsListRegister
{
    public static void RegisterMealsList(this IServiceCollection services)
    {
        services.AddScoped<IMealsListRepository, MealsListRepository>();
    }
}