using AutoFixture;
using FluentAssertions;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.Machine;
using MachinesDataAnalyzingSystem.WebAPI.Controllers;
using Moq;

namespace MachinesDataAnalyzingSystem.ControllersTests;

public class MachinesControllerTest
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IMachinesService> _machinesServiceMock = new();
    private readonly MachinesController _machinesController;
    
    public MachinesControllerTest()
    {
        _machinesController = new(_machinesServiceMock.Object);
    }

    #region DeleteMachine

    [Fact]
    public async Task DeleteMachineShouldDeleteMachineForCorrectRequest()
    {
        var request = _fixture.Create<MachineDeleteRequest>();
        _machinesServiceMock.Setup(x => x.DeleteMachine(It.IsAny<MachineDeleteRequest>()));

        var action = async () => await _machinesController.DeleteMachine(request);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteMachineShouldThrowExceptionForIncorrectRequest()
    {
        var request = _fixture.Create<MachineDeleteRequest>();
        _machinesServiceMock.Setup(x => x.DeleteMachine(It.IsAny<MachineDeleteRequest>()))
            .ThrowsAsync(new());

        var action = async () => await _machinesController.DeleteMachine(request);

        await action.Should().ThrowAsync();
    }

    #endregion
    #region GetMachines

    [Fact]
    public async Task GetMachinesShouldReturnMachinesFromService()
    {
        var machines = _fixture.Create<List<MachineResponse>>();
        _machinesServiceMock.Setup(x => x.GetMachines())
            .ReturnsAsync(machines);

        var response = await _machinesController.GetMachines();

        response.Value.Should().BeEquivalentTo(machines);
    }

    #endregion
    #region GetMachinesForService

    [Fact]
    public async Task GetMachinesForServiceShouldReturnMachinesFromService()
    {
        var machines = _fixture.Create<List<MachineResponse>>();
        _machinesServiceMock.Setup(x => x.GetMachines())
            .ReturnsAsync(machines);

        var response = await _machinesController.GetMachinesForService();

        response.Value.Should().BeEquivalentTo(machines);
    }

    #endregion
    #region PostMachine

    [Fact]
    public async Task PostMachineShouldAddMachineForCorrectRequest()
    {
        var request = _fixture.Build<MachineAddRequest>()
            .With(x => x.MachineName, "Plastic converter")
            .Create();
        _machinesServiceMock.Setup(x => x.AddMachine(It.IsAny<MachineAddRequest>()));

        var action = async () => await _machinesController.PostMachine(request);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task PostMachineShouldThrowExceptionForIncorrectRequest()
    {
        var request = _fixture.Build<MachineAddRequest>()
            .With(x => x.MachineName, "")
            .Create();
        _machinesServiceMock.Setup(x => x.AddMachine(It.IsAny<MachineAddRequest>()))
            .ThrowsAsync(new());

        var action = async () => await _machinesController.PostMachine(request);

        await action.Should().ThrowAsync();
    }

    #endregion
    #region PutMachine

    [Fact]
    public async Task PutMachineShouldEditMachineForCorrectRequest()
    {
        var request = _fixture.Create<MachineEditRequest>();
        _machinesServiceMock.Setup(x => x.EditMachine(It.IsAny<MachineEditRequest>()));

        var action = async () => await _machinesController.PutMachine(request);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task PutMachineShouldEditMachineForIncorrectRequest()
    {
        var request = _fixture.Build<MachineEditRequest>()
            .With(x => x.NewMachineName, "")
            .Create();
        _machinesServiceMock.Setup(x => x.EditMachine(It.IsAny<MachineEditRequest>()))
            .ThrowsAsync(new());

        var action = async () => await _machinesController.PutMachine(request);

        await action.Should().ThrowAsync();
    }

    #endregion
}
