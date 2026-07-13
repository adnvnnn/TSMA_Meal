using Microsoft.EntityFrameworkCore;
using TSMA.Meal.Web.Database;
using TSMA.Meal.Web.Database.Entities;

namespace TSMA.Meal.Web.Features.Meals.CancelReserve;

public interface ICancelReserveRepository
{
    Task<Reserve?> GetReserveAsync(int id, CancellationToken cancellationToken);

    Task UpdateReserveAsync(Reserve reserve, CancellationToken cancellationToken);

}

internal class CancelReserveRepository(AppDbContext context) : ICancelReserveRepository
{
    public async Task<Reserve?> GetReserveAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Reserve
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task UpdateReserveAsync(Reserve reserve, CancellationToken cancellationToken)
    {
        context.Reserve.Update(reserve);

        await context.SaveChangesAsync(cancellationToken);
    }
}