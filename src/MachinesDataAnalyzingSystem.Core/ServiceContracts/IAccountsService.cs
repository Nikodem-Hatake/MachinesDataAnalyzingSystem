using MachinesDataAnalyzingSystem.Shared.DTO.Accounts;

namespace MachinesDataAnalyzingSystem.Core.ServiceContracts;

public interface IAccountsService
{
    Task<AccountsCountResponse> GetAccountsCount();
}