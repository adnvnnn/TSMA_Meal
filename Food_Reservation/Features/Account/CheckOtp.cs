using MediatR;
using TSMA_Meal.ConfigModels;
using TSMA_Meal.Shared;
using TSMA_Meal.Tools;

namespace TSMA_Meal.Features.Account;
public record CheckOtpRequest(string PhoneNumber, string Code):IRequest<bool>;

internal class CheckOtpHandler(IOtpService service) : IRequestHandler<CheckOtpRequest,bool>
{
    public async Task<bool> Handle(CheckOtpRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Code)) return false;

        var code = service.GetOtp(request.PhoneNumber);

        var checkOtpAttempts = service.GetCheckAttemptCount(request.PhoneNumber);

        if (SecuritySettingsConfigModel.MaxCheckOtpAttemptsPerSend <= checkOtpAttempts) return false;

        if (code != Md5Hash.Hash(request.Code))
        {
            service.SetCheckAttempt(request.PhoneNumber);

            return false;
        }

        service.DeleteCheckAttempt(request.PhoneNumber);

        return true ;
    }
}

