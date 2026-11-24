using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace MachinesDataAnalyzingSystem.Infrastructure.Repositories;

public class MachinesRepository(IDbContextFactory<ApplicationDbContext> dbContextBuilder) : IMachinesRepository
{
    public async Task<bool> AddMachine(Machine machine)
    {
        using var dbContext = await dbContextBuilder.CreateDbContextAsync();
        if((await dbContext.Machines.FirstOrDefaultAsync(x => x.Name == machine.Name)) is not null)
        {
            return false;
        }

        await dbContext.Machines.AddAsync(machine);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteMachine(Guid id)
    {
        using var dbContext = await dbContextBuilder.CreateDbContextAsync();
        if((await dbContext.Machines.FirstOrDefaultAsync(x => x.Id == id)) is null)
        {
            return false;
        }

        dbContext.Machines.Remove(dbContext.Machines.First(x => x.Id == id));
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EditMachine(Machine machine)
    {
        using var dbContext = await dbContextBuilder.CreateDbContextAsync();
        if((await dbContext.Machines.FirstOrDefaultAsync(x => x.Id == machine.Id)) is null)
        {
            return false;
        }

        var machineToEdit = await dbContext.Machines.FirstAsync(x => x.Id == machine.Id);
        machineToEdit.Name = machine.Name;
        dbContext.Machines.Update(machineToEdit);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<Machine>> GetMachines()
    {
        using var dbContext = await dbContextBuilder.CreateDbContextAsync();
        return await dbContext.Machines.ToListAsync();
    }
}