using FluentAssertions;
using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Core.Services;
using Moq;

namespace MachinesDataAnalyzingSystem.ServiceTests;

public class AccountsServiceTest
{
    private readonly Mock<IAccountsRepository> _accountsRepositoryMock = new();
    private readonly IAccountsService _accountsService;

    public AccountsServiceTest()
    {
        _accountsService = new AccountsService(_accountsRepositoryMock.Object);
    }

    #region GetAccountsCount

    [Fact]
    public async Task GetAccountsCountShouldReturnCorrectCounts()
    {
        long totalAccounts = 5;
        long adminAccounts = 2;
        _accountsRepositoryMock.Setup(x => x.GetTotalAccountsCount())
            .ReturnsAsync(totalAccounts);
        _accountsRepositoryMock.Setup(x => x.GetTotalAdminAccountsCount())
            .ReturnsAsync(adminAccounts);

        var result = await _accountsService.GetAccountsCount();

        result.TotalCount.Should().Be(totalAccounts);
        result.AdminAccountsCount.Should().Be(adminAccounts);
    }

    #endregion
}