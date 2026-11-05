using MachinesDataAnalyzingSystem.Shared.DTO.Machine;
using System.ComponentModel.DataAnnotations;

namespace MachinesDataAnalyzingSystem.Core.Domain.Entities;

public class Machine
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    public override bool Equals(object obj)
    {
        if(obj is MachineResponse machineResponse)
        {
            return machineResponse.MachineName == Name
                && machineResponse.MachineId == Id;
            
        }

        return base.Equals(obj);
    }
}