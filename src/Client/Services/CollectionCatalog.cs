using System.Net.Http.Json;

namespace MarksBaseballCards.Client.Services;

/// <summary>The original, public checklist. This is an archive, never a live sale inventory.</summary>
public sealed class CollectionCatalog(HttpClient http)
{
    private Task<List<CollectionRecord>>? _records;
    public async Task<List<CollectionRecord>> GetAsync()
    {
        try { return await (_records ??= LoadAsync()); }
        catch { _records = null; throw; }
    }
    private async Task<List<CollectionRecord>> LoadAsync() =>
        await http.GetFromJsonAsync<List<CollectionRecord>>("data/collection.json") ?? [];
}

public sealed class CollectionRecord
{
    public string CardNumber { get; set; } = "";
    public string PlayerName { get; set; } = "";
    public bool IsRookie { get; set; }
    public bool IsRoyals { get; set; }
}
