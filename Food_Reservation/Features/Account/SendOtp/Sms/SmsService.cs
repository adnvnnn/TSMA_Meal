using System.Text;
using System.Xml.Linq;
using TSMA_Meal.ConfigModels;
using TSMA_Meal.Features.Account.SendOtp.Exceptions;

namespace TSMA_Meal.Features.Account.SendOtp.Sms;

internal class SmsService : ISmsService
{
    public async Task SendOtpAsync(string phone, string code)
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
