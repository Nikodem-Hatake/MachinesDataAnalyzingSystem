using MachinesDataAnalyzingSystem.Shared.DTO.MachineDatas;

namespace MachinesDataAnalyzingSystem.Core.ServiceContracts;

public interface IMachineDatasService
{
    Task<bool> AddMachineData(MachineDataDTO machineDataDTO);
    Task<MachineDatasCountResponse> GetMachineDatasCount();
}