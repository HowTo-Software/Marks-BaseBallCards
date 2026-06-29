using Microsoft.JSInterop;

namespace MarksBaseballCards.Client.Auth;

/// <summary>Stores the JWT in browser localStorage.</summary>
public class TokenStore
{
    private const string Key = "mbc_token";
    private readonly IJSRuntime _js;

    public TokenStore(IJSRuntime js) => _js = js;

    public ValueTask<string?> GetAsync() => _js.InvokeAsync<string?>("localStorage.getItem", Key);

    public ValueTask SetAsync(string token) => _js.InvokeVoidAsync("localStorage.setItem", Key, token);

    public ValueTask RemoveAsync() => _js.InvokeVoidAsync("localStorage.removeItem", Key);
}
