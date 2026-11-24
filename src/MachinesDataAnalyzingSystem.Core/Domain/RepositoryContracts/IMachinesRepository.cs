using MachinesDataAnalyzingSystem.Core.Domain.Entities;

namespace MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;

public interface IMachinesRepository
{
    Task<bool> AddMachine(Machine machine);
    Task<bool> DeleteMachine(Guid id);
    Task<bool> EditMachine(Machine machine);
    Task<List<Machine>> GetMachines();
}