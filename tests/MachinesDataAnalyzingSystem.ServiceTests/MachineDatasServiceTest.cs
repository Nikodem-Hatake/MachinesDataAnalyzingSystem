using FluentAssertions;
using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Core.Services;
using MachinesDataAnalyzingSystem.Shared.DTO.MachineDatas;
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
        _machineDatasRepositoryMock.Setup(x => x.AddMachineData(It.IsAny<MachineData>())).ReturnsAsync(true);

        var result = await _machineDatasService.AddMachineData(machineDataDTO);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task AddMachineDataShouldNotAddMachineDataWhenMachineIsNotFound()
    {
        var machineDataDTO = new MachineDataDTO();
        _machineDatasRepositoryMock.Setup(x => x.AddMachineData(It.IsAny<MachineData>())).ReturnsAsync(false);

        var result = await _machineDatasService.AddMachineData(machineDataDTO);

        result.Should().BeFalse();
    }

    #endregion
    #region GetMachineDatasCount

    [Fact]
    public async Task GetMachineDatasCountShouldReturnCorrectCount()
    {
        long correctCountValue = 5;
        _machineDatasRepositoryMock.Setup(x => x.GetMachineDatasCount())
            .ReturnsAsync(correctCountValue);

        var result = await _machineDatasService.GetMachineDatasCount();

        result.Count.Should().Be(correctCountValue);
    }

    #endregion
}