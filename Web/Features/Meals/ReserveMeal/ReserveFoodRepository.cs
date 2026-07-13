using Microsoft.EntityFrameworkCore;
using TSMA.Meal.Web.Database;
using TSMA.Meal.Web.Database.Entities;

namespace TSMA.Meal.Web.Features.Meals.ReserveMeal;

public interface IReserveFoodRepository
{
    Task<Database.Entities.Meal?> GetMealAsync(int id, CancellationToken cancellationToken);
    Task<Reserve?> GetCanceledReserveAsync(int personnelCode, string reserveDate, CancellationToken cancellationToken);



    Task AddReserveAsync(Reserve reserve, CancellationToken cancellationToken);
    Task UpdateReserveAsync(Reserve reserve, CancellationToken cancellationToken);
}

internal class ReserveFoodRepository(AppDbContext context) : IReserveFoodRepository
{

    public async Task<Database.Entities.Meal?> GetMealAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Meals.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }



    public async Task<Reserve?> GetCanceledReserveAsync(int personnelCode, string reserveDate, CancellationToken cancellationToken)
    {
        return await context.Reserve
            .FirstOrDefaultAsync(r => r.PersonnelCode == personnelCode && r.ReserveDate == reserveDate,
                cancellationToken);
    }



    public async Task AddReserveAsync(Reserve reserve, CancellationToken cancellationToken)
    {
        await context.Reserve.AddAsync(reserve, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateReserveAsync(Reserve reserve, CancellationToken cancellationToken)
    {
        context.Reserve.Update(reserve);

        await context.SaveChangesAsync(cancellationToken);
    }
}