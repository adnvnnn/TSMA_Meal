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

var demoMode = builder.Configuration.GetValue<bool>("DemoMode");

builder.Configuration.RegisterLog(builder.Environment.EnvironmentName);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.RegisterLocalization();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddHttpContextAccessor();

builder.Services.RegisterAuthentication();

builder.Services.AddAuthorization();

builder.Services.RegisterDataBase(demoMode);

builder.Services.AddMemoryCache();

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(BaseUrlConfigModel.BaseUrl)});

builder.Configuration.RegisterConfigModels();

builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services.RegisterSendOtp(demoMode);

builder.Services.RegisterMealsList();

builder.Services.RegisterReserveFood();

builder.Services.RegisterCancelReserve();

builder.Services.RegisterShared();


var app = builder.Build();

if (demoMode)
{
    app.SeedDemoData();

    app.Logger.LogWarning(
        "DEMO MODE: in-memory data and console OTP. Sign in with {Phone}; the code is printed in this console.",
        DemoDataSeeder.DemoPhone);
}

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