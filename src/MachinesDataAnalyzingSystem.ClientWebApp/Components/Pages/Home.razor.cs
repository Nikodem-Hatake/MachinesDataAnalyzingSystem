using MachinesDataAnalyzingSystem.ClientWebApp.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared;
using MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;
using MachinesDataAnalyzingSystem.Shared.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Components.Pages;

public partial class Home(IWebAPIService webAPIHttpClient, ISnackbar snackbar, NavigationManager navigationManager)
{
    private List<AggregatedMachineDatasForMachineDTO> _aggregatedMachineDatasForMachineDTOS;
    private bool _isFirstRender = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _isFirstRender = firstRender;
        if(!firstRender)
        {
            return;
        }

        try
        {
            var response = await webAPIHttpClient.SendAsync(HttpMethod.Get, $"/api/aggregatedMachineDatas/{DateTime
                .UtcNow.AddMinutes(-60).ToString(GlobalConstants.DATE_TIME_FORMAT_TO_SEND_IN_HTTP_REQUEST)}/{((int)
                HowLongIsAggregatedMachineData.TenMinutes)}/{6}");
            response.EnsureSuccessStatusCode();
            _aggregatedMachineDatasForMachineDTOS = await response.Content.ReadFromJsonAsync<List<AggregatedMachineDatasForMachineDTO>>();
        }
        catch(HttpRequestException e)
        {
            if(e.StatusCode is null)
            {
                snackbar.Add("Server is unavailable. Please try later.", Severity.Error);
            }
            else if(e.StatusCode == HttpStatusCode.Unauthorized)
            {
                navigationManager.NavigateTo("/Logout");
            }
            else
            {
                snackbar.Add($"HTTP error with status code: {e.StatusCode.ToString()}", Severity.Error);
            }
        }
        StateHasChanged();
    }
}