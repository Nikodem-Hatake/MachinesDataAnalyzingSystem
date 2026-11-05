using MachinesDataAnalyzingSystem.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;

public class AggregatedMachineDataRequest
{
    public HowLongIsAggregatedMachineData HowLong { get; set; } = HowLongIsAggregatedMachineData.TenMinutes;
    [Range(1, 10)]
    public int HowMany { get; set; } = 1;
    [Required]
    public DateTime? StartDate { get; set; } = DateTime.UtcNow.Date;
    [Required]
    public TimeSpan? StartTime { get; set; } = TimeSpan.Zero;
}