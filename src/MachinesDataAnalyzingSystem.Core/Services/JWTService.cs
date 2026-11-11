using MachinesDataAnalyzingSystem.Core.Domain.Identity;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.Accounts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MachinesDataAnalyzingSystem.Core.Services;

public class JWTService(IConfiguration configuration) : IJWTService
{
    private string GenerateJWTRefreshToken()
    {
        var bytes = new byte[64];
        var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public AuthenticationResponse GenerateJWTToken(ApplicationUser user, IList<string> roles)
    {
        var expirationValue = DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["JWTSettings:TokenExpirationMinutes"]));
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            new(JwtRegisteredClaimNames.NameId, user.Email),
            new(ClaimTypes.Email, user.Email)
        };
        foreach(var role in roles)
        {
            claims.Add(new(ClaimTypes.Role, role));
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenGenerator = new JwtSecurityToken(configuration["JWTSettings:Issuer"], configuration["JWTSettings:Audience"],
            claims, expires: expirationValue, signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.WriteToken(tokenGenerator);
        return new()
        {
            JWTRefreshToken = GenerateJWTRefreshToken(),
            JWTRefreshTokenExpirationDate = DateTime.UtcNow.AddDays(Convert.ToDouble(configuration["JWTSettings:RefreshTokenExpirationDays"])),
            JWTToken = token,
            JWTTokenExpirationDate = expirationValue,
        };
    }

    public ClaimsPrincipal? GetPrincipalFromJWTToken(string token)
    {
        var tokenValidaitonParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"])),
            ValidAudience = configuration["JWTSettings:Audience"],
            ValidIssuer = configuration["JWTSettings:Issuer"],
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var result = tokenHandler.ValidateToken(token, tokenValidaitonParameters, out var securityToken);

        if(securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals
            (SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid JWT token.");
        }

        return result;
    }
}