using System.ComponentModel.DataAnnotations;

namespace MachinesDataAnalyzingSystem.Shared.DTO.Machine;

public class MachineEditRequest
{
    [Required]
    public string NewMachineName { get; set; }
    public Guid MachineId { get; set; }
}