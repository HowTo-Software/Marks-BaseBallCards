namespace MarksBaseballCards.Shared.Models;

/// <summary>Returned to the client after creating a Stripe Checkout Session.</summary>
public class CheckoutSessionResponse
{
    /// <summary>The Stripe-hosted checkout page URL the browser should be redirected to.</summary>
    public string Url { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
}

/// <summary>Lightweight status of a checkout session, used by the success page.</summary>
public class CheckoutStatusDto
{
    public string Status { get; set; } = string.Empty; // open, complete, expired
    public string PaymentStatus { get; set; } = string.Empty; // paid, unpaid, no_payment_required
    public int? CardId { get; set; }
    public string? CardNumber { get; set; }
    public string? PlayerName { get; set; }
}
