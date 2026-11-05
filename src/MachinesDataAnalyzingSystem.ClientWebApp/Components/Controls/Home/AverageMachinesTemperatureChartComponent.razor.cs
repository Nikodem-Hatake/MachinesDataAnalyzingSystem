using MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Components.Controls.Home;

public partial class AverageMachinesTemperatureChartComponent
{
    [Parameter]
    public List<AggregatedMachineDatasForMachineDTO> AggregatedMachineDatasForMachineDTOS { get; set; }
    private List<ChartSeries<float>> _chartSeries = [];
    private readonly StackedBarChartOptions _options = new()
    {
        XAxisTitle = "Time",
        YAxisTicks = 20,
        YAxisTitle = "Average temperature"
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
                Data = aggregatedMachineDatasForMachineDTO.AggregatedMachineDataDTOS.Select(x => x.AverageTemperature).ToArray(),
                Name = aggregatedMachineDatasForMachineDTO.MachineName
            });
        }
    }
}