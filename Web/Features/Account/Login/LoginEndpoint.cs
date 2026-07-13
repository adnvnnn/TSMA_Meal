using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TSMA.Meal.Web.ConfigModels;
using TSMA.Meal.Web.Shared.Exceptions;
using TSMA.Meal.Web.Shared.Repositories.Personnel;
using TSMA.Meal.Web.Shared.Services.Otp;
using TSMA.Meal.Web.Tools;

namespace TSMA.Meal.Web.Features.Account.Login;

public static class LoginEndpoint
{
    public static void MapLogin(this IEndpointRouteBuilder app)
    {
        app.MapPost("/check-otp", async (
            [FromForm] LoginDto request,
            IPersonnelRepository repository,
            IOtpService service,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            try
            {
                if (string.IsNullOrEmpty(request.Code))
                    return Results.BadRequest();

                var checkOtpAttempts = service.GetCheckAttemptCount(request.PhoneNumber);

                if (SecuritySettingsConfigModel.MaxCheckOtpAttemptsPerSend <= checkOtpAttempts)
                    return Results.BadRequest();

                var code = service.GetOtp(request.PhoneNumber);

                if (code != Encoder.Encode(request.Code))
                {
                    service.IncreaseCheckAttempt(request.PhoneNumber);
                    throw new InvalidCodeException();
                }

                service.DeleteCheckAttempt(request.PhoneNumber);

                var personnel = await repository.GetPersonnelByPhoneAsync(
                    request.PhoneNumber,
                    cancellationToken);

                if (personnel == null)
                    throw new NotPermittedException();

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, personnel.Name + " " + personnel.LastName),
                    new(ClaimTypes.NameIdentifier, personnel.PersonnelCode.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(SecuritySettingsConfigModel.LoginDays),
                    AllowRefresh = true
                };

                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties);

                return Results.LocalRedirect("/");
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        })
        .AllowAnonymous()
        .DisableAntiforgery();
    }
}
