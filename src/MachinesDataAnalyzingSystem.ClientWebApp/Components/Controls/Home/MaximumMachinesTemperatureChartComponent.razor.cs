using MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Components.Controls.Home;

public partial class MaximumMachinesTemperatureChartComponent
{
    [Parameter]
    public List<AggregatedMachineDatasForMachineDTO> AggregatedMachineDatasForMachineDTOS { get; set; }
    private List<ChartSeries<float>> _chartSeries = [];
    private readonly LineChartOptions _options = new()
    {
        XAxisTitle = "Time",
        YAxisRequireZeroPoint = true,
        YAxisTicks = 20,
        YAxisTitle = "Maximum temperature"
    };
    private readonly List<string> _xAxisLabels = [];

    protected override void OnParametersSet()
    {
        if(AggregatedMachineDatasForMachineDTOS is null || AggregatedMachineDatasForMachineDTOS.Count == 0)
        {
            return;
        }

        _xAxisLabels.AddRange(AggregatedMachineDatasForMachineDTOS.MaxBy(x => x.AggregatedMachineDataDTOS.Count)
            .AggregatedMachineDataDTOS.Select(x => x.StartDate.ToString("HH:mm")));

        foreach(var aggregatedMachineDatasForMachineDTO in AggregatedMachineDatasForMachineDTOS)
        {
            _chartSeries.Add(new()
            {
                Data = aggregatedMachineDatasForMachineDTO.AggregatedMachineDataDTOS.Select(x => x.MaxTemperature).ToArray(),
                Name = aggregatedMachineDatasForMachineDTO.MachineName
            });
        }
    }
}