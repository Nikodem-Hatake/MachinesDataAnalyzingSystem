using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace MachinesDataAnalyzingSystem.Infrastructure.Repositories;

public class MachinesRepository(IDbContextFactory<ApplicationDbContext> dbContextBuilder) : IMachinesRepository
{
    public async Task AddMachine(Machine machine)
    {
        using var dbContext = await dbContextBuilder.CreateDbContextAsync();
        await dbContext.Machines.AddAsync(machine);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> ContainsMachine(Guid id)
    {
        using var dbContext = await dbContextBuilder.CreateDbContextAsync();
        return (await dbContext.Machines.FirstOrDefaultAsync(x => x.Id == id)) is not null;
    }

    public async Task DeleteMachine(Guid id)
    {
        using var dbContext = await dbContextBuilder.CreateDbContextAsync();
        dbContext.Machines.Remove(dbContext.Machines.First(x => x.Id == id));
        await dbContext.SaveChangesAsync();
    }

    public async Task EditMachine(Machine machine)
    {
        using var dbContext = await dbContextBuilder.CreateDbContextAsync();
        dbContext.Machines.Update(machine);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<Machine>> GetMachines()
    {
        using var dbContext = await dbContextBuilder.CreateDbContextAsync();
        return await dbContext.Machines.ToListAsync();
    }
}