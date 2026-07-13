namespace TSMA.Meal.Web.Shared.Repositories.OffDays;

public interface IOffDaysRepository
{
    Task<List<string>> GetOffDaysAsync(CancellationToken cancellationToken);

}