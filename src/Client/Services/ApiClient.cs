using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarksBaseballCards.Shared.Models;

namespace MarksBaseballCards.Client.Services;

/// <summary>Result of a mutating API call, carrying either a value or an error message.</summary>
public record ApiResult<T>(bool Ok, T? Value, string? Error)
{
    public static ApiResult<T> Success(T value) => new(true, value, null);
    public static ApiResult<T> Fail(string error) => new(false, default, error);
}

/// <summary>Typed wrapper over the existing API; network errors remain recoverable in the UI.</summary>
public class ApiClient(HttpClient http)
{
    public async Task<List<MarketplaceCardDto>> GetMarketplaceAsync(string? search = null, bool includeSold = false)
    {
        var url = $"api/marketplace?includeSold={includeSold}";
        if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";
        return await http.GetFromJsonAsync<List<MarketplaceCardDto>>(url) ?? [];
    }
    public Task<ApiResult<CheckoutSessionResponse>> StartCheckoutAsync(int cardId) =>
        SendAsync<CheckoutSessionResponse>(() => http.PostAsync($"api/checkout/{cardId}", null));
    public async Task<CheckoutStatusDto?> GetCheckoutStatusAsync(string sessionId)
    {
        using var response = await http.GetAsync($"api/checkout/status/{Uri.EscapeDataString(sessionId)}");
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<CheckoutStatusDto>() : null;
    }
    public async Task<List<CardDto>> GetCardsAsync(string? search = null)
    {
        var url = "api/cards";
        if (!string.IsNullOrWhiteSpace(search)) url += $"?search={Uri.EscapeDataString(search)}";
        return await http.GetFromJsonAsync<List<CardDto>>(url) ?? [];
    }
    public Task<ApiResult<CardDto>> CreateCardAsync(CardUpsertDto dto) =>
        SendAsync<CardDto>(() => http.PostAsJsonAsync("api/cards", dto));
    public Task<ApiResult<CardDto>> UpdateCardAsync(int id, CardUpsertDto dto) =>
        SendAsync<CardDto>(() => http.PutAsJsonAsync($"api/cards/{id}", dto));
    public Task<ApiResult<CardDto>> SellCardAsync(int id, SellRequest request) =>
        SendAsync<CardDto>(() => http.PostAsJsonAsync($"api/cards/{id}/sell", request));
    public async Task<ApiResult<bool>> DeleteCardAsync(int id)
    {
        try
        {
            using var response = await http.DeleteAsync($"api/cards/{id}");
            return response.IsSuccessStatusCode ? ApiResult<bool>.Success(true) : ApiResult<bool>.Fail(await ReadErrorAsync(response));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException) { return ApiResult<bool>.Fail("We couldn’t reach the server. Please try again."); }
    }
    public async Task<StatisticsDto?> GetStatisticsAsync() => await http.GetFromJsonAsync<StatisticsDto>("api/statistics");
    public async Task<List<CardHistoryDto>> GetHistoryAsync(int take = 200) => await http.GetFromJsonAsync<List<CardHistoryDto>>($"api/history?take={take}") ?? [];
    private static async Task<ApiResult<T>> SendAsync<T>(Func<Task<HttpResponseMessage>> send)
    {
        try
        {
            using var response = await send();
            if (!response.IsSuccessStatusCode) return ApiResult<T>.Fail(await ReadErrorAsync(response));
            var value = await response.Content.ReadFromJsonAsync<T>();
            return value is null ? ApiResult<T>.Fail("The server returned an empty response. Please try again.") : ApiResult<T>.Success(value);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException) { return ApiResult<T>.Fail("We couldn’t reach the server. Please try again."); }
        catch (JsonException) { return ApiResult<T>.Fail("The server returned an unexpected response. Please try again."); }
    }
    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var body = await response.Content.ReadFromJsonAsync<ErrorBody>();
            if (!string.IsNullOrWhiteSpace(body?.Error)) return body.Error;
        }
        catch (JsonException) { }
        catch (NotSupportedException) { }
        return response.StatusCode switch {
            HttpStatusCode.TooManyRequests => "Too many requests. Please wait a moment and try again.",
            HttpStatusCode.Unauthorized => "Your session has expired. Please sign in again.",
            HttpStatusCode.Forbidden => "Your account does not have access to this action.",
            HttpStatusCode.Conflict => "This record has changed. Refresh the page and try again.",
            HttpStatusCode.ServiceUnavailable => "This feature is not available right now. Please try again later.",
            _ => "That request couldn’t be completed. Please try again."
        };
    }
    private sealed record ErrorBody(string? Error);
}
