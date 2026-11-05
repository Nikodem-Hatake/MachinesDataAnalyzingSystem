using MachinesDataAnalyzingSystem.Shared.Enums;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Helpers;

public static class ChartXAxisLabelsDateFormater
{
    public static string GetFormat(HowLongIsAggregatedMachineData howLong)
    {
        switch(howLong)
        {
            case HowLongIsAggregatedMachineData.TenMinutes:
            case HowLongIsAggregatedMachineData.OneHour:
            {
                return "HH:mm";
            }
            case HowLongIsAggregatedMachineData.OneDay:
            case HowLongIsAggregatedMachineData.OneWeek:
            {
                return "dd.MM";
            }
            case HowLongIsAggregatedMachineData.OneMonth:
            {
                return "MM.yyyy";
            }
        }

        return "";
    }
}