namespace MarksBaseballCards.Shared.Auth;

/// <summary>Successful login result containing the signed JWT and identity info.</summary>
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresUtc { get; set; }
}
