using Microsoft.EntityFrameworkCore;
using TSMA_Meal.Database;
using TSMA_Meal.Database.Entities;

namespace TSMA_Meal.Features.Meals.MealsList;

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