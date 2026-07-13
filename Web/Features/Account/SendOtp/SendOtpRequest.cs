using MediatR;

namespace TSMA.Meal.Web.Features.Account.SendOtp;

public record SendOtpRequest(string PhoneNumber) : IRequest;
