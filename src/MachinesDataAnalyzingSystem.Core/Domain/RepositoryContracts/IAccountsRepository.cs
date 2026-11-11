namespace MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;

public interface IAccountsRepository
{
    Task<long> GetTotalAccountsCount();
    Task<long> GetTotalAdminAccountsCount();
}