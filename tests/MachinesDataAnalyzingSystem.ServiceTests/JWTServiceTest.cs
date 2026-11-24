using AutoFixture;
using FluentAssertions;
using MachinesDataAnalyzingSystem.Core.Domain.Identity;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Core.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace MachinesDataAnalyzingSystem.ServiceTests;

public class JWTServiceTest
{
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly Fixture _fixture = new();
    private readonly IJWTService _JWTService;

    public JWTServiceTest()
    {
        _JWTService = new JWTService(_configurationMock.Object);
    }

    #region GenerateJWTRefreshToken

    [Fact]
    public async Task GenerateJWTRefreshTokenShouldGenerateRandomJWTRefreshToken()
    {
        var refreshTokenOne = _JWTService.GenerateJWTRefreshToken();
        var refreshTokenTwo = _JWTService.GenerateJWTRefreshToken();

        refreshTokenOne.Should().NotBeNullOrWhiteSpace();
        refreshTokenTwo.Should().NotBeNullOrWhiteSpace();
        refreshTokenOne.Should().NotBe(refreshTokenTwo);
    }

    #endregion
    #region GenerateJWTToken

    [Fact]
    public async Task GenerateJWTTokenShouldGenerateValidJWTTokenWithValidJWTRefreshToken()
    {
        var randomExpirationValue = Random.Shared.Next(5, 11);
        var user = _fixture.Create<ApplicationUser>();
        _configurationMock.Setup(x => x["JWTSettings:TokenExpirationMinutes"]).Returns(randomExpirationValue.ToString());
        _configurationMock.Setup(x => x["JWTSettings:RefreshTokenExpirationDays"]).Returns(randomExpirationValue.ToString());
        _configurationMock.Setup(x => x["JWTSettings:Key"]).Returns(_fixture.Create<string>());
        _configurationMock.Setup(x => x["JWTSettings:Audience"]).Returns(_fixture.Create<string>());
        _configurationMock.Setup(x => x["JWTSettings:Issuer"]).Returns(_fixture.Create<string>());

        var response = _JWTService.GenerateJWTToken(user, _fixture.Create<List<string>>());

        response.Should().NotBeNull();
        response.JWTTokenExpirationDate.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(int.Parse
            (_configurationMock.Object["JWTSettings:TokenExpirationMinutes"])), TimeSpan.FromMinutes(1));
        response.JWTRefreshTokenExpirationDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(int.Parse
            (_configurationMock.Object["JWTSettings:TokenExpirationMinutes"])), TimeSpan.FromMinutes(1));
        response.JWTToken.Should().NotBeNullOrEmpty();
        response.JWTRefreshToken.Should().NotBeEmpty();
        response.JWTToken.Split(".").Should().HaveCount(3);
        var claimsPrincipal = _JWTService.GetPrincipalFromJWTToken(response.JWTToken);
        claimsPrincipal.Should().NotBeNull();
        var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email);
        emailClaim.Should().NotBeNull();
        emailClaim.Value.Should().Be(user.Email);
    }

    #endregion
    #region GetPrincipalFromJWTToken

    [Fact]
    public async Task GetPrincipalFromJWTTokenShouldReturnValidClaimsPrincipalForValidJWTToken()
    {
        _configurationMock.Setup(x => x["JWTSettings:Key"]).Returns(_fixture.Create<string>());
        _configurationMock.Setup(x => x["JWTSettings:Audience"]).Returns(_fixture.Create<string>());
        _configurationMock.Setup(x => x["JWTSettings:Issuer"]).Returns(_fixture.Create<string>());
        var user = _fixture.Create<ApplicationUser>();
        var token = _JWTService.GenerateJWTToken(user, _fixture.Create<List<string>>()).JWTToken;

        var claimsPrincipal = _JWTService.GetPrincipalFromJWTToken(token);

        claimsPrincipal.Should().NotBeNull();
        var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email);
        emailClaim.Should().NotBeNull();
        emailClaim.Value.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetPrincipalFromJWTTokenShouldReturnNullForInvalidJWTToken()
    {
        _configurationMock.Setup(x => x["JWTSettings:Key"]).Returns(_fixture.Create<string>());
        _configurationMock.Setup(x => x["JWTSettings:Audience"]).Returns(_fixture.Create<string>());
        _configurationMock.Setup(x => x["JWTSettings:Issuer"]).Returns(_fixture.Create<string>());

        var claimsPrincipal = _JWTService.GetPrincipalFromJWTToken(_fixture.Create<string>());

        claimsPrincipal.Should().BeNull();
    }

    [Fact]
    public async Task GetPrincipalFromJWTTokenShouldReturnNullForInvalidJWTTokenKey()
    {
        int randomExpirationValue = Random.Shared.Next(5, 11);
        _configurationMock.Setup(x => x["JWTSettings:TokenExpirationMinutes"]).Returns(randomExpirationValue.ToString());
        _configurationMock.Setup(x => x["JWTSettings:RefreshTokenExpirationDays"]).Returns(randomExpirationValue.ToString());
        _configurationMock.Setup(x => x["JWTSettings:Key"]).Returns(_fixture.Create<string>());
        _configurationMock.Setup(x => x["JWTSettings:Audience"]).Returns(_fixture.Create<string>());
        _configurationMock.Setup(x => x["JWTSettings:Issuer"]).Returns(_fixture.Create<string>());
        var token = _JWTService.GenerateJWTToken(_fixture.Create<ApplicationUser>(), _fixture.Create<List<string>>()).JWTToken;

        _configurationMock.Setup(x => x["JWTSettings:Key"]).Returns(_fixture.Create<string>());
        var claimsPrincipal = _JWTService.GetPrincipalFromJWTToken(_fixture.Create<string>());

        claimsPrincipal.Should().BeNull();
    }

    [Fact]
    public async Task GetPrincipalFromJWTTokenShouldReturnNullForInvalidJWTTokenAudience()
    {
        int randomExpirationValue = Random.Shared.Next(5, 11);
        _configurationMock.Setup(x => x["JWTSettings:TokenExpirationMinutes"]).Returns(randomExpirationValue.ToString());
        _configurationMock.Setup(x => x["JWTSettings:RefreshTokenExpirationDays"]).Returns(randomExpirationValue.ToString());
        _configurationMock.Setup(x => x["JWTSettings:Key"]).Returns(_fixture.Create<string>());
        _configurationMock.Setup(x => x["JWTSettings:Audience"]).Returns(_fixture.Create<string>());
        _configurationMock.Setup(x => x["JWTSettings:Issuer"]).Returns(_fixture.Create<string>());
        var token = _JWTService.GenerateJWTToken(_fixture.Create<ApplicationUser>(), _fixture.Create<List<string>>()).JWTToken;

        _configurationMock.Setup(x => x["JWTSettings:Audience"]).Returns(_fixture.Create<string>());
        var claimsPrincipal = _JWTService.GetPrincipalFromJWTToken(_fixture.Create<string>());

        claimsPrincipal.Should().BeNull();
    }

    [Fact]
    public async Task GetPrincipalFromJWTTokenShouldReturnNullForInvalidJWTTokenIssuer()
    {
        int randomExpirationValue = Random.Shared.Next(5, 11);
        _configurationMock.Setup(x => x["JWTSettings:TokenExpirationMinutes"]).Returns(randomExpirationValue.ToString());
        _configurationMock.Setup(x => x["JWTSettings:RefreshTokenExpirationDays"]).Returns(randomExpirationValue.ToString());
        _configurationMock.Setup(x => x["JWTSettings:Key"]).Returns(_fixture.Create<string>());
        _configurationMock.Setup(x => x["JWTSettings:Audience"]).Returns(_fixture.Create<string>());
        _configurationMock.Setup(x => x["JWTSettings:Issuer"]).Returns(_fixture.Create<string>());
        var token = _JWTService.GenerateJWTToken(_fixture.Create<ApplicationUser>(), _fixture.Create<List<string>>()).JWTToken;

        _configurationMock.Setup(x => x["JWTSettings:Issuer"]).Returns(_fixture.Create<string>());
        var claimsPrincipal = _JWTService.GetPrincipalFromJWTToken(_fixture.Create<string>());

        claimsPrincipal.Should().BeNull();
    }

    #endregion
}