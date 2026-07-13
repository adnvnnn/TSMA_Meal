namespace TSMA.Meal.Web.Features.Account.SendOtp.Sms;

public interface ISmsService
{
    Task SendOtpAsync(string phone, string code);
}