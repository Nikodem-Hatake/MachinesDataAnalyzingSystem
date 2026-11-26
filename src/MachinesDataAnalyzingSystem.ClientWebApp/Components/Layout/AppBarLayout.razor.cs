using Microsoft.AspNetCore.Components;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Components.Layout;

public partial class AppBarLayout
{
    [Parameter]
    public bool IsDarkMode { get; set; }
    private bool _isMenuOpened = false;
    [Parameter]
    public EventCallback<bool> OnThemeModeChanged { get; set; }

    private void ToggleMenu()
    {
        _isMenuOpened = !_isMenuOpened;
    }
}