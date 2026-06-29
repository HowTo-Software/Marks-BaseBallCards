namespace MarksBaseballCards.Shared.Models;

/// <summary>A single audit-trail entry describing a change to the collection.</summary>
public class CardHistoryDto
{
    public int Id { get; set; }
    public int? CardId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;

    /// <summary>One of: Created, Updated, Deleted, Listed, Unlisted, Sold, Login, LoginFailed.</summary>
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAtUtc { get; set; }
}
