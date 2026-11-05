using MachinesDataAnalyzingSystem.Shared.DTO;

namespace MachinesDataAnalyzingSystem.DataCollector.ServiceContracts;

public interface IMachineDataCollector
{
    List<MachineDataDTO> CollectNewData();
}