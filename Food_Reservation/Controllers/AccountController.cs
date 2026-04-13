using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TSMA_Meal.Features.Account;

namespace TSMA_Meal.Controllers;

[Route("account")]
public class AccountController() : Controller
{


    [HttpPost("login-otp")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginOtp([FromBody] LoginOtpRequest request)
    {

        //var optResult = await Mediator.Send(new CheckOtpRequest(PhoneNumber, _code));

        //if (!optResult) return;

        //var personnel = await Mediator.Send(new PersonnelByPhoneQuery(PhoneNumber));

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, request.Name),
            new(ClaimTypes.NameIdentifier, request.PersonnelCode)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);


            
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
            AllowRefresh = true
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);


        return LocalRedirect("/");
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return LocalRedirect("/");
    }



    public class LoginOtpRequest
    {
        public string Name { get; set; }
        public string PersonnelCode { get; set; }
    }
}