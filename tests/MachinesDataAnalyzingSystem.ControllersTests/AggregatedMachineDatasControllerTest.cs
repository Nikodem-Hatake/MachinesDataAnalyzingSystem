using AutoFixture;
using FluentAssertions;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;
using MachinesDataAnalyzingSystem.Shared.DTO.Machine;
using MachinesDataAnalyzingSystem.Shared.Enums;
using MachinesDataAnalyzingSystem.WebAPI.Controllers;
using Moq;

namespace MachinesDataAnalyzingSystem.ControllersTests;

public class AggregatedMachineDatasControllerTest
{
    private readonly AggregatedMachineDatasController _aggregatedMachineDatasController;
    private readonly Mock<IAggregatedMachineDatasService> _aggregatedMachineDatasServiceMock = new();
    private readonly Fixture _fixture = new();
    private readonly Mock<IMachinesService> _machineServiceMock = new();

    public AggregatedMachineDatasControllerTest()
    {
        _aggregatedMachineDatasController = new(_aggregatedMachineDatasServiceMock.Object, _machineServiceMock.Object);
    }

    #region GetAggregatedMachineDatas

    [Fact]
    public async Task GetAggregatedMachineDatasShouldReturnCorrectAggregatedMachineDatasForCorrectRequest()
    {
        var aggregatedMachineDatas = _fixture.Create<List<AggregatedMachineDataDTO>>();
        _aggregatedMachineDatasServiceMock.Setup(x => x.GetAggregatedMachineDatas(It.IsAny<Guid>(), It.IsAny<DateTime>(),
            It.IsAny<HowLongIsAggregatedMachineData>(), It.IsAny<int>()))
            .ReturnsAsync(aggregatedMachineDatas);

        var response = await _aggregatedMachineDatasController.GetAggregatedMachineDatas(Guid.NewGuid(), DateTime.UtcNow.AddYears(-1),
            aggregatedMachineDatas.First().HowLongIsAggregatedMachineData, aggregatedMachineDatas.Count);

        response.Value.Should().BeEquivalentTo(aggregatedMachineDatas);
    }

    #endregion
    #region GetAggregatedMachineDatasForAllMachines

    [Fact]
    public async Task GetAggregatedMachineDatasForAllMachinesShouldReturnCorrectAggregatedMachineDatasForCorrectRequest()
    {
        var aggregatedMachineDatas = _fixture.Create<List<AggregatedMachineDataDTO>>();
        var machines = _fixture.Create<List<MachineResponse>>();
        _aggregatedMachineDatasServiceMock.Setup(x => x.GetAggregatedMachineDatas(It.IsAny<Guid>(), It.IsAny<DateTime>(),
            It.IsAny<HowLongIsAggregatedMachineData>(), It.IsAny<int>()))
            .ReturnsAsync(aggregatedMachineDatas);
        _machineServiceMock.Setup(x => x.GetMachines())
            .ReturnsAsync(machines);

        var response = await _aggregatedMachineDatasController.GetAggregatedMachineDatasForAllMachines(DateTime.UtcNow.AddYears(-1),
            aggregatedMachineDatas.First().HowLongIsAggregatedMachineData, aggregatedMachineDatas.Count);

        response.Value.Count.Should().Be(machines.Count);
        response.Value.ForEach(x =>
        {
            machines.FirstOrDefault(y => x.MachineName == y.MachineName).Should().NotBeNull();
            x.AggregatedMachineDataDTOS.Should().BeEquivalentTo(aggregatedMachineDatas);
        });
    }

    #endregion
}