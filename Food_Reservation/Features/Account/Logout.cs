using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TSMA_Meal.ConfigModels;

namespace TSMA_Meal.Features.Account;


public static class LogoutEndpoint
{
    public static void MapLogout(this IEndpointRouteBuilder app)
    {
        app.MapPost(SecuritySettingsConfigModel.LogoutPath, async ([FromBody] LoginCommand request, ISender mediator)
                => await mediator.Send(new LoginCommand(request.PhoneNumber, request.Code)))
            .WithTags("Logout");
    }
}

