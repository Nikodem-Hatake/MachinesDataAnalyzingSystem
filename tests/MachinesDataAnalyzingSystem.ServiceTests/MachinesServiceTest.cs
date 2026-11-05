using AutoFixture;
using FluentAssertions;
using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Core.Services;
using MachinesDataAnalyzingSystem.Shared.DTO.Machine;
using Moq;

namespace MachinesDataAnalyzingSystem.ServiceTests;

public class MachinesServiceTest
{
    private readonly IFixture _fixture = new Fixture();
    private readonly Mock<IMachinesRepository> _machinesRepositoryMock = new();
    private readonly IMachinesService _machinesService;

    public MachinesServiceTest()
    {
        _machinesService = new MachinesService(_machinesRepositoryMock.Object);
    }

    #region AddMachine

    [Fact]
    public async Task AddMachineShouldAddMachineSuccessfully()
    {
        var machineAddRequest = _fixture.Build<MachineAddRequest>()
            .With(x => x.MachineName, "Plastic converter").Create();
        _machinesRepositoryMock.Setup(x => x.AddMachine(It.IsAny<Machine>()));

        var action = async () => await _machinesService.AddMachine(machineAddRequest);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AddMachineShouldThrowWhenMachineNameIsNull()
    {
        var machineAddRequest = _fixture.Build<MachineAddRequest>()
            .With(x => x.MachineName, value: null).Create();
        _machinesRepositoryMock.Setup(x => x.AddMachine(It.IsAny<Machine>()));

        var action = async () => await _machinesService.AddMachine(machineAddRequest);

        await action.Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddMachineShouldThrowWhenMachineNameIsTooShort()
    {
        var machineAddRequest = _fixture.Build<MachineAddRequest>()
            .With(x => x.MachineName, "").Create();
        _machinesRepositoryMock.Setup(x => x.AddMachine(It.IsAny<Machine>()));

        var action = async () => await _machinesService.AddMachine(machineAddRequest);

        await action.Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddMachineShouldThrowWhenMachineNameIsTooLong()
    {
        var machineAddRequest = _fixture.Build<MachineAddRequest>()
            .With(x => x.MachineName, new string('A', 1001)).Create();
        _machinesRepositoryMock.Setup(x => x.AddMachine(It.IsAny<Machine>()));

        var action = async () => await _machinesService.AddMachine(machineAddRequest);

        await action.Should().ThrowExactlyAsync<ArgumentException>();
    }

    #endregion
    #region DeleteMachine

    [Fact]
    public async Task DeleteMachineShouldDeleteMachineSuccessfully()
    {
        var machineDeleteRequest = _fixture.Create<MachineDeleteRequest>();
        _machinesRepositoryMock.Setup(x => x.DeleteMachine(It.IsAny<Guid>()));

        var action = async () => await _machinesService.DeleteMachine(machineDeleteRequest);

        await action.Should().NotThrowAsync();
    }

    #endregion
    #region EditMachine

    [Fact]
    public async Task EditMachineShouldEditMachineSuccessfully()
    {
        var machineEditRequest = _fixture.Build<MachineEditRequest>()
            .With(x => x.NewMachineName, "Plastic converter").Create();
        _machinesRepositoryMock.Setup(x => x.EditMachine(It.IsAny<Machine>()));

        var action = async() => await _machinesService.EditMachine(machineEditRequest);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task EditMachineShouldThrowWhenMachineNameIsNull()
    {
        var machineEditRequest = _fixture.Build<MachineEditRequest>()
            .With(x => x.NewMachineName, value: null).Create();
        _machinesRepositoryMock.Setup(x => x.EditMachine(It.IsAny<Machine>()));

        var action = async () => await _machinesService.EditMachine(machineEditRequest);

        await action.Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Fact]
    public async Task EditMachineShouldThrowWhenMachineNameIsTooShort()
    {
        var machineEditRequest = _fixture.Build<MachineEditRequest>()
            .With(x => x.NewMachineName, "").Create();
        _machinesRepositoryMock.Setup(x => x.EditMachine(It.IsAny<Machine>()));

        var action = async () => await _machinesService.EditMachine(machineEditRequest);

        await action.Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Fact]
    public async Task EditMachineShouldThrowWhenMachineNameIsTooLong()
    {
        var machineEditRequest = _fixture.Build<MachineEditRequest>()
            .With(x => x.NewMachineName, new string('A', 1001)).Create();
        _machinesRepositoryMock.Setup(x => x.EditMachine(It.IsAny<Machine>()));

        var action = async () => await _machinesService.EditMachine(machineEditRequest);

        await action.Should().ThrowExactlyAsync<ArgumentException>();
    }

    #endregion
    #region GetMachines

    [Fact]
    public async Task GetMachinesShouldReturnEmptyListWhenThereAreNoMachines()
    {
        _machinesRepositoryMock.Setup(x => x.GetMachines())
            .ReturnsAsync([]);

        var machines = await _machinesService.GetMachines();

        machines.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMachinesShouldReturnListWithMachinesFromRepository()
    {
        var machines = _fixture.Create<List<Machine>>();
        _machinesRepositoryMock.Setup(x => x.GetMachines())
            .ReturnsAsync(machines);

        var machinesFromGetMachines = await _machinesService.GetMachines();

        machines.Count.Should().Be(machinesFromGetMachines.Count);
        for(int i = 0; i < machines.Count; ++i)
        {
            machines[i].Equals(machinesFromGetMachines[i]).Should().BeTrue();
        }
    }

    #endregion
}