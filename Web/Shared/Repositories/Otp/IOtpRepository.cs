namespace TSMA.Meal.Web.Shared.Repositories.Otp;


public interface IOtpRepository
{
    void Set(string phone, string code, TimeSpan? window = null);

    string? Get(string phone);

    void Delete(string phone);
}