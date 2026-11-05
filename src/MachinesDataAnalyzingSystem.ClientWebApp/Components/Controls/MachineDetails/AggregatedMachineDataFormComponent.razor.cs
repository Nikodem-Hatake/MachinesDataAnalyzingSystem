using MachinesDataAnalyzingSystem.ClientWebApp.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared;
using MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Text.Json;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Components.Controls.MachineDetails;

public partial class AggregatedMachineDataFormComponent(ISnackbar snackbar, IWebAPIService webAPIService)
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
            await OnValidSubmitEventCallback.InvokeAsync(JsonSerializer.Deserialize<List<AggregatedMachineDataDTO>>
                (await response.Content.ReadAsStringAsync(), new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                }));
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