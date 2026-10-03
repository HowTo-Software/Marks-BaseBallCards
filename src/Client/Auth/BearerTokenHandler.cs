using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace MarksBaseballCards.Client.Auth;

/// <summary>Attaches the stored bearer token to every outgoing API request.</summary>
public class BearerTokenHandler : DelegatingHandler
{
    private readonly TokenStore _store;

    public BearerTokenHandler(TokenStore store) => _store = store;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? token = null;
        try { token = await _store.GetAsync(); }
        catch (JSException)
        {
            // Public reads remain available when browser storage is disabled.
            // Protected endpoints still enforce authentication on the server.
        }
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
