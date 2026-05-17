namespace TSMA_Meal.Shared.Repositories.OffDays;

public interface IOffDaysRepository
{
    Task<List<string>> GetOffDaysAsync(CancellationToken cancellationToken);

}