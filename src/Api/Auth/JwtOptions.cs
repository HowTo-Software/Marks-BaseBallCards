namespace MarksBaseballCards.Api.Auth;

/// <summary>Strongly-typed JWT settings bound from the "Jwt" configuration section.</summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "MarksBaseballCards";
    public string Audience { get; set; } = "MarksBaseballCards";

    /// <summary>HMAC signing key. Must be at least 32 characters. Provided via configuration / .env.</summary>
    public string Key { get; set; } = string.Empty;

    public int ExpiryMinutes { get; set; } = 120;
}
