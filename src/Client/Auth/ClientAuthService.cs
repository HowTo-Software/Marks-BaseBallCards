using System.Net;
using System.Net.Http.Json;
using MarksBaseballCards.Shared.Auth;

namespace MarksBaseballCards.Client.Auth;

/// <summary>Client-side login / logout that talks to the API auth endpoint.</summary>
public class ClientAuthService
{
    private readonly HttpClient _http;
    private readonly TokenStore _store;
    private readonly JwtAuthenticationStateProvider _provider;

    public ClientAuthService(HttpClient http, TokenStore store, JwtAuthenticationStateProvider provider)
    {
        _http = http;
        _store = store;
        _provider = provider;
    }

    public async Task<(bool Ok, string? Error)> LoginAsync(LoginRequest request)
    {
        HttpResponseMessage response;
        try
        {
            response = await _http.PostAsJsonAsync("api/auth/login", request);
        }
        catch
        {
            return (false, "Unable to reach the server. Please try again.");
        }

        using (response)
        {
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result is null || string.IsNullOrWhiteSpace(result.Token))
                {
                    return (false, "Unexpected response from the server.");
                }

                await _store.SetAsync(result.Token);
                _provider.NotifyStateChanged();
                return (true, null);
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                return (false, "Too many sign-in attempts. Please wait a minute and try again.");
            }

            if ((int)response.StatusCode >= 500 || response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.MethodNotAllowed)
            {
                return (false, "Staff sign-in is unavailable right now. Please try again later.");
            }
            var error = await ReadErrorAsync(response);
            return (false, error ?? "Invalid username or password.");
        }
    }

    public async Task LogoutAsync()
    {
        await _store.RemoveAsync();
        _provider.NotifyStateChanged();
    }

    private static async Task<string?> ReadErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var body = await response.Content.ReadFromJsonAsync<ErrorBody>();
            return body?.Error;
        }
        catch
        {
            return null;
        }
    }

    private sealed record ErrorBody(string? Error);
}
