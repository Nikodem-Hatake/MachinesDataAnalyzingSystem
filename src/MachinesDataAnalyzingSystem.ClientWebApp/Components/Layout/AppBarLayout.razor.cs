using Microsoft.AspNetCore.Components;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Components.Layout;

public partial class AppBarLayout
{
    [Parameter]
    public bool IsDarkMode { get; set; }
    [Parameter]
    public EventCallback<bool> OnThemeModeChanged { get; set; }
}