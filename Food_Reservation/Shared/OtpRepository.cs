using Microsoft.Extensions.Caching.Memory;

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
//internal class OtpRepository
//{
//    private readonly IDatabase _db;
//    private readonly string _prefix = "otp:";

//    public OtpRepository(IConnectionMultiplexer redis)
//    {
//        _db = redis.GetDatabase();
//    }

//    public void Set(string phone, string code, TimeSpan? window = null)
//    {
//        var key = _prefix + phone;

//        if (window != null)
//        {
//            _db.StringSet(key, code, (TimeSpan)window);
//        }
//        else
//        {
//            _db.StringSet(key, code);
//        }
//    }

//    public string? Get(string phone)
//    {
//        var key = _prefix + phone;
//        return _db.StringGet(key);
//    }

//    public void Delete(string phone)
//    {
//        var key = _prefix + phone;
//        _db.KeyDelete(key);
//    }

//    // Bonus: Check if exists
//    public bool Exists(string phone) => _db.KeyExists(_prefix + phone);
//}