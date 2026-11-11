using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.Accounts;

namespace MachinesDataAnalyzingSystem.Core.Services;

public class AccountsService(IAccountsRepository accountsRepository) : IAccountsService
{
    public async Task<AccountsCountResponse> GetAccountsCount()
    {
        return new()
        {
            AdminAccountsCount = await accountsRepository.GetTotalAdminAccountsCount(),
            TotalCount = await accountsRepository.GetTotalAccountsCount()
        };
    }
}