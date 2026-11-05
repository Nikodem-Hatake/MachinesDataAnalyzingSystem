using Microsoft.AspNetCore.Identity;

namespace MachinesDataAnalyzingSystem.Core.Domain.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string JWTRefreshToken { get; set; }
    public DateTime JWTRefreshTokenExpirationDate { get; set; }
}