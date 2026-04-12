//using System.Security.Claims;
//using MediatR;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Components.Authorization;
//using TSMA_Meal.Features.Account;

//namespace TSMA_Meal.Authentication
//{
//    // CustomAuthStateProvider.cs
//    public class CustomAuthStateProvider(IHttpContextAccessor contextAccessor) : AuthenticationStateProvider
//    {
//        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//        {
//            var httpContext = contextAccessor.HttpContext;

//            if (httpContext?.User?.Identity?.IsAuthenticated == true)
//            {
//                return new AuthenticationState(httpContext.User);
//            }

//            // Anonymous user
//            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
//            return new AuthenticationState(anonymousUser);
//        }

//        public async Task LoginAsync(string personnelCode,string name)
//        {

//            var claims = new List<Claim>
//            {
//                new(ClaimTypes.Name, name),
//                new(ClaimTypes.NameIdentifier, personnelCode)
//            };

//            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//            var user = new ClaimsPrincipal(identity);

//            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
//        }
//    }
//}
