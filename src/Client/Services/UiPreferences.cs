using System.Globalization;
using System.Text.Json;
using Microsoft.JSInterop;

namespace MarksBaseballCards.Client.Services;

/// <summary>Browser preferences and the site's English, Brazilian Portuguese and Spanish copy.</summary>
public sealed class UiPreferences(IJSRuntime js)
{
    private static readonly Dictionary<string, Dictionary<string, string>> Translations = LoadTranslations();
    public string Language { get; private set; } = "en";
    public string Theme { get; private set; } = "light";
    public bool IsDark => Theme == "dark";
    public CultureInfo Culture => CultureInfo.GetCultureInfo(Language switch { "pt-BR" => "pt-BR", "es" => "es-ES", _ => "en-US" });
    public event Action? Changed;

    // The original English copy is the fallback for any missing translation.
    public string this[string text]
    {
        get
        {
            if (Translations.TryGetValue(Language, out var table) && table.TryGetValue(text, out var value)) return value;
            // This API message includes an inventory number, which must remain unchanged.
            const string prefix = "Card number '", suffix = "' already exists.";
            if (Language != "en" && text != "Card number '{0}' already exists." && text.StartsWith(prefix, StringComparison.Ordinal) && text.EndsWith(suffix, StringComparison.Ordinal))
                return Format("Card number '{0}' already exists.", text[prefix.Length..^suffix.Length]);
            return text;
        }
    }
    public string Format(string text, params object?[] args) => args.Length == 0 ? this[text] : string.Format(Culture, this[text], args);
    public string Usd(decimal? value, string fallback = "Not listed") => value is null ? this[fallback] : value.Value.ToString("N2", Culture);
    public string DateTimeLocal(DateTime utc) => utc.ToLocalTime().ToString("g", Culture);
    public string Number(double value) => value.ToString("0.##", Culture);

    public async Task InitializeAsync()
    {
        try
        {
            var saved = await js.InvokeAsync<BrowserPreferences>("htsPreferences.read");
            Language = ValidLanguage(saved.Language);
            Theme = saved.Theme == "dark" ? "dark" : "light";
        }
        catch (JSException) { /* Storage restrictions must not prevent the site from opening. */ }
    }
    public Task SetLanguageAsync(string? language) => ApplyAsync(ValidLanguage(language), Theme);
    public Task ToggleThemeAsync() => ApplyAsync(Language, IsDark ? "light" : "dark");
    private async Task ApplyAsync(string language, string theme)
    {
        Language = language;
        Theme = theme;
        try { await js.InvokeVoidAsync("htsPreferences.apply", Language, Theme); }
        catch (JSException) { /* Keep the selection for this session when persistence is unavailable. */ }
        Changed?.Invoke();
    }
    private static string ValidLanguage(string? language) => language is "pt-BR" or "es" ? language : "en";
    private static Dictionary<string, Dictionary<string, string>> LoadTranslations()
    {
        using var stream = typeof(UiPreferences).Assembly.GetManifestResourceStream("MarksBaseballCards.Client.Localization.translations.json")
            ?? throw new InvalidOperationException("The UI translations resource is missing.");
        return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(stream) ?? [];
    }
    private sealed record BrowserPreferences(string? Language, string? Theme);
}
