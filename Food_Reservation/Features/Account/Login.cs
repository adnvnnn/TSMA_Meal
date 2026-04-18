using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TSMA_Meal.ConfigModels;
using TSMA_Meal.Shared;
using TSMA_Meal.Tools;

namespace TSMA_Meal.Features.Account;

public class AccountController(IPersonnelRepository repository, IOtpService service) : Controller
{
    [HttpPost("check-otp")]
    [IgnoreAntiforgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> LoginOtp([FromBody] LoginDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Code)) return Ok();

        var checkOtpAttempts = service.GetCheckAttemptCount(request.PhoneNumber);

        if (SecuritySettingsConfigModel.MaxCheckOtpAttemptsPerSend <= checkOtpAttempts) return Ok();


        var code = service.GetOtp(request.PhoneNumber);

        if (code != Encoder.Encode(request.Code))
        {
            service.IncreaseCheckAttempt(request.PhoneNumber);

            throw new InvalidCodeException();
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

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

        return LocalRedirect("/");
    }

}

public record LoginDto(string PhoneNumber, string Code);

public class InvalidCodeException() : Exception("کد وارد شده صحیح نمیباشد.");