using MachinesDataAnalyzingSystem.Shared.DTO.Machine;

namespace MachinesDataAnalyzingSystem.Core.ServiceContracts;

public interface IMachinesService
{
    Task<bool> AddMachine(MachineAddRequest machineAddRequest);
    Task<bool> DeleteMachine(MachineDeleteRequest machineDeleteRequest);
    Task<bool> EditMachine(MachineEditRequest machineEditRequest);
    Task<List<MachineResponse>> GetMachines();
}