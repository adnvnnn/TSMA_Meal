using Microsoft.Extensions.Caching.Memory;

namespace TSMA_Meal.Shared;

public interface IOtpService
{
    void SetOtp(string phone, string code, TimeSpan expiredTime);
    string? GetOtp(string phone);


    void SetAttempt(string phone, TimeSpan banTime);

    int GetAttemptCount(string phone);


    void SetCheckAttempt(string phone);

    int GetCheckAttemptCount(string phone);

    void DeleteCheckAttempt(string phone);

}

internal class OtpService(IOtpRepository repository) : IOtpService
{
    public void SetOtp(string phone, string code, TimeSpan expiredTime)
    {
        repository.Set(phone,code, expiredTime);
    }

    public string? GetOtp(string phone)
    {
        return repository.Get(phone);
    }



    public void SetAttempt(string phone, TimeSpan banTime)
    {
        var current = GetAttemptCount(phone);

        repository.Set($"OtpAttempt:{phone}", (current + 1).ToString(), banTime);
    }

    public int GetAttemptCount(string phone)
    {
        var attempts = repository.Get($"OtpAttempt:{phone}");

        return Convert.ToInt32(attempts);
    }



    public void SetCheckAttempt(string phone)
    {
        var current = GetAttemptCount(phone);

        repository.Set($"CheckAttempt:{phone}", (current + 1).ToString());
    }

    public int GetCheckAttemptCount(string phone)
    {
        var attempts = repository.Get($"CheckAttempt:{phone}");

        return Convert.ToInt32(attempts);
    }

    public void DeleteCheckAttempt(string phone)
    {
        repository.Delete($"CheckAttempt:{phone}");
    }
}

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