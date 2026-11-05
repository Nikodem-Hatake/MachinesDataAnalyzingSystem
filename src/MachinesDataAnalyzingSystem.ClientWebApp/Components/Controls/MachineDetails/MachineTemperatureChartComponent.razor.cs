using MachinesDataAnalyzingSystem.ClientWebApp.Helpers;
using MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Components.Controls.MachineDetails;

public partial class MachineTemperatureChartComponent
{
    [Parameter]
    public List<AggregatedMachineDataDTO> AggregatedMachineDatas { get; set; }
    private List<ChartSeries<float>> _chartSeries = [];
    private BarChartOptions _options = new()
    {
        XAxisTitle = "Time",
        YAxisTicks = 20,
        YAxisTitle = "Temperature"
    };
    private List<string> _xAxisLabels = [];

    protected override void OnParametersSet()
    {
        if(AggregatedMachineDatas is null || AggregatedMachineDatas.Count == 0)
        {
            return;
        }

        _xAxisLabels.AddRange(AggregatedMachineDatas.Select(x => x.StartDate.ToString
            (ChartXAxisLabelsDateFormater.GetFormat(AggregatedMachineDatas[0].HowLongIsAggregatedMachineData))));
        _chartSeries.Clear();
        _chartSeries.Add(new()
        {
            Data = AggregatedMachineDatas.Select(x => x.AverageTemperature).ToArray(),
            Name = "Avg"
        });
        _chartSeries.Add(new()
        {
            Data = AggregatedMachineDatas.Select(x => x.MinTemperature).ToArray(),
            Name = "Min"
        });
        _chartSeries.Add(new()
        {
            Data = AggregatedMachineDatas.Select(x => x.MaxTemperature).ToArray(),
            Name = "Max"
        });
    }
}