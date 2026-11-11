using MachinesDataAnalyzingSystem.ClientWebApp.Helpers;
using MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Components.Controls.MachineDetails;

public partial class MachineAverageMotorsSpeedChartComponent
{
    [Parameter]
    public List<AggregatedMachineDataDTO> AggregatedMachineDatas { get; set; }
    private List<ChartSeries<float>> _chartSeries = [];
    private BarChartOptions _options = new()
    {
        XAxisTitle = "Time",
        YAxisTicks = 5,
        YAxisTitle = "Speed"
    };
    private List<string> _xAxisLabels = [];

    protected override void OnParametersSet()
    {
        if(AggregatedMachineDatas is null || AggregatedMachineDatas.Count == 0)
        {
            return;
        }

        _xAxisLabels.Clear();
        _xAxisLabels.AddRange(AggregatedMachineDatas.Select(x => x.StartDate.ToString
            (ChartXAxisLabelsDateFormater.GetFormat(AggregatedMachineDatas[0].HowLongIsAggregatedMachineData))));
        _chartSeries.Clear();
        _chartSeries.Add(new()
        {
            Data = AggregatedMachineDatas.Select(x => x.AverageMotorsSpeed).ToArray(),
            Name = "Avg"
        });
    }
}