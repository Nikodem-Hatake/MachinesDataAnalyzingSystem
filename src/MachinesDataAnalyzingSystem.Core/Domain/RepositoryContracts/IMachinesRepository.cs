using MachinesDataAnalyzingSystem.Core.Domain.Entities;

namespace MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;

public interface IMachinesRepository
{
    Task AddMachine(Machine machine);
    Task<bool> ContainsMachine(Guid id);
    Task DeleteMachine(Guid id);
    Task EditMachine(Machine machine);
    Task<List<Machine>> GetMachines();
}