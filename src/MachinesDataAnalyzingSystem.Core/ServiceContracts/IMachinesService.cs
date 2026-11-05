using MachinesDataAnalyzingSystem.Shared.DTO.Machine;

namespace MachinesDataAnalyzingSystem.Core.ServiceContracts;

public interface IMachinesService
{
    Task AddMachine(MachineAddRequest machineAddRequest);
    Task DeleteMachine(MachineDeleteRequest machineDeleteRequest);
    Task EditMachine(MachineEditRequest machineEditRequest);
    Task<List<MachineResponse>> GetMachines();
}