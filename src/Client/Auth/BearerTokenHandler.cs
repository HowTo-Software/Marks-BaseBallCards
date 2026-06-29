using System.Net.Http.Headers;

namespace MarksBaseballCards.Client.Auth;

/// <summary>Attaches the stored bearer token to every outgoing API request.</summary>
public class BearerTokenHandler : DelegatingHandler
{
    private readonly TokenStore _store;

    public BearerTokenHandler(TokenStore store) => _store = store;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _store.GetAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
