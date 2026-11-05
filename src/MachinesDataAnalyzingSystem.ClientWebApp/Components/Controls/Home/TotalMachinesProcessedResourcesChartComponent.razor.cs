using MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;
using Microsoft.AspNetCore.Components;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Components.Controls.Home;

public partial class TotalMachinesProcessedResourcesChartComponent
{
    [Parameter]
    public List<AggregatedMachineDatasForMachineDTO> AggregatedMachineDatasForMachineDTOS { get; set; }
    private List<int> _chartSeries = [];
    private string[] _xAxisLabels = [];

    protected override void OnParametersSet()
    {
        if(AggregatedMachineDatasForMachineDTOS is null || AggregatedMachineDatasForMachineDTOS.Count == 0)
        {
            return;
        }

        _xAxisLabels = AggregatedMachineDatasForMachineDTOS.Select(x => x.MachineName).ToArray();

        foreach(var aggregatedMachineDatasForMachineDTO in AggregatedMachineDatasForMachineDTOS)
        {
            _chartSeries.Add(aggregatedMachineDatasForMachineDTO.AggregatedMachineDataDTOS
                .Sum(x => x.TotalNumberOfProcessedResources));
        }
    }
}