using System.Net;
using System.Net.Http.Json;
using MarksBaseballCards.Shared.Models;

namespace MarksBaseballCards.Client.Services;

/// <summary>Result of a mutating API call, carrying either a value or an error message.</summary>
public record ApiResult<T>(bool Ok, T? Value, string? Error)
{
    public static ApiResult<T> Success(T value) => new(true, value, null);
    public static ApiResult<T> Fail(string error) => new(false, default, error);
}

/// <summary>Typed wrapper over the API endpoints used by the Blazor pages.</summary>
public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http) => _http = http;

    // ---- Public marketplace ----
    public async Task<List<MarketplaceCardDto>> GetMarketplaceAsync(string? search = null, bool includeSold = false)
    {
        var url = $"api/marketplace?includeSold={includeSold}";
        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"&search={Uri.EscapeDataString(search)}";
        }
        return await _http.GetFromJsonAsync<List<MarketplaceCardDto>>(url) ?? new();
    }

    public async Task<ApiResult<CheckoutSessionResponse>> StartCheckoutAsync(int cardId)
    {
        var response = await _http.PostAsync($"api/checkout/{cardId}", null);
        if (response.IsSuccessStatusCode)
        {
            var session = await response.Content.ReadFromJsonAsync<CheckoutSessionResponse>();
            return session is null
                ? ApiResult<CheckoutSessionResponse>.Fail("Unexpected response from server.")
                : ApiResult<CheckoutSessionResponse>.Success(session);
        }
        return ApiResult<CheckoutSessionResponse>.Fail(await ReadErrorAsync(response));
    }

    public async Task<CheckoutStatusDto?> GetCheckoutStatusAsync(string sessionId)
    {
        var response = await _http.GetAsync($"api/checkout/status/{Uri.EscapeDataString(sessionId)}");
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<CheckoutStatusDto>()
            : null;
    }

    // ---- Admin cards ----
    public async Task<List<CardDto>> GetCardsAsync(string? search = null)
    {
        var url = "api/cards";
        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"?search={Uri.EscapeDataString(search)}";
        }
        return await _http.GetFromJsonAsync<List<CardDto>>(url) ?? new();
    }

    public async Task<ApiResult<CardDto>> CreateCardAsync(CardUpsertDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/cards", dto);
        return await ReadResultAsync<CardDto>(response);
    }

    public async Task<ApiResult<CardDto>> UpdateCardAsync(int id, CardUpsertDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/cards/{id}", dto);
        return await ReadResultAsync<CardDto>(response);
    }

    public async Task<ApiResult<CardDto>> SellCardAsync(int id, SellRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/cards/{id}/sell", request);
        return await ReadResultAsync<CardDto>(response);
    }

    public async Task<ApiResult<bool>> DeleteCardAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/cards/{id}");
        return response.IsSuccessStatusCode
            ? ApiResult<bool>.Success(true)
            : ApiResult<bool>.Fail(await ReadErrorAsync(response));
    }

    // ---- System admin ----
    public async Task<StatisticsDto?> GetStatisticsAsync()
        => await _http.GetFromJsonAsync<StatisticsDto>("api/statistics");

    public async Task<List<CardHistoryDto>> GetHistoryAsync(int take = 200)
        => await _http.GetFromJsonAsync<List<CardHistoryDto>>($"api/history?take={take}") ?? new();

    // ---- Helpers ----
    private static async Task<ApiResult<T>> ReadResultAsync<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var value = await response.Content.ReadFromJsonAsync<T>();
            return value is null
                ? ApiResult<T>.Fail("Unexpected empty response.")
                : ApiResult<T>.Success(value);
        }
        return ApiResult<T>.Fail(await ReadErrorAsync(response));
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var body = await response.Content.ReadFromJsonAsync<ErrorBody>();
            if (!string.IsNullOrWhiteSpace(body?.Error))
            {
                return body!.Error!;
            }
        }
        catch
        {
            // fall through to status-based message
        }

        return response.StatusCode switch
        {
            HttpStatusCode.TooManyRequests => "Too many requests. Please slow down.",
            HttpStatusCode.Unauthorized => "Your session has expired. Please sign in again.",
            HttpStatusCode.Forbidden => "You do not have permission to do that.",
            HttpStatusCode.Conflict => "That action conflicts with the current state.",
            HttpStatusCode.ServiceUnavailable => "That feature is not configured.",
            _ => $"Request failed ({(int)response.StatusCode})."
        };
    }

    private sealed record ErrorBody(string? Error);
}
