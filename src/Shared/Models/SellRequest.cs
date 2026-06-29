using System.ComponentModel.DataAnnotations;

namespace MarksBaseballCards.Shared.Models;

/// <summary>Payload used by an admin to record the sale of a card.</summary>
public class SellRequest
{
    [Range(0, 1_000_000, ErrorMessage = "Sale price must be between 0 and 1,000,000.")]
    public decimal? SoldPrice { get; set; }

    [StringLength(200, ErrorMessage = "Buyer / note must be 200 characters or fewer.")]
    public string? Note { get; set; }
}
