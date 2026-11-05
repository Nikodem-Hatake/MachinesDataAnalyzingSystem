using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using MachinesDataAnalyzingSystem.Core.Helpers;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.Machine;

namespace MachinesDataAnalyzingSystem.Core.Services;

public class MachinesService(IMachinesRepository machinesRepository) : IMachinesService
{
    public async Task AddMachine(MachineAddRequest machineAddRequest)
    {
        var machine = new Machine()
        {
            Id = Guid.NewGuid(),
            Name = machineAddRequest.MachineName
        };
        ValidationHelper.ModelValidation(machine);
        await machinesRepository.AddMachine(machine);
    }

    public async Task DeleteMachine(MachineDeleteRequest machineDeleteRequest)
    {
        await machinesRepository.DeleteMachine(machineDeleteRequest.MachineId);
    }

    public async Task EditMachine(MachineEditRequest machineEditRequest)
    {
        var machine = new Machine()
        {
            Id = machineEditRequest.MachineId,
            Name = machineEditRequest.NewMachineName
        };
        ValidationHelper.ModelValidation(machine);
        await machinesRepository.EditMachine(machine);
    }

    public async Task<List<MachineResponse>> GetMachines()
    {
        var machines = await machinesRepository.GetMachines();
        return machines.Select(x => new MachineResponse()
        {
            MachineId = x.Id,
            MachineName = x.Name
        }).ToList();
    }
}
