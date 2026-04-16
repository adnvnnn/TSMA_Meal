namespace TSMA_Meal.Shared;

public interface IOtpService
{
    void SetOtp(string phone, string code, TimeSpan expiredTime);
    string? GetOtp(string phone);
    bool ExistOtp(string phone);


    void IncreaseSendAttempt(string phone, TimeSpan banTime);

    int GetSendAttemptCount(string phone);


    void IncreaseCheckAttempt(string phone);

    int GetCheckAttemptCount(string phone);

    void DeleteCheckAttempt(string phone);


}

internal class OtpService(IOtpRepository repository) : IOtpService
{
    public void SetOtp(string phone, string code, TimeSpan expiredTime)
    {
        repository.Set(phone, code, expiredTime);
    }

    public string? GetOtp(string phone)
    {
        return repository.Get(phone);
    }
    public bool ExistOtp(string phone)
    {
        return repository.Get(phone) != null;
    }


    public void IncreaseSendAttempt(string phone, TimeSpan banTime)
    {
        var current = GetSendAttemptCount(phone);

        repository.Set($"OtpAttempt:{phone}", (current + 1).ToString(), banTime);
    }

    public int GetSendAttemptCount(string phone)
    {
        var attempts = repository.Get($"OtpAttempt:{phone}");

        return Convert.ToInt32(attempts);
    }



    public void IncreaseCheckAttempt(string phone)
    {
        var current = GetSendAttemptCount(phone);

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
