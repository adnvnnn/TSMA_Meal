using Microsoft.Extensions.Caching.Memory;

namespace TSMA.Meal.Web.Shared.Repositories.Otp;

internal class OtpRepository(IMemoryCache cache) : IOtpRepository
{
    public void Set(string phone, string code, TimeSpan? window = null)
    {
        if (window != null)
        {
            cache.Set(phone, code, (TimeSpan)window);
        }
        else
        {
            cache.Set(phone, code);
        }
    }

    public string? Get(string phone)
    {
        cache.TryGetValue(phone, out string? hashedCode);
        return hashedCode;
    }

    public void Delete(string phone)
    {
        cache.Remove(phone);
    }
}