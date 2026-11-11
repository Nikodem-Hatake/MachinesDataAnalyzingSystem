using MachinesDataAnalyzingSystem.Core.Domain.Identity;
using MachinesDataAnalyzingSystem.Shared.DTO.Accounts;
using System.Security.Claims;

namespace MachinesDataAnalyzingSystem.Core.ServiceContracts;

public interface IJWTService
{
    AuthenticationResponse GenerateJWTToken(ApplicationUser user, IList<string> roles);
    ClaimsPrincipal? GetPrincipalFromJWTToken(string token);
}