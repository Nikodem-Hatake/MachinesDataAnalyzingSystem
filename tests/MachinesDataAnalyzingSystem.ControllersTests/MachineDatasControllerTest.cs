using AutoFixture;
using FluentAssertions;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.MachineDatas;
using MachinesDataAnalyzingSystem.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MachinesDataAnalyzingSystem.ControllersTests;

public class MachineDatasControllerTest
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IMachineDatasService> _machineDatasServiceMock = new();
    private readonly MachineDatasController _machineDatasController;

    public MachineDatasControllerTest()
    {
        _machineDatasController = new(_machineDatasServiceMock.Object);
    }

    #region GetMachineDatasCount

    [Fact]
    public async Task GetMachineDatasCountShouldReturnCorrectCount()
    {
        var machineDatasCount = _fixture.Create<MachineDatasCountResponse>();
        _machineDatasServiceMock.Setup(x => x.GetMachineDatasCount())
            .ReturnsAsync(machineDatasCount);

        var response = await _machineDatasController.GetMachineDatasCount();

        response.Value.Should().BeEquivalentTo(machineDatasCount);
    }

    #endregion
    #region PostMachineData

    [Fact]
    public async Task PostMachineDataShouldAddMachineDataSuccessfully()
    {
        _machineDatasServiceMock.Setup(x => x.AddMachineData(It.IsAny<MachineDataDTO>()))
            .ReturnsAsync(true);

        var result = await _machineDatasController.PostMachineData(_fixture.Create<MachineDataDTO>());

        result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PostMachineDataShouldNotAddMachineDataForMachineNotFound()
    {
        _machineDatasServiceMock.Setup(x => x.AddMachineData(It.IsAny<MachineDataDTO>()))
            .ReturnsAsync(false);

        var result = await _machineDatasController.PostMachineData(_fixture.Create<MachineDataDTO>());

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion
}