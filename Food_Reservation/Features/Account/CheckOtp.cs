using MediatR;
using TSMA_Meal.Shared;
using TSMA_Meal.Tools;

namespace TSMA_Meal.Features.Account;
public record CheckOtpRequest(string PhoneNumber, string Code):IRequest<bool>;

internal class CheckOtpHandler(IOtpRepository repository) : IRequestHandler<CheckOtpRequest,bool>
{
    public async Task<bool> Handle(CheckOtpRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Code)) return false;

        var code = repository.Get(request.PhoneNumber);

        if (code != Md5Hash.Hash(request.Code))
        {
            //TODO: Check failed count

            return false;
        }

        repository.Delete(request.PhoneNumber);

        return true ;
    }
}

