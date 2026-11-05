using AutoFixture;
using FluentAssertions;
using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Core.Services;
using MachinesDataAnalyzingSystem.Shared.Enums;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using System.Text.Json;

namespace MachinesDataAnalyzingSystem.ServiceTests;

public class AggregatedMachineDatasServiceTest
{
    private readonly Mock<IAggregatedMachineDatasRepository> _aggregatedMachineDatasRepositoryMock = new();
    private readonly IAggregatedMachineDatasService _aggregatedMachineDatasService;
    private readonly Mock<IDistributedCache> _distributedCacheMock = new();
    private readonly Fixture _fixture = new();
    private readonly Mock<IMachineDatasRepository> _machineDatasRepositoryMock = new();

    public AggregatedMachineDatasServiceTest()
    {
        _aggregatedMachineDatasService = new AggregatedMachineDatasService(_aggregatedMachineDatasRepositoryMock.Object,
            _machineDatasRepositoryMock.Object, _distributedCacheMock.Object, new Mock<ILogger<AggregatedMachineDatasService>>().Object);
    }

    #region GetAggregatedMachineDatas

    [Fact]
    public async Task GetAggregatedMachineDataShouldAggregateMachineDatasAndReturnThemWhenNoneIsFoundInStorage()
    {
        var machineData = _fixture.Build<MachineData>()
            .With(x => x.Id, Guid.NewGuid())
            .With(x => x.DateTime, DateTime.UtcNow.AddMinutes(-(DateTime.UtcNow.Minute % 10)))
            .Create();
        var machineDatas = new List<MachineData>() { machineData, machineData, machineData, machineData, machineData };
        _distributedCacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: null);
        _aggregatedMachineDatasRepositoryMock.Setup(x => x.GetAggregatedMachineData(It.IsAny<Guid>(), It.IsAny<DateTime>(),
            It.IsAny<HowLongIsAggregatedMachineData>())).ReturnsAsync(value: null);
        _machineDatasRepositoryMock.Setup(x => x.GetMachineDatasForAggregation(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(machineDatas);

        var aggregatedMachineDatasFromGetData = await _aggregatedMachineDatasService.GetAggregatedMachineDatas
            (machineData.MachineId, machineData.DateTime.AddMinutes(-10), HowLongIsAggregatedMachineData.TenMinutes, 1);

        aggregatedMachineDatasFromGetData.Count.Should().Be(1);
        aggregatedMachineDatasFromGetData[0].AverageMotorsSpeed.Should().BeApproximately(machineData.AverageMotorsSpeed, 2);
        aggregatedMachineDatasFromGetData[0].AverageTemperature.Should().BeApproximately(machineData.AverageTemperature, 2);
        aggregatedMachineDatasFromGetData[0].MaxTemperature.Should().Be(machineData.AverageTemperature);
        aggregatedMachineDatasFromGetData[0].MinTemperature.Should().Be(machineData.AverageTemperature);
        aggregatedMachineDatasFromGetData[0].HowLongIsAggregatedMachineData.Should().Be(HowLongIsAggregatedMachineData.TenMinutes);
        aggregatedMachineDatasFromGetData[0].MachineId.Should().Be(machineData.MachineId);
        aggregatedMachineDatasFromGetData[0].TotalNumberOfProcessedResources.Should().Be(machineDatas.Sum(x => x.NumberOfProcessedResources));
        aggregatedMachineDatasFromGetData[0].TotalNumberOfSecondsToProcessResources.Should()
            .BeApproximately(machineDatas.Sum(x => x.NumberOfSecondsToProcessResources), 2);
    }

    [Fact]
    public async Task GetAggregatedMachineDataShouldReturnAggregatedMachineDataWhenItExistsInCache()
    {
        var aggregatedMachineData = _fixture.Create<AggregatedMachineData>();
        aggregatedMachineData.StartDate = DateTime.UtcNow.AddYears(-1);
        _distributedCacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(aggregatedMachineData)));
        _aggregatedMachineDatasRepositoryMock.Setup(x => x.GetAggregatedMachineData(It.IsAny<Guid>(), It.IsAny<DateTime>(), 
            It.IsAny<HowLongIsAggregatedMachineData>())).ReturnsAsync(aggregatedMachineData);

        var aggregatedMachineDatasFromGetData = await _aggregatedMachineDatasService.GetAggregatedMachineDatas
            (aggregatedMachineData.MachineId, aggregatedMachineData.StartDate, aggregatedMachineData.HowLongIsAggregatedMachineData, 1);

        aggregatedMachineDatasFromGetData.Count.Should().Be(1);
        aggregatedMachineData.Equals(aggregatedMachineDatasFromGetData[0]).Should().BeTrue();
    }

    [Fact]
    public async Task GetAggregatedMachineDataShouldReturnAggregatedMachineDataWhenItExistsInDatabase()
    {
        var aggregatedMachineData = _fixture.Create<AggregatedMachineData>();
        aggregatedMachineData.StartDate = DateTime.UtcNow.AddYears(-1);
        _distributedCacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: null);
        _aggregatedMachineDatasRepositoryMock.Setup(x => x.GetAggregatedMachineData(It.IsAny<Guid>(), It.IsAny<DateTime>(),
            It.IsAny<HowLongIsAggregatedMachineData>())).ReturnsAsync(aggregatedMachineData);

        var aggregatedMachineDatasFromGetData = await _aggregatedMachineDatasService.GetAggregatedMachineDatas
            (aggregatedMachineData.MachineId, aggregatedMachineData.StartDate, aggregatedMachineData.HowLongIsAggregatedMachineData, 1);

        aggregatedMachineDatasFromGetData.Count.Should().Be(1);
        aggregatedMachineData.Equals(aggregatedMachineDatasFromGetData[0]).Should().BeTrue();
    }

    [Fact]
    public async Task GetAggregatedMachineDataShouldReturnEmptyListWhenTimeIsIncorrect()
    {
        foreach(var value in Enum.GetValues<HowLongIsAggregatedMachineData>())
        {
            var aggregatedMachineDatas = await _aggregatedMachineDatasService.GetAggregatedMachineDatas
                (_fixture.Create<Guid>(), DateTime.UtcNow, value, 1);

            aggregatedMachineDatas.Should().BeEmpty();
        }
    }

    #endregion
}