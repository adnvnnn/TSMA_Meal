using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Localization;
using TSMA_Meal.Shared.Exceptions;

namespace TSMA_Meal.Shared.Middleware;

public class ExceptionHandler(IHostEnvironment env,IStringLocalizer<SharedResource> localizer) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {

        var message = localizer["ServerError"].Value;


        if(exception is BaseException)
        {
            httpContext.Response.StatusCode = 400;

            message = exception.Message;
        }
        else
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            if (env.IsDevelopment())
            {
                message = exception.Message;
            }
        }

        await httpContext.Response.WriteAsJsonAsync(message, cancellationToken);

        return true;
    }
}