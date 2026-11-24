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
        _machinesRepositoryMock.Setup(x => x.AddMachine(It.IsAny<Machine>()))
            .ReturnsAsync(true);

        var result = await _machinesService.AddMachine(machineAddRequest);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task AddMachineShouldNotAddMachineWhenMachineNameIsNull()
    {
        var machineAddRequest = _fixture.Build<MachineAddRequest>()
            .With(x => x.MachineName, value: null).Create();
        _machinesRepositoryMock.Setup(x => x.AddMachine(It.IsAny<Machine>()))
            .ReturnsAsync(true);

        var result = await _machinesService.AddMachine(machineAddRequest);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddMachineShouldNotAddMachineWhenMachineNameIsTooShort()
    {
        var machineAddRequest = _fixture.Build<MachineAddRequest>()
            .With(x => x.MachineName, "").Create();
        _machinesRepositoryMock.Setup(x => x.AddMachine(It.IsAny<Machine>()))
            .ReturnsAsync(true);

        var result = await _machinesService.AddMachine(machineAddRequest);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddMachineShouldNotAddMachineWhenMachineNameIsTooLong()
    {
        var machineAddRequest = _fixture.Build<MachineAddRequest>()
            .With(x => x.MachineName, new string('A', 1001)).Create();
        _machinesRepositoryMock.Setup(x => x.AddMachine(It.IsAny<Machine>()))
            .ReturnsAsync(true);

        var result = await _machinesService.AddMachine(machineAddRequest);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddMachineShouldNotAddMachineWhenMachineNameAlreadyExists()
    {
        var machineAddRequest = _fixture.Build<MachineAddRequest>()
            .With(x => x.MachineName, new string('A', 1001)).Create();
        _machinesRepositoryMock.Setup(x => x.AddMachine(It.IsAny<Machine>()))
            .ReturnsAsync(false);

        var result = await _machinesService.AddMachine(machineAddRequest);

        result.Should().BeFalse();
    }

    #endregion
    #region DeleteMachine

    [Fact]
    public async Task DeleteMachineShouldDeleteMachineSuccessfully()
    {
        var machineDeleteRequest = _fixture.Create<MachineDeleteRequest>();
        _machinesRepositoryMock.Setup(x => x.DeleteMachine(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        var result = await _machinesService.DeleteMachine(machineDeleteRequest);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteMachineShouldNotDeleteMachineWhenMachineWithSelectedNameAlreadyExists()
    {
        var machineDeleteRequest = _fixture.Create<MachineDeleteRequest>();
        _machinesRepositoryMock.Setup(x => x.DeleteMachine(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        var result = await _machinesService.DeleteMachine(machineDeleteRequest);

        result.Should().BeFalse();
    }

    #endregion
    #region EditMachine

    [Fact]
    public async Task EditMachineShouldEditMachineSuccessfully()
    {
        var machineEditRequest = _fixture.Build<MachineEditRequest>()
            .With(x => x.NewMachineName, "Plastic converter").Create();
        _machinesRepositoryMock.Setup(x => x.EditMachine(It.IsAny<Machine>()))
            .ReturnsAsync(true);

        var result = await _machinesService.EditMachine(machineEditRequest);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task EditMachineShouldNotEditMachineWhenMachineNameIsNull()
    {
        var machineEditRequest = _fixture.Build<MachineEditRequest>()
            .With(x => x.NewMachineName, value: null).Create();
        _machinesRepositoryMock.Setup(x => x.EditMachine(It.IsAny<Machine>()))
            .ReturnsAsync(true);

        var result = await _machinesService.EditMachine(machineEditRequest);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task EditMachineShouldNotEditMachineWhenMachineNameIsTooShort()
    {
        var machineEditRequest = _fixture.Build<MachineEditRequest>()
            .With(x => x.NewMachineName, "").Create();
        _machinesRepositoryMock.Setup(x => x.EditMachine(It.IsAny<Machine>()))
            .ReturnsAsync(true);

        var result = await _machinesService.EditMachine(machineEditRequest);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task EditMachineShouldNotEditMachineWhenMachineNameIsTooLong()
    {
        var machineEditRequest = _fixture.Build<MachineEditRequest>()
            .With(x => x.NewMachineName, new string('A', 1001)).Create();
        _machinesRepositoryMock.Setup(x => x.EditMachine(It.IsAny<Machine>()))
            .ReturnsAsync(true);

        var result = await _machinesService.EditMachine(machineEditRequest);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task EditMachineShouldNotEditMachineWhenMachineNotExists()
    {
        var machineEditRequest = _fixture.Build<MachineEditRequest>()
            .With(x => x.NewMachineName, new string('A', 1001)).Create();
        _machinesRepositoryMock.Setup(x => x.EditMachine(It.IsAny<Machine>()))
            .ReturnsAsync(false);

        var result = await _machinesService.EditMachine(machineEditRequest);

        result.Should().BeFalse();
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