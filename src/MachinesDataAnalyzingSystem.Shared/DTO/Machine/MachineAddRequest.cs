using System.ComponentModel.DataAnnotations;

namespace MachinesDataAnalyzingSystem.Shared.DTO.Machine;

public class MachineAddRequest
{
    [Required]
    public string MachineName { get; set; }
}