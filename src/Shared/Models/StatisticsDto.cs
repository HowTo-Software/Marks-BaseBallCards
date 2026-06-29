namespace MarksBaseballCards.Shared.Models;

/// <summary>Progress toward completing one of the six master sets.</summary>
public class SetProgressDto
{
    public int SetNumber { get; set; }
    public int CardsFiled { get; set; }
    public int TotalCards { get; set; }
    public double PercentComplete => TotalCards == 0 ? 0 : Math.Round(CardsFiled * 100.0 / TotalCards, 1);
}

/// <summary>Aggregate statistics for the system-admin dashboard.</summary>
public class StatisticsDto
{
    public int TotalCards { get; set; }
    public int CardsInPlastic { get; set; }
    public int RookieCards { get; set; }
    public int RoyalsCards { get; set; }
    public int TotalDoubles { get; set; }

    public int CardsForSale { get; set; }
    public int CardsSold { get; set; }
    public decimal ForSaleValue { get; set; }
    public decimal TotalSalesValue { get; set; }

    public List<SetProgressDto> SetProgress { get; set; } = new();
    public List<CardHistoryDto> RecentActivity { get; set; } = new();
    public DateTime GeneratedAtUtc { get; set; }
}
