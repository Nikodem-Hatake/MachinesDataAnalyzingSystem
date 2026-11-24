using AutoFixture;
using MachinesDataAnalyzingSystem.Infrastructure;
using MachinesDataAnalyzingSystem.Shared.DTO.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace MachinesDataAnalyzingSystem.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<WebAPIWebApplicationFactory>
{
    protected const string VALID_TEST_ADMIN_EMAIL = "admin@example.com";
    protected const string VALID_TEST_ADMIN_PASSWORD = "Admin123.";
    protected const string VALID_TEST_USER_EMAIL = "user@example.com";
    protected const string VALID_TEST_USER_PASSWORD = "User123.";
    protected readonly Fixture _fixture = new();
    protected readonly string ValidAPIKey;

    protected HttpClient _client;
    protected readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public BaseIntegrationTest(WebAPIWebApplicationFactory webAPIWebApplicationFactory)
    {
        ValidAPIKey = webAPIWebApplicationFactory.ValidAPIKey;
        _client = webAPIWebApplicationFactory.CreateClient();
        var scope = webAPIWebApplicationFactory.Services.CreateScope();
        _dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    }

    private async Task AddAdminRoleToAdminAccount()
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.UserRoles.AddAsync(new Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>()
        {
            UserId = (await dbContext.Users.FirstAsync(x => x.Email == VALID_TEST_ADMIN_EMAIL)).Id,
            RoleId = (await dbContext.Roles.FirstAsync(x => x.NormalizedName == "ADMIN")).Id
        });
        await dbContext.SaveChangesAsync();
    }

    protected async Task<AuthenticationResponse> GetValidTestAdminAuthenticationResponse()
    {
        var response = await _client.PostAsJsonAsync<LoginRequest>("/api/accounts/Login", new LoginRequest()
        {
            Email = VALID_TEST_ADMIN_EMAIL,
            Password = VALID_TEST_ADMIN_PASSWORD
        });

        if(!response.IsSuccessStatusCode)
        {
            await _client.PostAsJsonAsync<RegisterRequest>("/api/accounts/Register", new RegisterRequest()
            {
                ConfirmPassword = VALID_TEST_ADMIN_PASSWORD,
                Email = VALID_TEST_ADMIN_EMAIL,
                Password = VALID_TEST_ADMIN_PASSWORD
            });
            await AddAdminRoleToAdminAccount();
            response = await _client.PostAsJsonAsync<LoginRequest>("/api/accounts/Login", new LoginRequest()
            {
                Email = VALID_TEST_ADMIN_EMAIL,
                Password = VALID_TEST_ADMIN_PASSWORD
            });
        }

        return await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
    }

    protected async Task<AuthenticationResponse> GetValidTestUserAuthenticationResponse()
    {
        var response = await _client.PostAsJsonAsync<LoginRequest>("/api/accounts/Login", new LoginRequest()
        {
            Email = VALID_TEST_USER_EMAIL,
            Password = VALID_TEST_USER_PASSWORD
        });

        if(!response.IsSuccessStatusCode)
        {
            await _client.PostAsJsonAsync<RegisterRequest>("/api/accounts/Register", new RegisterRequest()
            {
                ConfirmPassword = VALID_TEST_USER_PASSWORD,
                Email = VALID_TEST_USER_EMAIL,
                Password = VALID_TEST_USER_PASSWORD
            });
            response = await _client.PostAsJsonAsync<LoginRequest>("/api/accounts/Login", new LoginRequest()
            {
                Email = VALID_TEST_USER_EMAIL,
                Password = VALID_TEST_USER_PASSWORD
            });
        }

        return await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
    }
}