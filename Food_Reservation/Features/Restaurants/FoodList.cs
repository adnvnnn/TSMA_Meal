using Azure.Core;
using Food_Reservation.Database;
using Food_Reservation.Tools;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Food_Reservation.Features.Restaurants;

public record FoodListRequest(int PersonnelCode) : IRequest<List<FoodListViewModel>>;

internal class FoodListHandler(AppDbContext context,IFoodListRepository repository) : IRequestHandler<FoodListRequest,List<FoodListViewModel>>
{
    public async Task<List<FoodListViewModel>> Handle(FoodListRequest request, CancellationToken cancellationToken)
    {

        var personnel = ;

        if (personnel == null) throw new Exception();

        if (!personnel.Active) throw new PersonnelFiredException();

        var isActive = await repository.IsPersonnelActive(request.PersonnelCode,cancellationToken);

        if (!isActive) throw new PersonnelNotAccessFoodListException();


        var today = DateTime.Now/*.AddDays(1)*/.DayOfWeek + 2;
        var daysToSaturday = today == DayOfWeek.Sunday ? 6 : (int)today;
        var startOfWeekDate = DateTime.Now.AddDays(-daysToSaturday).Date;
        var endOfWeekDate = startOfWeekDate.AddDays(6);

        var startOfWeek = startOfWeekDate.ToString("yyyy/MM/dd");
        var endOfWeek = endOfWeekDate.ToString("yyyy/MM/dd");

        try
        {
            var userReserved = await context.Reserve
                .Where(r => r.PersonnelCode == request.PersonnelCode)
                .Where(r => string.Compare(r.ReserveDate, startOfWeek) >= 0
                            && string.Compare(r.ReserveDate, endOfWeek) <= 0)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var reservedFoodIds = userReserved.Select(r => r.FoodId).ToHashSet();

            var allFoods = await context.Foods
                .OrderBy(f => f.OrderDay)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var weekDays = new List<DateTime>
            {
                startOfWeekDate.AddDays(0),
                startOfWeekDate.AddDays(1),
                startOfWeekDate.AddDays(2), 
                startOfWeekDate.AddDays(3), 
                startOfWeekDate.AddDays(4), 
                startOfWeekDate.AddDays(5), 
                startOfWeekDate.AddDays(6) 
            };


            var invalidDays = await context.MealOffDay.AsNoTracking().ToArrayAsync(cancellationToken);
            return allFoods.Select(f => new FoodListViewModel
            {
                Id = f.Id,
                OrderDay = f.OrderDay,
                Name = f.Name,
                Day = f.Day,
                Restaurant = f.Restaurant,
                ImageUrl = f.ImageUrl,
                Price = f.Price,
                DiscountedPrice = Convert.ToInt32(f.Price) * (100 - personnel.Discount) / 100,
                Date = weekDays[f.OrderDay].ToPersianDate(),
                IsExpired = f.OrderDay < (int)today + 1,
                IsAvailable = !invalidDays.Select(m => m.Date).Contains(weekDays[f.OrderDay].ToPersianDate()),
                IsReserved = reservedFoodIds.Contains(f.Id)
            }).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
}

public class FoodListViewModel
{
    public bool IsReserved { get; set; }
    public bool IsExpired { get; set; }
    public bool IsAvailable { get; set; }


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


//internal interface IFoodListAccess
//{
//    Task<bool> HasAccess(int personnelCode);
//}

//internal class FoodListAccess(AppDbContext context) : IFoodListAccess
//{
//    public async Task<bool> HasAccess(int personnelCode)
//    {


//        var personnel = await context.Personnel.FirstOrDefaultAsync(p => p.Active);
//    }
//}

public class PersonnelNotAccessFoodListException : Exception;

internal interface IFoodListRepository
{
    Task<bool> IsPersonnelActive(int personnelCode,CancellationToken token);
}
internal class FoodListRepository : IFoodListRepository
{
    public async Task<bool> IsPersonnelActive(int personnelCode, CancellationToken token)
    {
        await context.Personnel.IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.PersonnelCode == request.PersonnelCode, cancellationToken)
    }
}



