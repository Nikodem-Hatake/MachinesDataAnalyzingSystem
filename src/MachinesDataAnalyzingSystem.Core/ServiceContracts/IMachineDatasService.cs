using MachinesDataAnalyzingSystem.Shared.DTO.MachineDatas;

namespace MachinesDataAnalyzingSystem.Core.ServiceContracts;

public interface IMachineDatasService
{
    Task AddMachineData(MachineDataDTO machineDataDTO);
    Task<MachineDatasCountResponse> GetMachineDatasCount();
}