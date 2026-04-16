using Microsoft.EntityFrameworkCore;
using System.Threading;
using TSMA_Meal.Database;
using TSMA_Meal.Database.Entities;

namespace TSMA_Meal.Shared;

public interface IPersonnelRepository
{
    Task<bool> ExistByPhoneAsync(string phone, CancellationToken cancellationToken);
    Task<Personnel?> GetPersonnelAsync(int personnelCode, CancellationToken cancellationToken);
    Task<Personnel?> GetPersonnelByPhoneAsync(string phone, CancellationToken cancellationToken);
    Task<bool> PersonnelHasAccess(int personnelCode);
    Task<bool> PersonnelHasAccessByPhone(string phone, CancellationToken cancellationToken);
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

    public async Task<Personnel?> GetPersonnelAsync(int personnelCode, CancellationToken cancellationToken)
    {
        return await context.Personnel.AsNoTracking()
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

    public async Task<bool> PersonnelHasAccessByPhone(string phone, CancellationToken cancellationToken)
    {
        return await (from pi in context.PersonnelInfo
            join p in context.Personnel on pi.Id equals p.PersonnelCode
            join pa in context.PersonnelAccess on p.PersonnelCode equals pa.PersonnelCode
            where pi.Phone == phone
            select pa).AnyAsync(cancellationToken);
    }
    public async Task<bool> IsActive(int personnelCode, CancellationToken cancellationToken)
    {
        return await context.Personnel.IgnoreQueryFilters()
            .AnyAsync(p => p.PersonnelCode == personnelCode && p.Active, cancellationToken);

    }
}