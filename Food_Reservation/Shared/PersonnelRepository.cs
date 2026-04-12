using Microsoft.EntityFrameworkCore;
using TSMA_Meal.Database;
using TSMA_Meal.Entities;

namespace TSMA_Meal.Shared;

public interface IPersonnelRepository
{
    Task<bool> ExistByPhoneAsync(string phone, CancellationToken cancellationToken);
    Task<Personnel?> GetPersonnelIgnoreQueryAsync(int personnelCode, CancellationToken cancellationToken);
    Task<Personnel?> GetPersonnelByPhoneAsync(string phone, CancellationToken cancellationToken);
    Task<bool> PersonnelHasAccess(int personnelCode);
    Task<bool> IsActive(int personnelCode, CancellationToken cancellationToken);


}

public class PersonnelRepository(AppDbContext context) : IPersonnelRepository
{
    public async Task<bool> ExistByPhoneAsync(string phone, CancellationToken cancellationToken)
    {
        return await context.Personnel
            .AsNoTracking()
            .AnyAsync(p => context.PersonnelInfo.Count(pi => pi.Phone == phone && pi.Id == p.PersonnelCode) > 0,
                cancellationToken: cancellationToken);
    }

    public async Task<Personnel?> GetPersonnelIgnoreQueryAsync(int personnelCode, CancellationToken cancellationToken)
    {
        return await context.Personnel.IgnoreQueryFilters().AsNoTracking()
            .FirstOrDefaultAsync(p => p.PersonnelCode == personnelCode, cancellationToken);
    }

    public async Task<Personnel?> GetPersonnelByPhoneAsync(string phone, CancellationToken cancellationToken)
    {
        return await context.Personnel
            .AsNoTracking()
            .FirstOrDefaultAsync(p => context.PersonnelInfo.Count(pi => pi.Phone == phone && pi.Id == p.PersonnelCode) > 0,
                cancellationToken: cancellationToken);
    }

    public async Task<bool> PersonnelHasAccess(int personnelCode)
    {
        return await context.PersonnelAccess.AnyAsync(a => a.FoodReservation && a.PersonnelCode == personnelCode);
    }
    public async Task<bool> IsActive(int personnelCode, CancellationToken cancellationToken)
    {
        return await context.Personnel.IgnoreQueryFilters()
            .AnyAsync(p => p.PersonnelCode == personnelCode && p.Active, cancellationToken);

    }
}