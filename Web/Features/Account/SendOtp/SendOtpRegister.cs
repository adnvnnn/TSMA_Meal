using TSMA.Meal.Web.Features.Account.SendOtp.Sms;

namespace TSMA.Meal.Web.Features.Account.SendOtp;

public static class SendOtpRegister
{
    public static void RegisterSendOtp(this IServiceCollection services)
    {
        services.AddScoped<ISmsService, SmsService>();
    }
}