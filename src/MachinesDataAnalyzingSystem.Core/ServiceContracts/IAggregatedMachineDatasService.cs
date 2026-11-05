using MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;
using MachinesDataAnalyzingSystem.Shared.Enums;

namespace MachinesDataAnalyzingSystem.Core.ServiceContracts;

public interface IAggregatedMachineDatasService
{
    Task<List<AggregatedMachineDataDTO>> GetAggregatedMachineDatas(Guid machineId, DateTime startDate, 
        HowLongIsAggregatedMachineData howLong, int howMany);
}