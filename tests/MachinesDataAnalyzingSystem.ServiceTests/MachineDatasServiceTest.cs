using FluentAssertions;
using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Core.Services;
using MachinesDataAnalyzingSystem.Shared.DTO;
using Moq;

namespace MachinesDataAnalyzingSystem.ServiceTests;

public class MachineDatasServiceTest
{
    private readonly Mock<IMachineDatasRepository> _machineDatasRepositoryMock = new();
    private readonly Mock<IMachinesRepository> _machinesRepositoryMock = new();
    private readonly IMachineDatasService _machineDatasService;

    public MachineDatasServiceTest()
    {
        _machineDatasService = new MachineDatasService(_machineDatasRepositoryMock.Object, _machinesRepositoryMock.Object);
    }

    #region AddMachineData

    [Fact]
    public async Task AddMachineDataShouldAddMachineDataSuccessfullyWhenMachineIsFound()
    {
        var machineDataDTO = new MachineDataDTO();
        _machinesRepositoryMock.Setup(x => x.ContainsMachine(It.IsAny<Guid>())).ReturnsAsync(true);

        var action = async () => await _machineDatasService.AddMachineData(machineDataDTO);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AddMachineDataShouldNotAddMachineDataWhenMachineIsNotFound()
    {
        var machineDataDTO = new MachineDataDTO();
        _machinesRepositoryMock.Setup(x => x.ContainsMachine(It.IsAny<Guid>())).ReturnsAsync(false);

        var action = async () => await _machineDatasService.AddMachineData(machineDataDTO);

        await action.Should().ThrowExactlyAsync<ArgumentException>();
    }

    #endregion
}