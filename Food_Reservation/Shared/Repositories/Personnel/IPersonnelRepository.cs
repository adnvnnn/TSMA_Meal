namespace TSMA_Meal.Shared.Repositories.Personnel;

public interface IPersonnelRepository
{
    Task<bool> ExistByPhoneAsync(string phone, CancellationToken cancellationToken);
    Task<Database.Entities.Personnel?> GetPersonnelAsync(int personnelCode, CancellationToken cancellationToken);
    Task<Database.Entities.Personnel?> GetPersonnelByPhoneAsync(string phone, CancellationToken cancellationToken);
    Task<bool> PersonnelHasAccess(int personnelCode);
    Task<bool> PersonnelHasAccessByPhone(string phone, CancellationToken cancellationToken);
    Task<bool> IsActive(int personnelCode, CancellationToken cancellationToken);
}
