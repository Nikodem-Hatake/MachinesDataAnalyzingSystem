using MachinesDataAnalyzingSystem.ClientWebApp.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared;
using MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Net;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Components.Controls.MachineDetails;

public partial class AggregatedMachineDataFormComponent(ISnackbar snackbar, IWebAPIService webAPIService, NavigationManager navigationManager)
{
    public AggregatedMachineDataRequest AggregatedMachineDataRequest { get; set; } = new();
    [Parameter]
    public Guid MachineId { get; set; }
    [Parameter]
    public string MachineName { get; set; }
    [Parameter]
    public EventCallback<List<AggregatedMachineDataDTO>> OnValidSubmitEventCallback { get; set; }

    public async Task OnValidSubmit(EditContext context)
    {
        try
        {
            var response = await webAPIService.SendAsync(HttpMethod.Get, $"/api/AggregatedMachineDatas/{MachineId}/{
                AggregatedMachineDataRequest.StartDate.Value.Add(AggregatedMachineDataRequest.StartTime.Value).ToString
                (GlobalConstants.DATE_TIME_FORMAT_TO_SEND_IN_HTTP_REQUEST)}/{((int)AggregatedMachineDataRequest.HowLong)}/{
                AggregatedMachineDataRequest.HowMany}");
            response.EnsureSuccessStatusCode();
            await OnValidSubmitEventCallback.InvokeAsync(await response.Content.ReadFromJsonAsync<List<AggregatedMachineDataDTO>>());
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
    }
}