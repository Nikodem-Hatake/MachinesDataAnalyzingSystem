namespace MachinesDataAnalyzingSystem.Shared.DTO.Accounts;

public class AuthenticationResponse
{
    public string JWTRefreshToken { get; set; }
    public DateTime JWTRefreshTokenExpirationDate { get; set; }
    public string JWTToken { get; set; }
    public DateTime JWTTokenExpirationDate { get; set; }
}