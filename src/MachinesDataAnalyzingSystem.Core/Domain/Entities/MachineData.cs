using System.ComponentModel.DataAnnotations;

namespace MachinesDataAnalyzingSystem.Core.Domain.Entities;

public class MachineData
{
    public float AverageMotorsSpeed { get; set; }
    public float AverageTemperature { get; set; }
    public DateTime DateTime { get; set; }
    [Key]
    public Guid Id { get; set; }
    public int NumberOfProcessedResources { get; set; }
    public float NumberOfSecondsToProcessResources { get; set; }
    public Guid MachineId { get; set; }
}