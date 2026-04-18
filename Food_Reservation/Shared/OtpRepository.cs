using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace TSMA_Meal.Shared;


public interface IOtpRepository
{
    void Set(string phone, string code, TimeSpan? window = null);

    string? Get(string phone);

    void Delete(string phone);
}

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
