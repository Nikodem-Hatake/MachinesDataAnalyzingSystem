using AutoFixture;
using FluentAssertions;
using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Shared;
using MachinesDataAnalyzingSystem.Shared.DTO.MachineDatas;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace MachinesDataAnalyzingSystem.IntegrationTests;

public class MachineDatasControllerIntegrationTest(WebAPIWebApplicationFactory webAPIWebApplicationFactory)
    : BaseIntegrationTest(webAPIWebApplicationFactory)
{
    private async Task AddMachineDatasToDatabase()
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.MachineDatas.AddRangeAsync(_fixture.Create<List<MachineData>>());
        await dbContext.SaveChangesAsync();
    }

    #region GetMachineDatasCount

    [Fact]
    public async Task GetMachineDatasCountShouldReturnMachineDatasCountForValidRequest()
    {
        await AddMachineDatasToDatabase();
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/machineDatas");
        request.Headers.Authorization = new("Bearer", (await GetValidTestAdminAuthenticationResponse()).JWTToken);

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var countResponse = await response.Content.ReadFromJsonAsync<MachineDatasCountResponse>();
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        countResponse.Count.Should().Be(await dbContext.MachineDatas.LongCountAsync());
    }

    [Fact]
    public async Task GetMachineDatasCountShouldReturnForbiddenStatusCodeForNonAdminUser()
    {
        await AddMachineDatasToDatabase();
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/machineDatas");
        request.Headers.Authorization = new("Bearer", (await GetValidTestUserAuthenticationResponse()).JWTToken);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetMachineDatasCountShouldReturnUnauthorizedStatusCodeForInvalidJWTToken()
    {
        await AddMachineDatasToDatabase();
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/machineDatas");
        request.Headers.Authorization = new("Bearer", "");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
    #region PostMachineData

    [Fact]
    public async Task PostMachineDataShouldAddMachineDataForValidRequest()
    {
        await AddMachineDatasToDatabase();
        Guid machineId;
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            machineId = (await dbContext.Machines.FirstAsync()).Id;
        }
        var machineDataSended = _fixture.Build<MachineDataDTO>()
            .With(x => x.MachineId, machineId)
            .Create();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/machineDatas");
        request.Content = JsonContent.Create<MachineDataDTO>(machineDataSended);
        request.Headers.Add(GlobalConstants.API_KEY_HEADER_NAME, ValidAPIKey);

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var machineDatasInDatabase = await dbContext.MachineDatas.Where(x => x.MachineId == machineDataSended.MachineId
                && x.DateTime == machineDataSended.DateTime).ToListAsync();
            machineDatasInDatabase.Count.Should().Be(1);
        }
    }

    [Fact]
    public async Task PostMachineDataShouldReturnBadRequestStatusCodeWithoutAddingMachineDataForInvalidMachineId()
    {
        await AddMachineDatasToDatabase();
        var machineDataSended = _fixture.Create<MachineDataDTO>();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/machineDatas");
        request.Content = JsonContent.Create<MachineDataDTO>(machineDataSended);
        request.Headers.Add(GlobalConstants.API_KEY_HEADER_NAME, ValidAPIKey);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var machineDatasInDatabase = await dbContext.MachineDatas.Where(x => x.MachineId == machineDataSended.MachineId
                && x.DateTime == machineDataSended.DateTime).ToListAsync();
            machineDatasInDatabase.Count.Should().Be(0);
        }
    }

    [Fact]
    public async Task PostMachineDataShouldReturnUnsupportedMediaTypeStatusCodeWithoutAddingMachineDataForEmptyBody()
    {
        await AddMachineDatasToDatabase();
        var machineDataSended = _fixture.Create<MachineDataDTO>();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/machineDatas");
        request.Headers.Add(GlobalConstants.API_KEY_HEADER_NAME, ValidAPIKey);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var machineDatasInDatabase = await dbContext.MachineDatas.Where(x => x.MachineId == machineDataSended.MachineId
                && x.DateTime == machineDataSended.DateTime).ToListAsync();
            machineDatasInDatabase.Count.Should().Be(0);
        }
    }

    [Fact]
    public async Task PostMachineDataShouldReturnUnauthorizedStatusCodeWithoutAddingMachineDataForInvalidAPIKey()
    {
        await AddMachineDatasToDatabase();
        Guid machineId;
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            machineId = (await dbContext.Machines.FirstAsync()).Id;
        }
        var machineDataSended = _fixture.Build<MachineDataDTO>()
            .With(x => x.MachineId, machineId)
            .Create();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/machineDatas");
        request.Content = JsonContent.Create<MachineDataDTO>(machineDataSended);
        request.Headers.Add(GlobalConstants.API_KEY_HEADER_NAME, "");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var machineDatasInDatabase = await dbContext.MachineDatas.Where(x => x.MachineId == machineDataSended.MachineId
                && x.DateTime == machineDataSended.DateTime).ToListAsync();
            machineDatasInDatabase.Count.Should().Be(0);
        }
    }

    [Fact]
    public async Task PostMachineDataShouldReturnUnauthorizedStatusCodeWithoutAddingMachineDataForNotSendingAPIKeyHeader()
    {
        await AddMachineDatasToDatabase();
        Guid machineId;
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            machineId = (await dbContext.Machines.FirstAsync()).Id;
        }
        var machineDataSended = _fixture.Build<MachineDataDTO>()
            .With(x => x.MachineId, machineId)
            .Create();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/machineDatas");
        request.Content = JsonContent.Create<MachineDataDTO>(machineDataSended);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var machineDatasInDatabase = await dbContext.MachineDatas.Where(x => x.MachineId == machineDataSended.MachineId
                && x.DateTime == machineDataSended.DateTime).ToListAsync();
            machineDatasInDatabase.Count.Should().Be(0);
        }
    }

    #endregion
}