namespace MarksBaseballCards.Api.Entities;

/// <summary>Immutable audit-trail entry for a change to the collection or a sign-in event.</summary>
public class CardHistory
{
    public int Id { get; set; }

    /// <summary>Null when the related card has since been deleted.</summary>
    public int? CardId { get; set; }
    public Card? Card { get; set; }

    public string CardNumber { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAtUtc { get; set; }
}
