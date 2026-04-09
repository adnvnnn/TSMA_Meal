using MediatR;

namespace Food_Reservation.Features.Account;
public record LoginRequest (string PhoneNumber,int Code):IRequest;
