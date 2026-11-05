using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using MachinesDataAnalyzingSystem.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace MachinesDataAnalyzingSystem.Infrastructure.Repositories;

public class AggregatedMachineDatasRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IAggregatedMachineDatasRepository
{
    public async Task AddAggregatedMachineData(AggregatedMachineData aggregatedMachineData)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.AggregatedMachineDatas.AddAsync(aggregatedMachineData);
        await dbContext.SaveChangesAsync();
    }

    public async Task<AggregatedMachineData?> GetAggregatedMachineData(Guid machineId, 
        DateTime startDate, HowLongIsAggregatedMachineData howLong)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AggregatedMachineDatas.Where
            (x => x.MachineId == machineId && x.StartDate == startDate && x.HowLongIsAggregatedMachineData == howLong).FirstOrDefaultAsync();
    }
}