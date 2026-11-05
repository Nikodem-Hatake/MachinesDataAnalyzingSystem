using MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;
using MachinesDataAnalyzingSystem.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace MachinesDataAnalyzingSystem.Core.Domain.Entities;

public class AggregatedMachineData
{
    public float AverageMotorsSpeed { get; set; }
    public float AverageTemperature { get; set; }
    public HowLongIsAggregatedMachineData HowLongIsAggregatedMachineData { get; set; }
    [Key]
    public Guid Id { get; set; }
    public Guid MachineId { get; set; }
    public float MaxTemperature { get; set; }
    public float MinTemperature { get; set; }
    public DateTime StartDate { get; set; }
    public int TotalNumberOfProcessedResources { get; set; }
    public float TotalNumberOfSecondsToProcessResources { get; set; }

    public override bool Equals(object obj)
    {
        if(obj is not AggregatedMachineDataDTO aggregatedMachineDataDTO)
        {
            return base.Equals(obj);
        }

        return aggregatedMachineDataDTO.AverageMotorsSpeed == AverageMotorsSpeed
            && aggregatedMachineDataDTO.AverageTemperature == AverageTemperature
            && aggregatedMachineDataDTO.HowLongIsAggregatedMachineData == HowLongIsAggregatedMachineData
            && aggregatedMachineDataDTO.MachineId == MachineId
            && aggregatedMachineDataDTO.MaxTemperature == MaxTemperature
            && aggregatedMachineDataDTO.MinTemperature == MinTemperature
            && aggregatedMachineDataDTO.StartDate == StartDate
            && aggregatedMachineDataDTO.TotalNumberOfProcessedResources == TotalNumberOfProcessedResources
            && aggregatedMachineDataDTO.TotalNumberOfSecondsToProcessResources == TotalNumberOfSecondsToProcessResources;
    }

    public AggregatedMachineDataDTO ToDTO()
    {
        return new()
        {
            AverageMotorsSpeed = AverageMotorsSpeed,
            AverageTemperature = AverageTemperature,
            HowLongIsAggregatedMachineData = HowLongIsAggregatedMachineData,
            MachineId = MachineId,
            MaxTemperature = MaxTemperature,
            MinTemperature = MinTemperature,
            StartDate = StartDate,
            TotalNumberOfProcessedResources = TotalNumberOfProcessedResources,
            TotalNumberOfSecondsToProcessResources = TotalNumberOfSecondsToProcessResources
        };
    }
}