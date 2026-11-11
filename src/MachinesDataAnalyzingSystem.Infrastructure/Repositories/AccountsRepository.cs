using MachinesDataAnalyzingSystem.Core.Domain.Identity;
using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MachinesDataAnalyzingSystem.Infrastructure.Repositories;

public class AccountsRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory, 
    RoleManager<ApplicationRole> roleManager) : IAccountsRepository
{
    public async Task<long> GetTotalAccountsCount()
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.Users.LongCountAsync();
    }

    public async Task<long> GetTotalAdminAccountsCount()
    {
        var adminRole = await roleManager.FindByNameAsync("admin");
        using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.UserRoles.LongCountAsync(x => x.RoleId == adminRole.Id);
    }
}