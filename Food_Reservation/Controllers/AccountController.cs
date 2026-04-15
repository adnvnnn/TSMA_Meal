using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TSMA_Meal.ConfigModels;
using TSMA_Meal.Features.Account;

namespace TSMA_Meal.Controllers;

public class AccountController(IMediator mediator) : Controller
{

    [HttpPost("check-otp")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> LoginOtp([FromForm] LoginOtpRequest request)
    {
        var otpResult = await mediator.Send(new CheckOtpRequest(request.PhoneNumber, request.Code));
        if (!otpResult) return LocalRedirect("/login?error=invalid");

        var personnel = await mediator.Send(new PersonnelByPhoneQuery(request.PhoneNumber));

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, personnel.FullName),
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
    //[HttpPost("check-otp")]
    //[IgnoreAntiforgeryToken]
    //public async Task<IActionResult> LoginOtp([FromBody] LoginOtpRequest request)
    //{

    //    var optResult = await mediator.Send(new CheckOtpRequest(request.PhoneNumber, request.Code));

    //    if (!optResult) return Unauthorized();

    //    var personnel = await mediator.Send(new PersonnelByPhoneQuery(request.PhoneNumber));

    //    var claims = new List<Claim>
    //    {
    //        new(ClaimTypes.Name, personnel.FullName),
    //        new(ClaimTypes.NameIdentifier, personnel.PersonnelCode.ToString())
    //    };

    //    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    //    var principal = new ClaimsPrincipal(identity);



    //    var authProperties = new AuthenticationProperties
    //    {
    //        IsPersistent = true,
    //        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(SecuritySettingsConfigModel.LoginDays),
    //        AllowRefresh = true
    //    };

    //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);


    //    return Ok();
    //}


    [HttpPost("logout")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return LocalRedirect("/login");
    }


    public class LoginOtpRequest
    {
        public string Code { get; set; }
        public string PhoneNumber { get; set; }
    }
}