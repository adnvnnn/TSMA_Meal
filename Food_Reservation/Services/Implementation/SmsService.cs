using Food_Reservation.Services.Interfaces;
using System.Resources;
using System.Text;
using System.Xml.Linq;

namespace Food_Reservation.Services.Implementation;

//public class SmsService : ISmsService
//{
    //public async Task<bool> SendPattern(SendSmsPatternDto sendVm)
    //{
    //    XNamespace soapEnv = "http://schemas.xmlsoap.org/soap/envelope/";
    //    XNamespace ser = "http://ippanel.com/class/sms/wsdlservice";

    //    var parameter = new XElement("name", "Message");
    //    if (!string.IsNullOrEmpty(sendVm.ParameterName))
    //    {
    //        parameter = new XElement(sendVm.ParameterName, sendVm.ParameterValue);
    //    }

    //    var soapEnvelope = new XElement(soapEnv + "Envelope",
    //        new XAttribute(XNamespace.Xmlns + "soapenv", soapEnv),
    //        new XAttribute(XNamespace.Xmlns + "ser", ser),
    //        new XElement(soapEnv + "Header"),
    //        new XElement(soapEnv + "Body",
    //            new XElement(ser + "sendPatternSms",
    //                new XElement("fromNum", sendVm.Sender),
    //                new XElement("toNum", sendVm.Receiver),
    //                new XElement("user", sendVm.Username),
    //                new XElement("pass", sendVm.Password),
    //                new XElement("pattern_code", sendVm.PatternCode),
    //                new XElement("input_data",
    //                    parameter
    //                )
    //            )
    //        )
    //    );

    //    using var client = new HttpClient();
    //    var request = new StringContent(soapEnvelope.ToString(), Encoding.UTF8, "text/xml");
    //    var response = await client.PostAsync("http://ippanel.com/class/sms/wsdlservice/server.php?wsdl", request);

    //    // Read the response as a string
    //    string result = await response.Content.ReadAsStringAsync();

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        return new ResponseDto<bool>(Resources.Messages.SmsError);

    //    }

    //    return new ResponseDto<bool>(true);
    //}
//}