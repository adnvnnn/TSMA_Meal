using MediatR;
using System.Text;
using System.Xml.Linq;
using TSMA_Meal.ConfigModels;
using TSMA_Meal.Shared;
using Encoder = TSMA_Meal.Tools.Encoder;

namespace TSMA_Meal.Features.Account;
public record SendOtpRequest (string PhoneNumber) : IRequest;

internal class SendOtpHandler(IPersonnelRepository repository,ISmsService smsService,IPersonnelRepository personnelRepository,IOtpService otpRepository) : IRequestHandler<SendOtpRequest>
{
    public async Task Handle(SendOtpRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.PhoneNumber) || !request.PhoneNumber.StartsWith("09")) throw new InvalidNumberException();

        //IF PERSONNEL EXIST
        var exist = await repository.ExistByPhoneAsync(request.PhoneNumber, cancellationToken);

        if (!exist) throw new PersonnelNotExistException();

        if (!await personnelRepository.PersonnelHasAccessByPhone(request.PhoneNumber, cancellationToken)) throw new NotPermittedException();


        if (otpRepository.ExistOtp(request.PhoneNumber))
        {
            throw new TrySendOtpAfterMinutes(SecuritySettingsConfigModel.OtpExpiryMinutes);
        }


        var attemptCount = otpRepository.GetSendAttemptCount(request.PhoneNumber);

        if (attemptCount >= SecuritySettingsConfigModel.MaxOtpAttemptsPerDay) throw new TooManyOtpSentException();

        var code = new Random().Next(100000, 999999).ToString("D6");
         
        await smsService.SendOtpAsync(request.PhoneNumber, code);

        otpRepository.IncreaseSendAttempt(request.PhoneNumber,TimeSpan.FromHours(SecuritySettingsConfigModel.BanDurationHours));

        var encoded = Encoder.Encode(code);

        otpRepository.SetOtp(request.PhoneNumber, encoded, TimeSpan.FromMinutes(SecuritySettingsConfigModel.OtpExpiryMinutes));

        otpRepository.DeleteCheckAttempt(request.PhoneNumber);
    }
}

public interface ISmsService
{
    Task SendOtpAsync(string phone,string code);
}
internal class SmsService : ISmsService
{
    public async Task SendOtpAsync(string phone,string code)
    {
        try
        {
            XNamespace soapEnv = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace ser = "http://ippanel.com/class/sms/wsdlservice";

            var parameter = new XElement("name", "Message");
            if (!string.IsNullOrEmpty(SmsConfigModel.ParameterName))
            {
                parameter = new XElement(SmsConfigModel.ParameterName, code);
            }

            var soapEnvelope = new XElement(soapEnv + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soapenv", soapEnv),
                new XAttribute(XNamespace.Xmlns + "ser", ser),
                new XElement(soapEnv + "Header"),
                new XElement(soapEnv + "Body",
                    new XElement(ser + "sendPatternSms",
                        new XElement("fromNum", SmsConfigModel.Sender),
                        new XElement("toNum", phone),
                        new XElement("user", SmsConfigModel.Username),
                        new XElement("pass", SmsConfigModel.Password),
                        new XElement("pattern_code", SmsConfigModel.VerificationPatternCode),
                        new XElement("input_data",
                            parameter
                        )
                    )
                )
            );

            using var client = new HttpClient();
            var request = new StringContent(soapEnvelope.ToString(), Encoding.UTF8, "text/xml");
            var response = await client.PostAsync("http://ippanel.com/class/sms/wsdlservice/server.php?wsdl", request);

            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) throw new SendOptFailedException();

        }
        catch
        {
            throw new SendOptFailedException();
        }
    }
}


internal class TrySendOtpAfterMinutes(int minutes) : Exception($"لطفا بعد از {minutes} دقیقه تلاش کنید.");
internal class InvalidNumberException() : Exception("شماره تلفن نامعتبر است.");
internal class SendOptFailedException() : Exception("خطا در ارسال کد. لطفاً مجدد تلاش کنید.");
internal class PersonnelNotExistException() : Exception("شما دسترسی ندارید. فقط پرسنل مجازند.");
internal class TooManyOtpSentException() : Exception("تعداد تلاش‌ها بیش از حد. ۲۴ ساعت دیگر امتحان کنید.");


public static class SendOtpRegister
{
    public static void RegisterSendOtp(this IServiceCollection services)
    {
        services.AddScoped<ISmsService, SmsService>();
    }
}