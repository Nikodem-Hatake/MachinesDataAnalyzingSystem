using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Shared.Enums;

namespace MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;

public interface IAggregatedMachineDatasRepository
{
    Task AddAggregatedMachineData(AggregatedMachineData aggregatedMachineData);
    Task <AggregatedMachineData?> GetAggregatedMachineData(Guid machineId, DateTime startDate, HowLongIsAggregatedMachineData howLong);
}