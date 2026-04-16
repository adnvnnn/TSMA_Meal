using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TSMA_Meal.ConfigModels;
using TSMA_Meal.Shared;
using TSMA_Meal.Tools;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TSMA_Meal.Features.Account;
public record LoginCommand(string PhoneNumber, string Code) : IRequest<bool>;

internal class LoginHandler(IPersonnelRepository repository, IOtpService service, IHttpContextAccessor httpContextAccessor) : IRequestHandler<LoginCommand, bool>
{
    public async Task<bool> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Code)) return false;

        var checkOtpAttempts = service.GetCheckAttemptCount(request.PhoneNumber);

        if (SecuritySettingsConfigModel.MaxCheckOtpAttemptsPerSend <= checkOtpAttempts) return false;


        var code = service.GetOtp(request.PhoneNumber);

        if (code != Encoder.Encode(request.Code))
        {
            service.IncreaseCheckAttempt(request.PhoneNumber);

            return false;
        }

        service.DeleteCheckAttempt(request.PhoneNumber);

        var personnel = await repository.GetPersonnelByPhoneAsync(request.PhoneNumber, cancellationToken);

        if (personnel == null) throw new NotPermittedException();


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

        await httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);

        return true;
    }
}


public static class LoginEndpoint
{
    public static void MapLogin(this IEndpointRouteBuilder app)
    {
        //app.MapPost("/check-login", async ([FromBody] LoginCommand request, ISender mediator) 
        //        => await mediator.Send(request))
        //    .WithTags("Authentication")
        //    .AllowAnonymous()
        //    .DisableAntiforgery();

        app.MapPost("/check-login", async (HttpContext httpContext, [FromBody] LoginCommand request, ISender mediator) =>
            {
                var result = await mediator.Send(request);

                if (!result)
                    return Results.BadRequest(new { error = "Invalid credentials" });

                // دستی redirect کنید
                httpContext.Response.Redirect("/");
                return Results.Ok();
            })
            .WithTags("Authentication")
            .AllowAnonymous()
            .DisableAntiforgery();
    }
}

