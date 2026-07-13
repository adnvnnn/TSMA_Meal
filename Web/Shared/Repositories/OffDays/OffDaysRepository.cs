using Microsoft.EntityFrameworkCore;
using TSMA.Meal.Web.Database;

namespace TSMA.Meal.Web.Shared.Repositories.OffDays;

public class OffDaysRepository(AppDbContext context) : IOffDaysRepository
{
    public async Task<List<string>> GetOffDaysAsync(CancellationToken cancellationToken)
    {
        return await context.MealOffDay.Select(o => o.Date).AsNoTracking().ToListAsync(cancellationToken);
    }
}