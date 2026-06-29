namespace MarksBaseballCards.Api.Entities;

/// <summary>An application user that can sign in to the admin areas.</summary>
public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;

    /// <summary>PBKDF2 hash produced by <see cref="Microsoft.AspNetCore.Identity.PasswordHasher{TUser}"/>.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    /// <summary>Consecutive failed sign-in attempts; reset on success.</summary>
    public int FailedLoginCount { get; set; }

    /// <summary>When set and in the future, the account is locked out.</summary>
    public DateTime? LockoutEndUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastLoginUtc { get; set; }
}
