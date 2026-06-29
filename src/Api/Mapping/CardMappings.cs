using MarksBaseballCards.Api.Entities;
using MarksBaseballCards.Shared.Models;

namespace MarksBaseballCards.Api.Mapping;

public static class CardMappings
{
    public static CardDto ToDto(this Card c) => new()
    {
        Id = c.Id,
        CardNumber = c.CardNumber,
        PlayerName = c.PlayerName,
        InPlastic = c.InPlastic,
        InSet1 = c.InSet1,
        InSet2 = c.InSet2,
        InSet3 = c.InSet3,
        InSet4 = c.InSet4,
        InSet5 = c.InSet5,
        InSet6 = c.InSet6,
        Doubles = c.Doubles,
        IsRookie = c.IsRookie,
        IsRoyals = c.IsRoyals,
        IsForSale = c.IsForSale,
        Price = c.Price,
        Condition = c.Condition,
        ListingNotes = c.ListingNotes,
        IsSold = c.IsSold,
        SoldAtUtc = c.SoldAtUtc,
        SoldPrice = c.SoldPrice
    };

    public static MarketplaceCardDto ToMarketplaceDto(this Card c) => new()
    {
        Id = c.Id,
        CardNumber = c.CardNumber,
        PlayerName = c.PlayerName,
        IsRookie = c.IsRookie,
        IsRoyals = c.IsRoyals,
        Price = c.Price,
        Condition = c.Condition,
        ListingNotes = c.ListingNotes,
        IsSold = c.IsSold
    };

    public static CardHistoryDto ToDto(this CardHistory h) => new()
    {
        Id = h.Id,
        CardId = h.CardId,
        CardNumber = h.CardNumber,
        PlayerName = h.PlayerName,
        Action = h.Action,
        Details = h.Details,
        ChangedBy = h.ChangedBy,
        ChangedAtUtc = h.ChangedAtUtc
    };

    /// <summary>Copies editable fields from an upsert payload onto the entity.</summary>
    public static void ApplyUpsert(this Card c, CardUpsertDto dto)
    {
        c.CardNumber = dto.CardNumber.Trim();
        c.PlayerName = dto.PlayerName.Trim();
        c.InPlastic = dto.InPlastic;
        c.InSet1 = dto.InSet1;
        c.InSet2 = dto.InSet2;
        c.InSet3 = dto.InSet3;
        c.InSet4 = dto.InSet4;
        c.InSet5 = dto.InSet5;
        c.InSet6 = dto.InSet6;
        c.Doubles = dto.Doubles;
        c.IsRookie = dto.IsRookie;
        c.IsRoyals = dto.IsRoyals;
        c.IsForSale = dto.IsForSale;
        c.Price = dto.Price;
        c.Condition = string.IsNullOrWhiteSpace(dto.Condition) ? null : dto.Condition.Trim();
        c.ListingNotes = string.IsNullOrWhiteSpace(dto.ListingNotes) ? null : dto.ListingNotes.Trim();
    }
}
