using MachinesDataAnalyzingSystem.Core.Domain.Entities;

namespace MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;

public interface IMachineDatasRepository
{
    Task AddMachineData(MachineData machineData);
    Task<long> GetMachineDatasCount();
    Task<List<MachineData>> GetMachineDatasForAggregation(Guid machineId, DateTime startDate, DateTime endDate);
}