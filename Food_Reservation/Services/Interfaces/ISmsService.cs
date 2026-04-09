namespace Food_Reservation.Services.Interfaces;
public interface ISmsService
{
    Task<bool> SendOtp(string phone);
}
