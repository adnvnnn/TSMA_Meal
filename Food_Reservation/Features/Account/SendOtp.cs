using MediatR;

namespace Food_Reservation.Features.Account;
public record SendOtpRequest (string PhoneNumber) : IRequest;
