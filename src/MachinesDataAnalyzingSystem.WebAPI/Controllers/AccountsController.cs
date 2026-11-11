using MachinesDataAnalyzingSystem.Core.Domain.Identity;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MachinesDataAnalyzingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController(SignInManager<ApplicationUser> signInManager, 
    UserManager<ApplicationUser> userManager, IJWTService JWTService) : ControllerBase
{
    [HttpPost("GenerateJWTToken")]
    public async Task<ActionResult<AuthenticationResponse>> PostGenerateJWTToken(JWTTokenRequest JWTTokenRequest)
    {
        var principal = JWTService.GetPrincipalFromJWTToken(JWTTokenRequest.JWTToken);
        if(principal is null)
        {
            return Unauthorized("Invalid JWT token.");
        }

        var email = principal.FindFirstValue(ClaimTypes.Email);
        if(email is null)
        {
            return Unauthorized("Email not found in token.");
        }

        var user = await userManager.FindByEmailAsync(email);
        if(user is null || user.JWTRefreshToken != JWTTokenRequest.JWTRefreshToken
            || user.JWTRefreshTokenExpirationDate <= DateTime.UtcNow)
        {
            return Unauthorized("Invalid JWT refresh token.");
        }

        var response = JWTService.GenerateJWTToken(user, await userManager.GetRolesAsync(user));
        user.JWTRefreshToken = response.JWTRefreshToken;
        user.JWTRefreshTokenExpirationDate = response.JWTRefreshTokenExpirationDate;
        await userManager.UpdateAsync(user);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponse>> PostLogin(LoginRequest loginRequest)
    {
        var user = await userManager.FindByEmailAsync(loginRequest.Email);
        if(user is null)
        {
            return BadRequest("User not found.");
        }

        var result = await signInManager.PasswordSignInAsync(user, loginRequest.Password, false, false);
        if(!result.Succeeded)
        {
            return BadRequest("Email or password not matched.");
        }
        
        var response = JWTService.GenerateJWTToken(user, await userManager.GetRolesAsync(user));
        user.JWTRefreshToken = response.JWTRefreshToken;
        user.JWTRefreshTokenExpirationDate = response.JWTRefreshTokenExpirationDate;
        await userManager.UpdateAsync(user);
        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task PostLogout()
    {
        await signInManager.SignOutAsync();
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthenticationResponse>> PostRegister(RegisterRequest registerRequest)
    {
        var user = new ApplicationUser()
        {
            Email = registerRequest.Email,
            UserName = registerRequest.Email
        };

        var result = await userManager.CreateAsync(user, registerRequest.Password);
        if(!result.Succeeded)
        {
            return BadRequest("Can't create user with specified values.");
        }

        await signInManager.SignInAsync(user, false);
        var response = JWTService.GenerateJWTToken(user, await userManager.GetRolesAsync(user));
        user.JWTRefreshToken = response.JWTRefreshToken;
        user.JWTRefreshTokenExpirationDate = response.JWTRefreshTokenExpirationDate;
        await userManager.UpdateAsync(user);
        return Ok(response);
    }
}