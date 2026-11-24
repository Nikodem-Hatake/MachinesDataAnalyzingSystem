using AutoFixture;
using FluentAssertions;
using MachinesDataAnalyzingSystem.Core.Domain.Identity;
using MachinesDataAnalyzingSystem.Shared.DTO.Accounts;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace MachinesDataAnalyzingSystem.IntegrationTests;

public class AccountsControllerIntegrationTest(WebAPIWebApplicationFactory webAPIWebApplicationFactory) 
    : BaseIntegrationTest(webAPIWebApplicationFactory)
{
    #region GetAccountsCount

    [Fact]
    public async Task GetAccountsCountShouldReturnCorrectAccountsCount()
    {
        var JWTToken = (await GetValidTestAdminAuthenticationResponse()).JWTToken;
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/accounts");
        request.Headers.Authorization = new("Bearer", JWTToken);

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var accountsCount = await response.Content.ReadFromJsonAsync<AccountsCountResponse>();
        accountsCount.Should().NotBeNull();
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        accountsCount.TotalCount.Should().Be(await dbContext.Users.LongCountAsync());
        var adminRole = await dbContext.Roles.FirstAsync(x => x.NormalizedName == "ADMIN");
        accountsCount.AdminAccountsCount.Should().Be(await dbContext.UserRoles.LongCountAsync(x => x.RoleId == adminRole.Id));
    }

    [Fact]
    public async Task GetAccountsCountShouldReturnForbiddenStatusCodeForNonAdminUser()
    {
        var JWTToken = (await GetValidTestUserAuthenticationResponse()).JWTToken;
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/accounts");
        request.Headers.Authorization = new("Bearer", JWTToken);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAccountsCountShouldReturnUnauthorizedStatusCodeForNoAuthorization()
    {
        var response = await _client.GetAsync("/api/accounts");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
    #region PostGenerateJWTToken

    [Fact]
    public async Task PostGenerateJWTTokenShouldCorrectlyGenerateJWTTokenWithRefreshTokenAndReturnItForCorrectRequest()
    {
        var authenticationResponse = await GetValidTestAdminAuthenticationResponse();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/GenerateJWTToken");
        request.Content = JsonContent.Create<JWTTokenRequest>(new()
        {
            JWTRefreshToken = authenticationResponse.JWTRefreshToken,
            JWTToken = authenticationResponse.JWTToken
        });

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var newAuthenticationResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        newAuthenticationResponse.Should().NotBeEquivalentTo(authenticationResponse);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        (await dbContext.Users.FirstOrDefaultAsync(x => x.JWTRefreshToken == newAuthenticationResponse.JWTRefreshToken
            && x.JWTRefreshTokenExpirationDate == newAuthenticationResponse.JWTRefreshTokenExpirationDate)).Should().NotBeNull();
    }

    [Fact]
    public async Task PostGenerateJWTTokenShouldReturnUnauthorizedStatusCodeForInvalidJWTToken()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/GenerateJWTToken");
        request.Content = JsonContent.Create<JWTTokenRequest>(new()
        {
            JWTRefreshToken = _fixture.Create<string>(),
            JWTToken = _fixture.Create<string>()
        });

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostGenerateJWTTokenShouldReturnUnauthorizedStatusCodeForInvalidJWTRefreshToken()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/GenerateJWTToken");
        request.Content = JsonContent.Create<JWTTokenRequest>(new()
        {
            JWTRefreshToken = _fixture.Create<string>(),
            JWTToken = (await GetValidTestUserAuthenticationResponse()).JWTToken
        });
        var content = await request.Content.ReadAsStringAsync();

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostGenerateJWTTokenShouldReturnUnauthorizedStatusCodeForSomeonesOtherJWTTokenWithUserJWTRefreshToken()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/GenerateJWTToken");
        request.Content = JsonContent.Create<JWTTokenRequest>(new()
        {
            JWTRefreshToken = (await GetValidTestUserAuthenticationResponse()).JWTRefreshToken,
            JWTToken = (await GetValidTestAdminAuthenticationResponse()).JWTToken
        });
        var content = await request.Content.ReadAsStringAsync();

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostGenerateJWTTokenShouldReturnBadRequestStatusCodeForInvalidRequestBody()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/GenerateJWTToken");
        request.Content = JsonContent.Create<JWTTokenRequest>(new()
        {
            JWTRefreshToken = null,
            JWTToken = null
        });

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostGenerateJWTTokenShouldReturnUnsupportedMediaTypeStatusCodeForEmptyRequestBody()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/GenerateJWTToken");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    #endregion
    #region PostLogin

    [Fact]
    public async Task PostLoginShouldGenerateNewJWTRefreshTokenAndReturnAuthenticationResponseForValidRequest()
    {
        await GetValidTestUserAuthenticationResponse(); //Generate JWTRefreshToken in database for test user.
        ApplicationUser user;
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            user = await dbContext.Users.FirstAsync(x => x.Email == VALID_TEST_USER_EMAIL);
        }
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/login");
        request.Content = JsonContent.Create<LoginRequest>(new()
        {
            Email = VALID_TEST_USER_EMAIL,
            Password = VALID_TEST_USER_PASSWORD
        });

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var authenticationResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        authenticationResponse.JWTRefreshToken.Should().NotBeNullOrWhiteSpace();
        authenticationResponse.JWTRefreshToken.Should().NotBe(user.JWTRefreshToken);
        authenticationResponse.JWTRefreshTokenExpirationDate.Should().NotBe(user.JWTRefreshTokenExpirationDate);
        authenticationResponse.JWTToken.Should().NotBeNullOrWhiteSpace();
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            user = await dbContext.Users.FirstAsync(x => x.Email == VALID_TEST_USER_EMAIL);
        }
        authenticationResponse.JWTRefreshToken.Should().Be(user.JWTRefreshToken);
        authenticationResponse.JWTRefreshTokenExpirationDate.Should().BeCloseTo(user.JWTRefreshTokenExpirationDate, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task PostLoginShouldReturnBadRequestStatusCodeForInvalidEmail()
    {
        await GetValidTestUserAuthenticationResponse(); //Ensures that test user is in database.
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/login");
        request.Content = JsonContent.Create<LoginRequest>(new()
        {
            Email = _fixture.Create<string>(),
            Password = VALID_TEST_USER_PASSWORD
        });

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostLoginShouldReturnBadRequestStatusCodeForInvalidPassword()
    {
        await GetValidTestUserAuthenticationResponse(); //Ensures that test user is in database.
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/login");
        request.Content = JsonContent.Create<LoginRequest>(new()
        {
            Email = VALID_TEST_USER_EMAIL,
            Password = _fixture.Create<string>()
        });

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostLoginShouldReturnBadRequestStatusCodeForUserNotFound()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/login");
        request.Content = JsonContent.Create<LoginRequest>(new()
        {
            Email = _fixture.Create<string>(),
            Password = _fixture.Create<string>()
        });

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostLoginShouldReturnBadRequestStatusCodeForInvalidRequestBody()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/login");
        request.Content = JsonContent.Create<LoginRequest>(new()
        {
            Email = null,
            Password = null
        });

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostLoginShouldReturnUnsupportedMediaTypeStatusCodeForEmptyRequestBody()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/login");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    #endregion
    #region PostLogout

    [Fact]
    public async Task PostLogoutShouldChangeUserJWTRefreshTokenInformationForValidRequest()
    {
        var user = await GetValidTestUserAuthenticationResponse();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/logout");
        request.Headers.Authorization = new("Bearer", user.JWTToken);

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var newUserInfo = await dbContext.Users.FirstAsync(x => x.Email == VALID_TEST_USER_EMAIL);
        user.JWTRefreshToken.Should().NotBe(newUserInfo.JWTRefreshToken);
        user.JWTRefreshTokenExpirationDate.Should().NotBe(newUserInfo.JWTRefreshTokenExpirationDate);
    }

    [Fact]
    public async Task PostLogoutShouldReturnUnauthorizedStatusCodeForUnauthorizedRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/logout");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
    #region PostRegister

    [Fact]
    public async Task PostRegisterShouldReturnAuthenticationResponseForValidRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/register");
        var email = nameof(PostRegisterShouldReturnAuthenticationResponseForValidRequest) + "@example.com";
        request.Content = JsonContent.Create<RegisterRequest>(new()
        {
            ConfirmPassword = "Test123.",
            Email = email,
            Password = "Test123."
        });

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var authenticationResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        authenticationResponse.JWTRefreshToken.Should().NotBeNullOrWhiteSpace();
        authenticationResponse.JWTToken.Should().NotBeNullOrWhiteSpace();
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        user.Should().NotBeNull();
        user.Email.Should().Be(email);
        user.JWTRefreshToken.Should().Be(authenticationResponse.JWTRefreshToken);
        user.JWTRefreshTokenExpirationDate.Should().BeCloseTo(authenticationResponse.JWTRefreshTokenExpirationDate, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task PostRegisterShouldReturnBadRequestStatusCodeForInvalidEmail()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/register");
        var email = Guid.NewGuid().ToString();
        request.Content = JsonContent.Create<RegisterRequest>(new()
        {
            ConfirmPassword = "Test123.",
            Email = email,
            Password = "Test123."
        });

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        user.Should().BeNull();
    }

    [InlineData("")]
    [InlineData("Password")]
    [InlineData("12345678")]
    [InlineData("Aa1._")]
    [Theory]
    public async Task PostRegisterShouldReturnBadRequestStatusCodeForInvalidPassword(string password)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/register");
        var email = nameof(PostRegisterShouldReturnBadRequestStatusCodeForInvalidPassword) + password + "@example.com";
        request.Content = JsonContent.Create<RegisterRequest>(new()
        {
            ConfirmPassword = password,
            Email = email,
            Password = password
        });

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        user.Should().BeNull();
    }

    [Fact]
    public async Task PostRegisterShouldReturnBadRequestStatusCodeForPasswordNotMatching()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/register");
        var email = nameof(PostRegisterShouldReturnBadRequestStatusCodeForPasswordNotMatching) + "@example.com";
        request.Content = JsonContent.Create<RegisterRequest>(new()
        {
            ConfirmPassword = "Test456.",
            Email = email,
            Password = "Test123."
        });

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        user.Should().BeNull();
    }

    [Fact]
    public async Task PostRegisterShouldReturnBadRequestStatusCodeForUserAlreadyExisting()
    {
        await GetValidTestUserAuthenticationResponse(); //Ensures that test user is in database.
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/register");
        request.Content = JsonContent.Create<RegisterRequest>(new()
        {
            ConfirmPassword = VALID_TEST_USER_EMAIL,
            Email = VALID_TEST_USER_EMAIL,
            Password = VALID_TEST_USER_EMAIL
        });

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var users = await dbContext.Users.Where(x => x.Email == VALID_TEST_USER_EMAIL).ToListAsync();
        users.Count.Should().Be(1);
    }

    [Fact]
    public async Task PostRegisterShouldReturnBadRequestStatusCodeForInvalidRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/register");
        request.Content = JsonContent.Create<RegisterRequest>(new()
        {
            ConfirmPassword = null,
            Email = null,
            Password = null
        });

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == null);
        user.Should().BeNull();
    }

    [Fact]
    public async Task PostRegisterShouldReturnUnsupportedMediaTypeStatusCodeForEmptyRequestBody()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/register");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    #endregion
}