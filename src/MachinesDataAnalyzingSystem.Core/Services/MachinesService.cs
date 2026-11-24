using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using MachinesDataAnalyzingSystem.Core.Helpers;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.Machine;

namespace MachinesDataAnalyzingSystem.Core.Services;

public class MachinesService(IMachinesRepository machinesRepository) : IMachinesService
{
    public async Task<bool> AddMachine(MachineAddRequest machineAddRequest)
    {
        var machine = new Machine()
        {
            Id = Guid.NewGuid(),
            Name = machineAddRequest.MachineName
        };
        try
        {
            ValidationHelper.ModelValidation(machine);
        }
        catch(Exception)
        {
            return false;
        }

        return await machinesRepository.AddMachine(machine);
    }

    public async Task<bool> DeleteMachine(MachineDeleteRequest machineDeleteRequest)
    {
        return await machinesRepository.DeleteMachine(machineDeleteRequest.MachineId);
    }

    public async Task<bool> EditMachine(MachineEditRequest machineEditRequest)
    {
        var machine = new Machine()
        {
            Id = machineEditRequest.MachineId,
            Name = machineEditRequest.NewMachineName
        };

        try
        {
            ValidationHelper.ModelValidation(machine);
        }
        catch(Exception)
        {
            return false;
        }

        return await machinesRepository.EditMachine(machine);
    }

    public async Task<List<MachineResponse>> GetMachines()
    {
        var machines = await machinesRepository.GetMachines();
        return machines.Select(x => x.ToDTO()).ToList();
    }
}
