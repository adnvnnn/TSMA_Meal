namespace TSMA.Meal.Web.Shared.Services.Otp;

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
