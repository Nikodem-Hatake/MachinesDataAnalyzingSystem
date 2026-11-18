using AutoFixture;
using FluentAssertions;
using MachinesDataAnalyzingSystem.Core.Domain.Identity;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.Accounts;
using MachinesDataAnalyzingSystem.WebAPI.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;

namespace MachinesDataAnalyzingSystem.ControllersTests;

public class AccountsControllerTests
{
    private readonly AccountsController _accountsController;
    private readonly Mock<IAccountsService> _accountsServiceMock = new();
    private readonly Fixture _fixture = new();
    private readonly Mock<IJWTService> _JWTServiceMock = new();
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    public AccountsControllerTests()
    {
        _userManagerMock = new(new Mock<IUserStore<ApplicationUser>>().Object,
            null, new Mock<IPasswordHasher<ApplicationUser>>().Object,
            new List<IUserValidator<ApplicationUser>> { new UserValidator<ApplicationUser>() },
            new List<IPasswordValidator<ApplicationUser>> { new PasswordValidator<ApplicationUser>() },
            null, null, null, null);
        _signInManagerMock = new(_userManagerMock.Object, new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object, new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<ApplicationUser>>>().Object, new Mock<IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<ApplicationUser>>().Object);
        _accountsController = new(_signInManagerMock.Object, _userManagerMock.Object,
            _accountsServiceMock.Object, _JWTServiceMock.Object);
    }

    #region GetAccountsCount

    [Fact]
    public async Task GetAccountsCountShouldReturnCorrectAccountsCount()
    {
        var accountsCount = _fixture.Create<AccountsCountResponse>();
        _accountsServiceMock.Setup(x => x.GetAccountsCount())
            .ReturnsAsync(accountsCount);

        var response = await _accountsController.GetAccountsCount();

        response.Value.Should().BeEquivalentTo(accountsCount);
    }

    #endregion
    #region PostGenerateJWTToken

    [Fact]
    public async Task PostGenerateJWTTokenShouldReturnCorrectAuthenticationResponseForCorrectRequest()
    {
        var tokenRequest = _fixture.Create<JWTTokenRequest>();
        var claimsPrincipal = _fixture.Create<ClaimsPrincipal>();
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new(ClaimTypes.Email, _fixture.Create<string>()));
        claimsPrincipal.AddIdentity(claimsIdentity);
        _JWTServiceMock.Setup(x => x.GetPrincipalFromJWTToken(It.IsAny<string>()))
            .Returns(claimsPrincipal);
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Build<ApplicationUser>()
            .With(x => x.JWTRefreshTokenExpirationDate, DateTime.UtcNow.AddDays(1))
            .With(x => x.JWTRefreshToken, tokenRequest.JWTRefreshToken)
            .Create());
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<List<string>>());
        _JWTServiceMock.Setup(x => x.GenerateJWTToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
            .Returns(_fixture.Create<AuthenticationResponse>());
        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<IdentityResult>());

        var response = await _accountsController.PostGenerateJWTToken(tokenRequest);

        response.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PostGenerateJWTTokenShouldReturnUnauthorizedResponseForInvalidJWTToken()
    {
        var tokenRequest = _fixture.Create<JWTTokenRequest>();
        _JWTServiceMock.Setup(x => x.GetPrincipalFromJWTToken(It.IsAny<string>()))
            .Returns(value: null);
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Build<ApplicationUser>()
            .With(x => x.JWTRefreshTokenExpirationDate, DateTime.UtcNow.AddDays(1))
            .With(x => x.JWTRefreshToken, tokenRequest.JWTRefreshToken)
            .Create());
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<List<string>>());
        _JWTServiceMock.Setup(x => x.GenerateJWTToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
            .Returns(_fixture.Create<AuthenticationResponse>());
        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<IdentityResult>());

        var response = await _accountsController.PostGenerateJWTToken(tokenRequest);

        response.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task PostGenerateJWTTokenShouldReturnUnauthorizedResponseForEmptyEmail()
    {
        var tokenRequest = _fixture.Create<JWTTokenRequest>();
        var claimsPrincipal = _fixture.Create<ClaimsPrincipal>();
        _JWTServiceMock.Setup(x => x.GetPrincipalFromJWTToken(It.IsAny<string>()))
            .Returns(claimsPrincipal);
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Build<ApplicationUser>()
            .With(x => x.JWTRefreshTokenExpirationDate, DateTime.UtcNow.AddDays(1))
            .With(x => x.JWTRefreshToken, tokenRequest.JWTRefreshToken)
            .Create());
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<List<string>>());
        _JWTServiceMock.Setup(x => x.GenerateJWTToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
            .Returns(_fixture.Create<AuthenticationResponse>());
        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<IdentityResult>());

        var response = await _accountsController.PostGenerateJWTToken(tokenRequest);

        response.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task PostGenerateJWTTokenShouldReturnUnauthorizedResponseForEmailNotFound()
    {
        var tokenRequest = _fixture.Create<JWTTokenRequest>();
        var claimsPrincipal = _fixture.Create<ClaimsPrincipal>();
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new(ClaimTypes.Email, _fixture.Create<string>()));
        claimsPrincipal.AddIdentity(claimsIdentity);
        _JWTServiceMock.Setup(x => x.GetPrincipalFromJWTToken(It.IsAny<string>()))
            .Returns(claimsPrincipal);
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(value: null);
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<List<string>>());
        _JWTServiceMock.Setup(x => x.GenerateJWTToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
            .Returns(_fixture.Create<AuthenticationResponse>());
        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<IdentityResult>());

        var response = await _accountsController.PostGenerateJWTToken(tokenRequest);

        response.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task PostGenerateJWTTokenShouldReturnUnauthorizedResponseForExpiredRefreshToken()
    {
        var tokenRequest = _fixture.Create<JWTTokenRequest>();
        var claimsPrincipal = _fixture.Create<ClaimsPrincipal>();
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new(ClaimTypes.Email, _fixture.Create<string>()));
        claimsPrincipal.AddIdentity(claimsIdentity);
        _JWTServiceMock.Setup(x => x.GetPrincipalFromJWTToken(It.IsAny<string>()))
            .Returns(claimsPrincipal);
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Build<ApplicationUser>()
            .With(x => x.JWTRefreshTokenExpirationDate, DateTime.UtcNow.AddDays(-1))
            .With(x => x.JWTRefreshToken, tokenRequest.JWTRefreshToken)
            .Create());
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<List<string>>());
        _JWTServiceMock.Setup(x => x.GenerateJWTToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
            .Returns(_fixture.Create<AuthenticationResponse>());
        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<IdentityResult>());

        var response = await _accountsController.PostGenerateJWTToken(tokenRequest);

        response.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task PostGenerateJWTTokenShouldReturnUnauthorizedResponseForInvalidRefreshToken()
    {
        var tokenRequest = _fixture.Create<JWTTokenRequest>();
        var claimsPrincipal = _fixture.Create<ClaimsPrincipal>();
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new(ClaimTypes.Email, _fixture.Create<string>()));
        claimsPrincipal.AddIdentity(claimsIdentity);
        _JWTServiceMock.Setup(x => x.GetPrincipalFromJWTToken(It.IsAny<string>()))
            .Returns(claimsPrincipal);
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Build<ApplicationUser>()
            .With(x => x.JWTRefreshTokenExpirationDate, DateTime.UtcNow.AddDays(1))
            .With(x => x.JWTRefreshToken, _fixture.Create<string>())
            .Create());
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<List<string>>());
        _JWTServiceMock.Setup(x => x.GenerateJWTToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
            .Returns(_fixture.Create<AuthenticationResponse>());
        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<IdentityResult>());

        var response = await _accountsController.PostGenerateJWTToken(tokenRequest);

        response.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    #endregion
    #region PostLogin

    [Fact]
    public async Task PostLoginShouldReturnCorrectAuthenticationResponseForCorrectRequest()
    {
        var user = _fixture.Create<ApplicationUser>();
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<ApplicationUser>(),
            It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<List<string>>());
        _JWTServiceMock.Setup(x => x.GenerateJWTToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
            .Returns(_fixture.Create<AuthenticationResponse>());
        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<IdentityResult>());

        var response = await _accountsController.PostLogin(_fixture.Create<LoginRequest>());

        response.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PostLoginShouldReturnBadRequestResponseForEmailNotFound()
    {
        var user = _fixture.Create<ApplicationUser>();
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(value: null);
        _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<ApplicationUser>(),
            It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<List<string>>());
        _JWTServiceMock.Setup(x => x.GenerateJWTToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
            .Returns(_fixture.Create<AuthenticationResponse>());
        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<IdentityResult>());

        var response = await _accountsController.PostLogin(_fixture.Create<LoginRequest>());

        response.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task PostLoginShouldReturnBadRequestResponseForInvalidPassword()
    {
        var user = _fixture.Create<ApplicationUser>();
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<ApplicationUser>(),
            It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<List<string>>());
        _JWTServiceMock.Setup(x => x.GenerateJWTToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
            .Returns(_fixture.Create<AuthenticationResponse>());
        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<IdentityResult>());

        var response = await _accountsController.PostLogin(_fixture.Create<LoginRequest>());

        response.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion
    #region PostLogout

    [Fact]
    public async Task PostLogoutShouldNotThrowException()
    {
        _signInManagerMock.Setup(x => x.SignOutAsync());

        var action = async () => await _accountsController.PostLogout();

        await action.Should().NotThrowAsync();
    }

    #endregion
    #region PostRegister

    [Fact]
    public async Task PostRegisterShouldReturnAuthenticationResponseForCorrectRequest()
    {
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _signInManagerMock.Setup(x => x.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>()));
        _JWTServiceMock.Setup(x => x.GenerateJWTToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
            .Returns(_fixture.Create<AuthenticationResponse>());
        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<IdentityResult>());

        var response = await _accountsController.PostRegister(_fixture.Create<RegisterRequest>());

        response.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PostRegisterShouldReturnBadRequestResponseForInvalidRequest()
    {
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        _signInManagerMock.Setup(x => x.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>()));
        _JWTServiceMock.Setup(x => x.GenerateJWTToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
            .Returns(_fixture.Create<AuthenticationResponse>());
        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(_fixture.Create<IdentityResult>());

        var response = await _accountsController.PostRegister(_fixture.Create<RegisterRequest>());

        response.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion
}