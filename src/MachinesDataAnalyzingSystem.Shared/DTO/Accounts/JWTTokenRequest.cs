using System.ComponentModel.DataAnnotations;

namespace MachinesDataAnalyzingSystem.Shared.DTO.Accounts;

public class JWTTokenRequest
{
    [Required]
    public string JWTRefreshToken { get; set; }
    [Required]
    public string JWTToken { get; set; }
}
