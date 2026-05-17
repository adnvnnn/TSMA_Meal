using Microsoft.Extensions.Options;
using System.Reflection;
using TSMA_Meal.Components;
using TSMA_Meal.ConfigModels;
using TSMA_Meal.Features.Account;
using TSMA_Meal.Features.Account.Login;
using TSMA_Meal.Features.Account.Logout;
using TSMA_Meal.Features.Account.SendOtp;
using TSMA_Meal.Features.Meals.CancelReserve;
using TSMA_Meal.Features.Meals.MealsList;
using TSMA_Meal.Features.Meals.ReserveMeal;
using TSMA_Meal.Setup;
using TSMA_Meal.Shared.Middleware;

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