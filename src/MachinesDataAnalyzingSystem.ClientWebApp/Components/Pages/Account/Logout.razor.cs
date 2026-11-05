using MachinesDataAnalyzingSystem.ClientWebApp.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Components.Pages.Account;

public partial class Logout(AuthenticationStateProvider authenticationStateProvider, NavigationManager navigationManager)
{
    protected override async Task OnInitializedAsync()
    {
        await ((CustomAuthenticationStateProvider)authenticationStateProvider).Logout();
        navigationManager.NavigateTo("/Login");
    }
}