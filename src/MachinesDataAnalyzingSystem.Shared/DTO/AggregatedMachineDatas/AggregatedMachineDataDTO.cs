using MachinesDataAnalyzingSystem.Shared.Enums;

namespace MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;

public class AggregatedMachineDataDTO
{
    public float AverageMotorsSpeed { get; set; }
    public float AverageTemperature { get; set; }
    public HowLongIsAggregatedMachineData HowLongIsAggregatedMachineData { get; set; }
    public Guid MachineId { get; set; }
    public float MaxTemperature { get; set; }
    public float MinTemperature { get; set; }
    public DateTime StartDate { get; set; }
    public int TotalNumberOfProcessedResources { get; set; }
    public float TotalNumberOfSecondsToProcessResources { get; set; }
}