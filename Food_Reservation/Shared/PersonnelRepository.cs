using Food_Reservation.Database;
using Food_Reservation.Entities;
using Microsoft.EntityFrameworkCore;

namespace Food_Reservation.Shared;

public interface IPersonnelRepository
{
    Task<Personnel?> GetPersonnelAsync(int personnelCode, CancellationToken cancellationToken);
    Task<bool> PersonnelHasAccess(int personnelCode);
    Task<bool> IsActive(int personnelCode, CancellationToken cancellationToken);


}

public class PersonnelRepository(AppDbContext context) : IPersonnelRepository
{
    public async Task<Personnel?> GetPersonnelAsync(int personnelCode, CancellationToken cancellationToken)
    {
        return await context.Personnel.IgnoreQueryFilters().AsNoTracking()
            .FirstOrDefaultAsync(p => p.PersonnelCode == personnelCode, cancellationToken);
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