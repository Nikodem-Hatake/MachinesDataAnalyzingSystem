using MachinesDataAnalyzingSystem.Shared.DTO;

namespace MachinesDataAnalyzingSystem.Core.ServiceContracts;

public interface IMachineDatasService
{
    Task AddMachineData(MachineDataDTO machineDataDTO);
}