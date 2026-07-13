using Microsoft.Extensions.Options;
using System.Reflection;
using TSMA.Meal.Web.Components;
using TSMA.Meal.Web.ConfigModels;
using TSMA.Meal.Web.Features.Account.Login;
using TSMA.Meal.Web.Features.Account.Logout;
using TSMA.Meal.Web.Features.Account.SendOtp;
using TSMA.Meal.Web.Features.Meals.CancelReserve;
using TSMA.Meal.Web.Features.Meals.MealsList;
using TSMA.Meal.Web.Features.Meals.ReserveMeal;
using TSMA.Meal.Web.Setup;
using TSMA.Meal.Web.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.RegisterLog(builder.Environment.EnvironmentName);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.RegisterLocalization();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddHttpContextAccessor();

builder.Services.RegisterAuthentication();

builder.Services.AddAuthorization();

builder.Services.RegisterDataBase();

builder.Services.AddMemoryCache();

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(BaseUrlConfigModel.BaseUrl)});

builder.Configuration.RegisterConfigModels();

builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services.RegisterSendOtp();

builder.Services.RegisterMealsList();

builder.Services.RegisterReserveFood();

builder.Services.RegisterCancelReserve();

builder.Services.RegisterShared();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.UseAntiforgery();

app.MapLogin();

app.MapLogout();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();