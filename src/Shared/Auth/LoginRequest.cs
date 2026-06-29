using System.ComponentModel.DataAnnotations;

namespace MarksBaseballCards.Shared.Auth;

/// <summary>Credentials submitted to the login endpoint.</summary>
public class LoginRequest
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(64, MinimumLength = 3, ErrorMessage = "Username must be 3-64 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;
}
