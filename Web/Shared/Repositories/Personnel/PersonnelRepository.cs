using Microsoft.EntityFrameworkCore;
using TSMA.Meal.Web.Database;

namespace TSMA.Meal.Web.Shared.Repositories.Personnel;


public class PersonnelRepository(AppDbContext context) : IPersonnelRepository
{
    public async Task<bool> ExistByPhoneAsync(string phone, CancellationToken cancellationToken)
    {
        return await context.Personnel
            .AsNoTracking()
            .AnyAsync(p => context.PersonnelInfo.Count(pi => pi.Phone == phone && pi.Id == p.PersonnelCode) > 0,
                cancellationToken: cancellationToken);
    }

    public async Task<Database.Entities.Personnel?> GetPersonnelAsync(int personnelCode, CancellationToken cancellationToken)
    {
        return await context.Personnel.AsNoTracking()
            .FirstOrDefaultAsync(p => p.PersonnelCode == personnelCode, cancellationToken);
    }

    public async Task<Database.Entities.Personnel?> GetPersonnelByPhoneAsync(string phone, CancellationToken cancellationToken)
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