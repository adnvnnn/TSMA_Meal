using TSMA_Meal.Features.Account.SendOtp.Sms;

namespace TSMA_Meal.Features.Account.SendOtp;

public static class SendOtpRegister
{
    public static void RegisterSendOtp(this IServiceCollection services)
    {
        services.AddScoped<ISmsService, SmsService>();
    }
}