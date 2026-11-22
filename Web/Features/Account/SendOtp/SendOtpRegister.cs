using TSMA.Meal.Web.Features.Account.SendOtp.Sms;

namespace TSMA.Meal.Web.Features.Account.SendOtp;

public static class SendOtpRegister
{
    public static void RegisterSendOtp(this IServiceCollection services, bool demoMode = false)
    {
        if (demoMode)
        {
            services.AddScoped<ISmsService, ConsoleSmsService>();

            return;
        }

        services.AddScoped<ISmsService, SmsService>();
    }
}
