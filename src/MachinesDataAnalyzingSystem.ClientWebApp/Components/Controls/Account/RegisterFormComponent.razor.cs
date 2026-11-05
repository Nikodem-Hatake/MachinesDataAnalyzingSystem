using MachinesDataAnalyzingSystem.ClientWebApp.Authentication;
using MachinesDataAnalyzingSystem.ClientWebApp.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.Accounts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Text.Json;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Components.Controls.Account;

public partial class RegisterFormComponent(IWebAPIService webAPIHttpClient, ISnackbar snackbar,
    AuthenticationStateProvider authenticationStateProvider, NavigationManager navigationManager)
{
    private RegisterRequest RegisterRequest { get; set; } = new();

    private async Task OnValidSubmit(EditContext context)
    {
        try
        {
            var response = await webAPIHttpClient.SendAsyncWithoutAuthorizationHeader
                (HttpMethod.Post, "/api/accounts/register", RegisterRequest);
            response.EnsureSuccessStatusCode();
            var authenticationResponse = JsonSerializer.Deserialize<AuthenticationResponse>(await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            await ((CustomAuthenticationStateProvider)authenticationStateProvider).SaveAuthenticationResponseData(authenticationResponse);
            navigationManager.NavigateTo("/");
        }
        catch(HttpRequestException e)
        {
            if(e.StatusCode is null)
            {
                snackbar.Add("Server is unavailable. Please try later.", Severity.Error);
            }
            else
            {
                snackbar.Add($"HTTP error with status code: {e.StatusCode.ToString()}", Severity.Error);
            }
        }
    }
}