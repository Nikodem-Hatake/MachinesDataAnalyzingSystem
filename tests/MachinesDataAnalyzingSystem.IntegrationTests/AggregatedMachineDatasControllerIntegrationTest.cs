using AutoFixture;
using FluentAssertions;
using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Shared;
using MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;
using MachinesDataAnalyzingSystem.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace MachinesDataAnalyzingSystem.IntegrationTests;

public class AggregatedMachineDatasControllerIntegrationTest : BaseIntegrationTest
{
    private const int HOW_MANY_MACHINE_DATAS_FOR_EACH_MACHINE = 7;
    private const string NOT_EXISTING_TEST_DATE_TIME_IN_MACHINE_DATAS_RECORDS = "12.12.2000 00:00";
    private const string VALID_TEST_DATE_TIME = "01.01.2000 12:00";
    private const string VALID_TEST_DATE_TIME_FORMAT = "dd.MM.yyyy HH:mm";

    public AggregatedMachineDatasControllerIntegrationTest(WebAPIWebApplicationFactory webAPIWebApplicationFactory) 
        : base(webAPIWebApplicationFactory)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        dbContext.Machines.AddRange(_fixture.Create<List<Machine>>());
        dbContext.SaveChanges();
    }

    private async Task AddMachineDatas(HowLongIsAggregatedMachineData howLong)
    {
        int howManyMinutesToAdd = howLong switch
        {
            HowLongIsAggregatedMachineData.TenMinutes => 10,
            HowLongIsAggregatedMachineData.OneHour => 60,
            HowLongIsAggregatedMachineData.OneDay => 1440,
            HowLongIsAggregatedMachineData.OneWeek => 10080,
            HowLongIsAggregatedMachineData.OneMonth => 50000,
            _ => 0
        };

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        foreach(var machine in await dbContext.Machines.ToListAsync())
        {
            for(int i = 0; i < HOW_MANY_MACHINE_DATAS_FOR_EACH_MACHINE; ++i)
            {
                dbContext.MachineDatas.Add(_fixture.Build<MachineData>()
                    .With(x => x.DateTime, DateTime.ParseExact(VALID_TEST_DATE_TIME, VALID_TEST_DATE_TIME_FORMAT, null)
                        .AddMinutes(i * howManyMinutesToAdd))
                    .With(x => x.MachineId, machine.Id)
                    .Create());
            }
        }
        await dbContext.SaveChangesAsync();
    }

    #region GetAggregatedMachineDatas

    [Fact]
    public async Task GetAggregatedMachineDatasShouldReturnAggregatedMachineDatasForCorrectRequestWithEveryHowLongValueAndStoreItInDatabase()
    {
        var JWTToken = (await GetValidTestUserAuthenticationResponse()).JWTToken;
        Guid machineId;
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            machineId = await dbContext.Machines.Select(x => x.Id).FirstOrDefaultAsync();
        }

        foreach(var howLong in Enum.GetValues<HowLongIsAggregatedMachineData>())
        {
            await AddMachineDatas(howLong);
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/aggregatedMachineDatas/{machineId}/{DateTime.ParseExact
                (VALID_TEST_DATE_TIME, VALID_TEST_DATE_TIME_FORMAT, null).ToString
                (GlobalConstants.DATE_TIME_FORMAT_TO_SEND_IN_HTTP_REQUEST)}/{howLong}/{HOW_MANY_MACHINE_DATAS_FOR_EACH_MACHINE}");
            request.Headers.Authorization = new("Bearer", JWTToken);

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            var aggregatedMachineDatas = await response.Content.ReadFromJsonAsync<List<AggregatedMachineDataDTO>>();
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var aggregatedMachineDatasFromDatabase = (await dbContext.AggregatedMachineDatas.Where
                (x => x.MachineId == machineId && x.HowLongIsAggregatedMachineData == howLong).ToListAsync())
                .Select(x => x.ToDTO()).ToList();
            aggregatedMachineDatas.Count.Should().Be(aggregatedMachineDatasFromDatabase.Count);
            aggregatedMachineDatas.Should().BeEquivalentTo(aggregatedMachineDatasFromDatabase);
        }
    }

    [Fact]
    public async Task GetAggregatedMachineDatasShouldReturnAggregatedMachineDatasWithEveryHowLongValueWhenItAlreadyExistsWithoutDuplicating()
    {
        var JWTToken = (await GetValidTestUserAuthenticationResponse()).JWTToken;
        Guid machineId;
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            machineId = await dbContext.Machines.Select(x => x.Id).FirstOrDefaultAsync();
        }

        foreach(var howLong in Enum.GetValues<HowLongIsAggregatedMachineData>())
        {
            await AddMachineDatas(howLong);
            var endpoint = $"/api/aggregatedMachineDatas/{machineId}/{DateTime.ParseExact
                (VALID_TEST_DATE_TIME, VALID_TEST_DATE_TIME_FORMAT, null).ToString
                (GlobalConstants.DATE_TIME_FORMAT_TO_SEND_IN_HTTP_REQUEST)}/{howLong}/{HOW_MANY_MACHINE_DATAS_FOR_EACH_MACHINE}";
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Authorization = new("Bearer", JWTToken);
            var secondRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
            secondRequest.Headers.Authorization = new("Bearer", JWTToken);

            var response = await _client.SendAsync(request);
            var secondResponse = await _client.SendAsync(secondRequest);

            response.EnsureSuccessStatusCode();
            secondResponse.EnsureSuccessStatusCode();
            var aggregatedMachineDatas = await response.Content.ReadFromJsonAsync<List<AggregatedMachineDataDTO>>();
            var secondAggregatedMachineDatas = await secondResponse.Content.ReadFromJsonAsync<List<AggregatedMachineDataDTO>>();
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var aggregatedMachineDatasFromDatabase = (await dbContext.AggregatedMachineDatas.Where
                (x => x.MachineId == machineId && x.HowLongIsAggregatedMachineData == howLong).ToListAsync())
                .Select(x => x.ToDTO()).ToList();
            aggregatedMachineDatas.Count.Should().Be(aggregatedMachineDatasFromDatabase.Count);
            aggregatedMachineDatas.Should().BeEquivalentTo(aggregatedMachineDatasFromDatabase);
            aggregatedMachineDatas.Should().BeEquivalentTo(secondAggregatedMachineDatas);
        }
    }

    [Fact]
    public async Task GetAggregatedMachineDatasShouldReturnEmptyArrayForInvalidMachineId()
    {
        var JWTToken = (await GetValidTestUserAuthenticationResponse()).JWTToken;
        await AddMachineDatas(HowLongIsAggregatedMachineData.TenMinutes);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/aggregatedMachineDatas/{Guid.NewGuid()}/{DateTime.ParseExact
            (VALID_TEST_DATE_TIME, VALID_TEST_DATE_TIME_FORMAT, null).ToString
            (GlobalConstants.DATE_TIME_FORMAT_TO_SEND_IN_HTTP_REQUEST)}/{HowLongIsAggregatedMachineData.TenMinutes}/{
            HOW_MANY_MACHINE_DATAS_FOR_EACH_MACHINE}");
        request.Headers.Authorization = new("Bearer", JWTToken);

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var aggregatedMachineDatas = await response.Content.ReadFromJsonAsync<List<AggregatedMachineDataDTO>>();
        aggregatedMachineDatas.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAggregatedMachineDatasShouldReturnNotFoundStatucCodeForInvalidDateTime()
    {
        var JWTToken = (await GetValidTestUserAuthenticationResponse()).JWTToken;
        Guid machineId;
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            machineId = await dbContext.Machines.Select(x => x.Id).FirstOrDefaultAsync();
        }
        await AddMachineDatas(HowLongIsAggregatedMachineData.TenMinutes);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/aggregatedMachineDatas/{machineId}/{DateTime.UtcNow.ToString()}/{
            HowLongIsAggregatedMachineData.TenMinutes}/{HOW_MANY_MACHINE_DATAS_FOR_EACH_MACHINE}");
        request.Headers.Authorization = new("Bearer", JWTToken);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAggregatedMachineDatasShouldReturnEmptyArrayForDateTimeThatIsNotFoundInMachineDatasRecords()
    {
        var JWTToken = (await GetValidTestUserAuthenticationResponse()).JWTToken;
        Guid machineId;
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            machineId = await dbContext.Machines.Select(x => x.Id).FirstOrDefaultAsync();
        }
        await AddMachineDatas(HowLongIsAggregatedMachineData.TenMinutes);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/aggregatedMachineDatas/{machineId}/{DateTime.ParseExact
            (NOT_EXISTING_TEST_DATE_TIME_IN_MACHINE_DATAS_RECORDS, VALID_TEST_DATE_TIME_FORMAT, null).ToString
            (GlobalConstants.DATE_TIME_FORMAT_TO_SEND_IN_HTTP_REQUEST)}/{HowLongIsAggregatedMachineData.TenMinutes}/{
            HOW_MANY_MACHINE_DATAS_FOR_EACH_MACHINE}");
        request.Headers.Authorization = new("Bearer", JWTToken);

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var aggregatedMachineDatas = await response.Content.ReadFromJsonAsync<List<AggregatedMachineDataDTO>>();
        aggregatedMachineDatas.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAggregatedMachineDatasShouldReturnBadRequestStatucCodeForInvalidHowLongValue()
    {
        var JWTToken = (await GetValidTestUserAuthenticationResponse()).JWTToken;
        Guid machineId;
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            machineId = await dbContext.Machines.Select(x => x.Id).FirstOrDefaultAsync();
        }
        await AddMachineDatas(HowLongIsAggregatedMachineData.TenMinutes);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/aggregatedMachineDatas/{machineId}/{DateTime.ParseExact
            (VALID_TEST_DATE_TIME, VALID_TEST_DATE_TIME_FORMAT, null).ToString
            (GlobalConstants.DATE_TIME_FORMAT_TO_SEND_IN_HTTP_REQUEST)}/{-1}/{HOW_MANY_MACHINE_DATAS_FOR_EACH_MACHINE}");
        request.Headers.Authorization = new("Bearer", JWTToken);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [InlineData(0)]
    [InlineData(11)]
    [Theory]
    public async Task GetAggregatedMachineDatasShouldReturnBadRequestStatusCodeForInvalidHowManyValue(int howMany)
    {
        var JWTToken = (await GetValidTestUserAuthenticationResponse()).JWTToken;
        Guid machineId;
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            machineId = await dbContext.Machines.Select(x => x.Id).FirstOrDefaultAsync();
        }
        await AddMachineDatas(HowLongIsAggregatedMachineData.TenMinutes);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/aggregatedMachineDatas/{machineId}/{DateTime.ParseExact
            (VALID_TEST_DATE_TIME, VALID_TEST_DATE_TIME_FORMAT, null).ToString
            (GlobalConstants.DATE_TIME_FORMAT_TO_SEND_IN_HTTP_REQUEST)}/{HowLongIsAggregatedMachineData.TenMinutes}/{0}");
        request.Headers.Authorization = new("Bearer", JWTToken);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAggregatedMachineDatasShouldReturnUnauthorizedStatusCodeForInvalidJWTToken()
    {
        Guid machineId;
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            machineId = await dbContext.Machines.Select(x => x.Id).FirstOrDefaultAsync();
        }
        await AddMachineDatas(HowLongIsAggregatedMachineData.TenMinutes);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/aggregatedMachineDatas/{machineId}/{DateTime.ParseExact
            (VALID_TEST_DATE_TIME, VALID_TEST_DATE_TIME_FORMAT, null).ToString
            (GlobalConstants.DATE_TIME_FORMAT_TO_SEND_IN_HTTP_REQUEST)}/{HowLongIsAggregatedMachineData.TenMinutes}/{
            HOW_MANY_MACHINE_DATAS_FOR_EACH_MACHINE}");
        request.Headers.Authorization = new("Bearer", "");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
    #region GetAggregatedMachineDatasForAllMachines

    [Fact]
    public async Task GetAggregatedMachineDatasForAllMachinesShouldReturnAggregatedMachineDatasForCorrectRequestWithEveryHowLongValueAndStoreItInDatabase()
    {
        var JWTToken = (await GetValidTestUserAuthenticationResponse()).JWTToken;
        foreach(var howLong in Enum.GetValues<HowLongIsAggregatedMachineData>())
        {
            await AddMachineDatas(howLong);
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/aggregatedMachineDatas/{DateTime.ParseExact
                (VALID_TEST_DATE_TIME, VALID_TEST_DATE_TIME_FORMAT, null).ToString
                (GlobalConstants.DATE_TIME_FORMAT_TO_SEND_IN_HTTP_REQUEST)}/{howLong}/{HOW_MANY_MACHINE_DATAS_FOR_EACH_MACHINE}");
            request.Headers.Authorization = new("Bearer", JWTToken);

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            var aggregatedMachineDatasForEveryMachine = await response.Content.ReadFromJsonAsync<List<AggregatedMachineDatasForMachineDTO>>();
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var howManyAggregatedMachineDatasInDatabase = await dbContext.AggregatedMachineDatas
                .CountAsync(x => x.HowLongIsAggregatedMachineData == howLong);
            aggregatedMachineDatasForEveryMachine.Sum(x => x.AggregatedMachineDataDTOS.Count)
                .Should().Be(howManyAggregatedMachineDatasInDatabase);
            foreach(var aggregatedMachineDatasForMachine in aggregatedMachineDatasForEveryMachine)
            {
                var aggregatedMachineDatasFromDatabase = (await dbContext.AggregatedMachineDatas.Where(x =>
                    x.MachineId == aggregatedMachineDatasForMachine.AggregatedMachineDataDTOS.First().MachineId
                    && x.HowLongIsAggregatedMachineData == howLong).ToListAsync()).Select(x => x.ToDTO()).ToList();
                aggregatedMachineDatasForMachine.AggregatedMachineDataDTOS.Should().BeEquivalentTo(aggregatedMachineDatasFromDatabase);
            }
        }
    }

    [Fact]
    public async Task GetAggregatedMachineDatasForAllMachinesShouldReturnNotFoundStatusCodeForInvalidDateTime()
    {
        var JWTToken = (await GetValidTestUserAuthenticationResponse()).JWTToken;
        await AddMachineDatas(HowLongIsAggregatedMachineData.TenMinutes);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/aggregatedMachineDatas/{DateTime.UtcNow.ToString()}/{
            HowLongIsAggregatedMachineData.TenMinutes}/{HOW_MANY_MACHINE_DATAS_FOR_EACH_MACHINE}");
        request.Headers.Authorization = new("Bearer", JWTToken);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAggregatedMachineDatasForAllMachinesShouldReturnEmptyArrayForDateTimeNotFoundInMachineDatasRecords()
    {
        var JWTToken = (await GetValidTestUserAuthenticationResponse()).JWTToken;
        await AddMachineDatas(HowLongIsAggregatedMachineData.TenMinutes);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/aggregatedMachineDatas/{DateTime.ParseExact
            (NOT_EXISTING_TEST_DATE_TIME_IN_MACHINE_DATAS_RECORDS, VALID_TEST_DATE_TIME_FORMAT, null).ToString
            (GlobalConstants.DATE_TIME_FORMAT_TO_SEND_IN_HTTP_REQUEST)}/{HowLongIsAggregatedMachineData.TenMinutes}/{
            HOW_MANY_MACHINE_DATAS_FOR_EACH_MACHINE}");
        request.Headers.Authorization = new("Bearer", JWTToken);

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var aggregatedMachineDatas = await response.Content.ReadFromJsonAsync<List<AggregatedMachineDatasForMachineDTO>>();
        aggregatedMachineDatas.Sum(x => x.AggregatedMachineDataDTOS.Count).Should().Be(0);
    }

    [Fact]
    public async Task GetAggregatedMachineDatasForAllMachinesShouldReturnBadRequestStatusCodeForInvalidHowLongValue()
    {
        var JWTToken = (await GetValidTestUserAuthenticationResponse()).JWTToken;
        await AddMachineDatas(HowLongIsAggregatedMachineData.TenMinutes);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/aggregatedMachineDatas/{DateTime.ParseExact
            (VALID_TEST_DATE_TIME, VALID_TEST_DATE_TIME_FORMAT, null).ToString
            (GlobalConstants.DATE_TIME_FORMAT_TO_SEND_IN_HTTP_REQUEST)}/{-1}/{HOW_MANY_MACHINE_DATAS_FOR_EACH_MACHINE}");
        request.Headers.Authorization = new("Bearer", JWTToken);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [InlineData(0)]
    [InlineData(8)]
    [Theory]
    public async Task GetAggregatedMachineDatasForAllMachinesShouldReturnBadRequestStatusCodeForInvalidHowManyValue(int howMany)
    {
        var JWTToken = (await GetValidTestUserAuthenticationResponse()).JWTToken;
        await AddMachineDatas(HowLongIsAggregatedMachineData.TenMinutes);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/aggregatedMachineDatas/{DateTime.ParseExact
            (VALID_TEST_DATE_TIME, VALID_TEST_DATE_TIME_FORMAT, null).ToString
            (GlobalConstants.DATE_TIME_FORMAT_TO_SEND_IN_HTTP_REQUEST)}/{HowLongIsAggregatedMachineData.TenMinutes}/{howMany}");
        request.Headers.Authorization = new("Bearer", JWTToken);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAggregatedMachineDatasForAllMachinesShouldReturnUnauthorizedStatusCodeForInvalidJWTToken()
    {
        await AddMachineDatas(HowLongIsAggregatedMachineData.TenMinutes);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/aggregatedMachineDatas/{DateTime.ParseExact
            (VALID_TEST_DATE_TIME, VALID_TEST_DATE_TIME_FORMAT, null).ToString
            (GlobalConstants.DATE_TIME_FORMAT_TO_SEND_IN_HTTP_REQUEST)}/{HowLongIsAggregatedMachineData.TenMinutes}/{
            HOW_MANY_MACHINE_DATAS_FOR_EACH_MACHINE}");
        request.Headers.Authorization = new("Bearer", "");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
}