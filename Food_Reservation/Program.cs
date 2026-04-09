using Food_Reservation.Components;
using Food_Reservation.Features.Restaurants;
using Food_Reservation.Setup;
using Microsoft.Extensions.Options;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.RegisterLog(builder.Environment.EnvironmentName);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.RegisterLocalization();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.RegisterAuthentication();

builder.Services.RegisterAuthorization();

builder.Services.RegisterDataBase();

builder.Configuration.RegisterConfigModels();


builder.Services.RegisterFoodList();

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

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
