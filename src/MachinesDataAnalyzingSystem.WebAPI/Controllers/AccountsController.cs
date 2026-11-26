using MachinesDataAnalyzingSystem.Core.Domain.Identity;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MachinesDataAnalyzingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, 
    IAccountsService accountsService, IJWTService JWTService) : ControllerBase
{
    [Authorize(Roles = "admin")]
    [HttpDelete("removeAdminRole/{email}")]
    public async Task<IActionResult> DeleteAdminRoleFromAccount([Required][EmailAddress] string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if(user is null)
        {
            return BadRequest("User not found.");
        }

        var result = await userManager.RemoveFromRoleAsync(user, "admin");
        if(!result.Succeeded)
        {
            return BadRequest("Can't remove admin role from user.");
        }

        return Ok();
    }

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<ActionResult<AccountsCountResponse>> GetAccountsCount()
    {
        return await accountsService.GetAccountsCount();
    }

    [HttpPost("GenerateJWTToken")]
    public async Task<ActionResult<AuthenticationResponse>> PostGenerateJWTToken(JWTTokenRequest JWTTokenRequest)
    {
        var principal = JWTService.GetPrincipalFromJWTToken(JWTTokenRequest.JWTToken);
        if(principal is null)
        {
            return Unauthorized("Invalid JWT token.");
        }

        var email = principal.FindFirstValue(ClaimTypes.Email);
        if(string.IsNullOrWhiteSpace(email))
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
            return BadRequest("Email or password not matched.");
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

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> PostLogout()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if(string.IsNullOrWhiteSpace(email))
        {
            return Unauthorized("Invalid JWT token.");
        }

        var user = await userManager.FindByEmailAsync(email);
        if(user is null)
        {
            return Unauthorized("Email not found in token.");
        }

        user.JWTRefreshToken = JWTService.GenerateJWTRefreshToken();
        user.JWTRefreshTokenExpirationDate = DateTime.UtcNow.AddDays(-1);
        await userManager.UpdateAsync(user);
        await signInManager.SignOutAsync();
        return Ok();
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

    [Authorize(Roles = "admin")]
    [HttpPut("addAdminRole/{email}")]
    public async Task<IActionResult> PutAccountToAdminRole([Required][EmailAddress] string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if(user is null)
        {
            return BadRequest("User not found.");
        }

        var result = await userManager.AddToRoleAsync(user, "admin");
        if(!result.Succeeded)
        {
            return BadRequest("Can't add admin role to user.");
        }

        return Ok();
    }
}