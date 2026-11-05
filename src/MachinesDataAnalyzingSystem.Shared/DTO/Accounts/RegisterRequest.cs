using System.ComponentModel.DataAnnotations;

namespace MachinesDataAnalyzingSystem.Shared.DTO.Accounts;

public class RegisterRequest
{
    [Compare(nameof(Password))]
    [Required]
    public string ConfirmPassword { get; set; }

    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}