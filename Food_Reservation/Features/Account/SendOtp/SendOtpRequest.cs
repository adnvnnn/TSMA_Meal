using MediatR;

namespace TSMA_Meal.Features.Account.SendOtp;

public record SendOtpRequest(string PhoneNumber) : IRequest;
