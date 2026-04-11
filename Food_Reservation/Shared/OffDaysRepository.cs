using Microsoft.EntityFrameworkCore;
using TSMA_Meal.Database;

namespace TSMA_Meal.Shared;

public interface IOffDaysRepository
{
    Task<List<string>> GetOffDaysAsync(CancellationToken cancellationToken);

}
public class OffDaysRepository(AppDbContext context) : IOffDaysRepository
{
    public async Task<List<string>> GetOffDaysAsync(CancellationToken cancellationToken)
    {
        return await context.MealOffDay.Select(o => o.Date).AsNoTracking().ToListAsync(cancellationToken);
    }
}