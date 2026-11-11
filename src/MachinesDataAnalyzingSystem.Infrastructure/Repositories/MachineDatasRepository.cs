using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace MachinesDataAnalyzingSystem.Infrastructure.Repositories;

public class MachineDatasRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IMachineDatasRepository
{
    public async Task AddMachineData(MachineData machineData)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.MachineDatas.AddAsync(machineData);
        await dbContext.SaveChangesAsync();
    }

    public async Task<long> GetMachineDatasCount()
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.MachineDatas.LongCountAsync();
    }

    public async Task<List<MachineData>> GetMachineDatasForAggregation(Guid machineId, DateTime startDate, DateTime endDate)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.MachineDatas.Where(x => x.MachineId == machineId && x.DateTime >= startDate && x.DateTime < endDate).ToListAsync();
    }
}