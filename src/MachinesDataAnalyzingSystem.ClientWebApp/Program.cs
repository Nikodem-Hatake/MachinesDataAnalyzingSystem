using MachinesDataAnalyzingSystem.ClientWebApp.Authentication;
using MachinesDataAnalyzingSystem.ClientWebApp.Components;
using MachinesDataAnalyzingSystem.ClientWebApp.ServiceContracts;
using MachinesDataAnalyzingSystem.ClientWebApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddHttpClient("WebAPI", options =>
{
    options.BaseAddress = new(builder.Configuration.GetConnectionString("WebAPI"));
});
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IWebAPIService, WebAPIService>();
builder.Services.AddAuthentication();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

app.UseHsts();
app.UseStatusCodePagesWithReExecute("/Not-found");
app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
