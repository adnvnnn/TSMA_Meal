using MediatR;
using TSMA_Meal.Shared;

namespace TSMA_Meal.Features.Account;
public record CheckOtpRequest(string PhoneNumber, string Code):IRequest<bool>;

internal class CheckOtpHandler(IOtpRepository repository) : IRequestHandler<CheckOtpRequest,bool>
{
    public async Task<bool> Handle(CheckOtpRequest request, CancellationToken cancellationToken)
    {
        var code = await repository.GetAsync(request.PhoneNumber);

        if (code != request.Code)
        {
            //TODO: Check failed count
        }

        return code == request.Code;
    }
}

