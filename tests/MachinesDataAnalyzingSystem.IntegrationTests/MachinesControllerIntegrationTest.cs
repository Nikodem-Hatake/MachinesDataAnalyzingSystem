using AutoFixture;
using FluentAssertions;
using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Shared;
using MachinesDataAnalyzingSystem.Shared.DTO.Machine;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace MachinesDataAnalyzingSystem.IntegrationTests;

public class MachinesControllerIntegrationTest(WebAPIWebApplicationFactory webAPIWebApplicationFactory)
    : BaseIntegrationTest(webAPIWebApplicationFactory)
{
    private async Task<Guid> AddMachineToDatabase()
    {
        var machine = _fixture.Create<Machine>();
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        if(await dbContext.Machines.ContainsAsync(machine))
        {
            return machine.Id;
        }

        await dbContext.Machines.AddAsync(machine);
        await dbContext.SaveChangesAsync();
        return machine.Id;
    }

    #region DeleteMachine

    [Fact]
    public async Task DeleteMachineShouldDeleteMachineForValidRequest()
    {
        var machineId = await AddMachineToDatabase();
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/machines");
        request.Headers.Authorization = new("Bearer", (await GetValidTestAdminAuthenticationResponse()).JWTToken);
        request.Content = JsonContent.Create<MachineDeleteRequest>(_fixture.Build<MachineDeleteRequest>()
            .With(x => x.MachineId, machineId)
            .Create());

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        (await dbContext.Machines.Where(x => x.Id == machineId).CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task DeleteMachineShouldReturnBadRequestStatusCodeForNotExistingMachineId()
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/machines");
        request.Headers.Authorization = new("Bearer", (await GetValidTestAdminAuthenticationResponse()).JWTToken);
        request.Content = JsonContent.Create<MachineDeleteRequest>(_fixture.Create<MachineDeleteRequest>());

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteMachineShouldReturnUnsupportedMediaTypeStatusCodeForNotExistingBody()
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/machines");
        request.Headers.Authorization = new("Bearer", (await GetValidTestAdminAuthenticationResponse()).JWTToken);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    public async Task DeleteMachineShouldReturnUnauthorizedStatusCodeForInvalidJWTToken()
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/machines");
        request.Content = JsonContent.Create<MachineDeleteRequest>(_fixture.Create<MachineDeleteRequest>());
        request.Headers.Authorization = new("Bearer", "");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteMachineShouldReturnForbiddenStatusCodeForJWTTokenWithoutAdminRole()
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/machines");
        request.Content = JsonContent.Create<MachineDeleteRequest>(_fixture.Create<MachineDeleteRequest>());
        request.Headers.Authorization = new("Bearer", (await GetValidTestUserAuthenticationResponse()).JWTToken);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion
    #region GetMachines

    [Fact]
    public async Task GetMachinesShouldReturnMachinesForValidRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/machines");
        request.Headers.Authorization = new("Bearer", (await GetValidTestUserAuthenticationResponse()).JWTToken);

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var machines = await response.Content.ReadFromJsonAsync<List<MachineResponse>>();
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        machines.Should().BeEquivalentTo((await dbContext.Machines.ToListAsync()).Select(x => x.ToDTO()).ToList());
    }

    [Fact]
    public async Task GetMachinesShouldReturnUnauthorizedStatusCodeForInvalidJWTToken()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/machines");
        request.Headers.Authorization = new("Bearer", "");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
    #region GetMachinesForService

    [Fact]
    public async Task GetMachinesForServiceShouldReturnMachinesForValidRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/machines/forService");
        request.Headers.Add(GlobalConstants.API_KEY_HEADER_NAME, ValidAPIKey);

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var machines = await response.Content.ReadFromJsonAsync<List<MachineResponse>>();
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        machines.Should().BeEquivalentTo((await dbContext.Machines.ToListAsync()).Select(x => x.ToDTO()).ToList());
    }

    [Fact]
    public async Task GetMachinesForServiceShouldReturnUnauthorizedStatusCodeForInvalidAPIKey()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/machines/forService");
        request.Headers.Add(GlobalConstants.API_KEY_HEADER_NAME, "");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMachinesForServiceShouldReturnUnauthorizedStatusCodeForNotSendingAPIKey()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/machines/forService");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
    #region PostMachine

    [Fact]
    public async Task PostMachineShouldAddMachineForValidRequest()
    {
        var machineAddRequest = _fixture.Create<MachineAddRequest>();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/machines");
        request.Headers.Authorization = new("Bearer", (await GetValidTestAdminAuthenticationResponse()).JWTToken);
        request.Content = JsonContent.Create<MachineAddRequest>(machineAddRequest);

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        (await dbContext.Machines.FirstOrDefaultAsync(x => x.Name == machineAddRequest.MachineName)).Should().NotBeNull();
    }

    [Fact]
    public async Task PostMachineShouldReturnBadRequestForEmptyMachineNameWithoutAddingMachine()
    {
        var machineAddRequest = _fixture.Build<MachineAddRequest>()
            .With(x => x.MachineName, "")
            .Create();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/machines");
        request.Headers.Authorization = new("Bearer", (await GetValidTestAdminAuthenticationResponse()).JWTToken);
        request.Content = JsonContent.Create<MachineAddRequest>(machineAddRequest);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        (await dbContext.Machines.FirstOrDefaultAsync(x => x.Name == machineAddRequest.MachineName)).Should().BeNull();
    }

    [Fact]
    public async Task PostMachineShouldReturnBadRequestForNullMachineNameWithoutAddingMachine()
    {
        var machineAddRequest = _fixture.Build<MachineAddRequest>()
            .With(x => x.MachineName, value: null)
            .Create();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/machines");
        request.Headers.Authorization = new("Bearer", (await GetValidTestAdminAuthenticationResponse()).JWTToken);
        request.Content = JsonContent.Create<MachineAddRequest>(machineAddRequest);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        (await dbContext.Machines.FirstOrDefaultAsync(x => x.Name == machineAddRequest.MachineName)).Should().BeNull();
    }

    [Fact]
    public async Task PostMachineShouldReturnBadRequestForTooLongMachineNameWithoutAddingMachine()
    {
        var machineAddRequest = _fixture.Build<MachineAddRequest>()
            .With(x => x.MachineName, new string('a', 101))
            .Create();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/machines");
        request.Headers.Authorization = new("Bearer", (await GetValidTestAdminAuthenticationResponse()).JWTToken);
        request.Content = JsonContent.Create<MachineAddRequest>(machineAddRequest);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        (await dbContext.Machines.FirstOrDefaultAsync(x => x.Name == machineAddRequest.MachineName)).Should().BeNull();
    }

    [Fact]
    public async Task PostMachineShouldReturnUnsupportedMediaTypeForEmptyBody()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/machines");
        request.Headers.Authorization = new("Bearer", (await GetValidTestAdminAuthenticationResponse()).JWTToken);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    public async Task PostMachineShouldReturnUnauthorizedStatusCodeForInvalidJWTTokenWithoutAddingMachine()
    {
        var machineAddRequest = _fixture.Create<MachineAddRequest>();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/machines");
        request.Headers.Authorization = new("Bearer", "");
        request.Content = JsonContent.Create<MachineAddRequest>(machineAddRequest);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        (await dbContext.Machines.FirstOrDefaultAsync(x => x.Name == machineAddRequest.MachineName)).Should().BeNull();
    }

    [Fact]
    public async Task PostMachineShouldReturnForbiddenStatusCodeForJWTTokenWithoutAdminRoleWithoutAddingMachine()
    {
        var machineAddRequest = _fixture.Create<MachineAddRequest>();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/machines");
        request.Headers.Authorization = new("Bearer", (await GetValidTestUserAuthenticationResponse()).JWTToken);
        request.Content = JsonContent.Create<MachineAddRequest>(machineAddRequest);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        (await dbContext.Machines.FirstOrDefaultAsync(x => x.Name == machineAddRequest.MachineName)).Should().BeNull();
    }

    #endregion
    #region PutMachine

    [Fact]
    public async Task PutMachineShouldEditMachineForValidRequest()
    {
        var machineId = await AddMachineToDatabase();
        var machineEditRequest = _fixture.Build<MachineEditRequest>()
            .With(x => x.MachineId, machineId)
            .Create();
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/machines");
        request.Headers.Authorization = new("Bearer", (await GetValidTestAdminAuthenticationResponse()).JWTToken);
        request.Content = JsonContent.Create(machineEditRequest);

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var machine = await dbContext.Machines.FirstAsync(x => x.Id == machineId);
        machine.Name.Should().Be(machineEditRequest.NewMachineName);
    }

    [Fact]
    public async Task PutMachineShouldReturnBadRequestStatusCodeForMachineIdNotFoundWithoutEditingMachine()
    {
        var machineId = await AddMachineToDatabase();
        var machineEditRequest = _fixture.Create<MachineEditRequest>();
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/machines");
        request.Headers.Authorization = new("Bearer", (await GetValidTestAdminAuthenticationResponse()).JWTToken);
        request.Content = JsonContent.Create(machineEditRequest);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var machine = await dbContext.Machines.FirstAsync(x => x.Id == machineId);
        machine.Name.Should().NotBe(machineEditRequest.NewMachineName);
    }

    [Fact]
    public async Task PutMachineShouldReturnUnsupportedMediaTypeStatusCodeForEmptyBody()
    {
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/machines");
        request.Headers.Authorization = new("Bearer", (await GetValidTestAdminAuthenticationResponse()).JWTToken);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    public async Task PutMachineShouldReturnUnauthorizedStatusCodeForInvalidJWTTokenWithoutEditingMachine()
    {
        var machineId = await AddMachineToDatabase();
        var machineEditRequest = _fixture.Create<MachineEditRequest>();
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/machines");
        request.Headers.Authorization = new("Bearer", "");
        request.Content = JsonContent.Create(machineEditRequest);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var machine = await dbContext.Machines.FirstAsync(x => x.Id == machineId);
        machine.Name.Should().NotBe(machineEditRequest.NewMachineName);
    }

    [Fact]
    public async Task PutMachineShouldReturnForbiddenStatusCodeForJWTTokenWithoutAdminRoleWithoutEditingMachine()
    {
        var machineId = await AddMachineToDatabase();
        var machineEditRequest = _fixture.Create<MachineEditRequest>();
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/machines");
        request.Headers.Authorization = new("Bearer", (await GetValidTestUserAuthenticationResponse()).JWTToken);
        request.Content = JsonContent.Create(machineEditRequest);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var machine = await dbContext.Machines.FirstAsync(x => x.Id == machineId);
        machine.Name.Should().NotBe(machineEditRequest.NewMachineName);
    }

    #endregion
}