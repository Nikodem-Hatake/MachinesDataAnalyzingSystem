using Microsoft.AspNetCore.Components;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Components.Controls;

public partial class RedirectToComponent(NavigationManager navigationManager)
{
    [Parameter]
    public string Url { get; set; }

    protected override void OnInitialized()
    {
        navigationManager.NavigateTo(Url);
    }
}