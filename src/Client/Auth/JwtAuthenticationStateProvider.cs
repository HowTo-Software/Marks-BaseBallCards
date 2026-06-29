using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace MarksBaseballCards.Client.Auth;

/// <summary>
/// Builds the Blazor authentication state from the JWT held in <see cref="TokenStore"/>,
/// validating expiry locally and exposing role/name claims.
/// </summary>
public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly TokenStore _store;

    public JwtAuthenticationStateProvider(TokenStore store) => _store = store;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _store.GetAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            return Anonymous;
        }

        var claims = ParseClaims(token).ToList();
        if (IsExpired(claims))
        {
            await _store.RemoveAsync();
            return Anonymous;
        }

        var identity = new ClaimsIdentity(claims, authenticationType: "jwt", nameType: "name", roleType: "role");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    /// <summary>Re-evaluates auth state after login / logout.</summary>
    public void NotifyStateChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    private static bool IsExpired(IEnumerable<Claim> claims)
    {
        var exp = claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        if (long.TryParse(exp, out var seconds))
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds) <= DateTimeOffset.UtcNow;
        }
        return false;
    }

    private static IEnumerable<Claim> ParseClaims(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length < 2)
        {
            yield break;
        }

        var json = Base64UrlDecode(parts[1]);
        var map = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        if (map is null)
        {
            yield break;
        }

        foreach (var (key, value) in map)
        {
            if (value.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in value.EnumerateArray())
                {
                    yield return new Claim(key, item.ToString());
                }
            }
            else
            {
                yield return new Claim(key, value.ToString());
            }
        }
    }

    private static byte[] Base64UrlDecode(string input)
    {
        var s = input.Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4)
        {
            case 2: s += "=="; break;
            case 3: s += "="; break;
        }
        return Convert.FromBase64String(s);
    }
}
