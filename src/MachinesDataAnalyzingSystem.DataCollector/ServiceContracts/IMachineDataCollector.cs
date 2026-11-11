using MachinesDataAnalyzingSystem.Shared.DTO.MachineDatas;

namespace MachinesDataAnalyzingSystem.DataCollector.ServiceContracts;

public interface IMachineDataCollector
{
    List<MachineDataDTO> CollectNewData();
}