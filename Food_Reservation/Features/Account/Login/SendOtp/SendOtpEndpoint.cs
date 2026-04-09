using Food_Reservation.Services.Interfaces;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;

namespace Food_Reservation.Features.Account.Login.SendOtp;

public static class SendOtpEndpoint
{
    public static async Task<IResult> Handle(SendOtpRequest request, ISmsService sender)
    {
        var otp = new Random().Next(1000, 9999).ToString();

        return Results.Ok();
    }
}