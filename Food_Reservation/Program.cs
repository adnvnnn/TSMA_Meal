using Microsoft.Extensions.Options;
using System.Reflection;
using TSMA_Meal.Components;
using TSMA_Meal.Features.Account;
using TSMA_Meal.Features.Reserves;
using TSMA_Meal.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.RegisterLog(builder.Environment.EnvironmentName);



builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.RegisterLocalization();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));


builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();


builder.Services.RegisterAuthentication();

builder.Services.RegisterAuthorization();

builder.Services.RegisterDataBase();

builder.Services.AddMemoryCache();

// Program.cs
builder.Services.AddHttpClient();

builder.Configuration.RegisterConfigModels();


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

app.MapControllers();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();

