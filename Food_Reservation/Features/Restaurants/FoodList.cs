using Food_Reservation.Database;
using Food_Reservation.Entities;
using Food_Reservation.Shared;
using Food_Reservation.Tools;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Food_Reservation.Features.Restaurants;

public record FoodListRequest(int PersonnelCode,int PageId = 0) : IRequest<List<FoodListViewModel>>;

internal class FoodListHandler(IFoodListRepository repository, IOffDaysRepository offDaysRepository,IPersonnelRepository personnelRepository) : IRequestHandler<FoodListRequest,List<FoodListViewModel>>
{
    public async Task<List<FoodListViewModel>> Handle(FoodListRequest request, CancellationToken cancellationToken)
    {
        //IS ACTIVE  

        var isActive = await personnelRepository.IsActive(request.PersonnelCode,cancellationToken);

        if (!isActive) throw new PersonnelFiredException();



        //CHECK PERSONNEL ACCESS    

        if (!await personnelRepository.PersonnelHasAccess(request.PersonnelCode)) throw new NotPermittedException();




        //GET PERSONNEL FOODS

        var discount = await repository.GetPersonnelDiscountAsync(request.PersonnelCode,cancellationToken);

        var startOfWeek = PersianDateTime.GetPersianDateByWeekDay(0);

        var endOfWeek = PersianDateTime.GetPersianDateByWeekDay(6);


        var personnelFoodsId = await repository
            .GetPersonnelReservesIdAsync(request.PersonnelCode,startOfWeek,endOfWeek, cancellationToken);

        var weekFoods = await repository.GetFoodsAsync(cancellationToken);

        var invalidDays = await offDaysRepository.GetOffDaysAsync(cancellationToken);


        return weekFoods.Select(f => new FoodListViewModel
        {
            Id = f.Id,
            OrderDay = f.OrderDay,
            Name = f.Name,
            Day = f.Day,
            Restaurant = f.Restaurant,
            ImageUrl = f.ImageUrl,
            Price = f.Price,

            DiscountedPrice = Convert.ToInt32(f.Price) * (100 - discount) / 100,

            Date = PersianDateTime.GetPersianDateByWeekDay(f.OrderDay),

            IsExpired = f.OrderDay < (int)PersianDateTime.GetTodayWeekDay() + 1,

            IsAvailable = !invalidDays.Contains(PersianDateTime.GetPersianDateByWeekDay(f.OrderDay)),

            IsReserved = personnelFoodsId.Contains(f.Id)
        }).ToList();

    }
}

public class FoodListViewModel
{
    public bool IsReserved { get; set; }
    public bool IsExpired { get; set; }
    public bool IsAvailable { get; set; }
    public int? ReserveId { get; set; }


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



internal interface IFoodListRepository
{

    Task<decimal> GetPersonnelDiscountAsync(int personnelCode, CancellationToken cancellationToken);

    Task<List<int>> GetPersonnelReservesIdAsync(int personnelCode,string startDate, string endDate, CancellationToken cancellationToken);

    Task<List<Food>> GetFoodsAsync(CancellationToken cancellationToken);
}
internal class FoodListRepository(AppDbContext context) : IFoodListRepository
{


    public async Task<decimal> GetPersonnelDiscountAsync(int personnelCode, CancellationToken cancellationToken)
    {
        var personnel = await context.Personnel.AsNoTracking()
            .FirstOrDefaultAsync(p => p.PersonnelCode == personnelCode, cancellationToken);

        if (personnel == null) throw new Exception();

        return personnel.Discount;
    }

    public async Task<List<int>> GetPersonnelReservesIdAsync(int personnelCode, string startDate, string endDate, CancellationToken cancellationToken)
    {
        return await context.Reserve
            .Where(r => r.PersonnelCode == personnelCode)
            .Where(r => string.Compare(r.ReserveDate, startDate) >= 0 && string.Compare(r.ReserveDate, endDate) <= 0)
            .AsNoTracking()
            .Select(r => r.FoodId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Food>> GetFoodsAsync(CancellationToken cancellationToken)
    {
        return await context.Foods
            .OrderBy(f => f.OrderDay)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}




public static class FoodListRegister
{
    public static void RegisterFoodList(this IServiceCollection services)
    {
        services.AddScoped<IFoodListRepository, FoodListRepository>();
    }
}