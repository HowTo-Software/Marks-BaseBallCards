using MarksBaseballCards.Api.Data;
using MarksBaseballCards.Api.Mapping;
using MarksBaseballCards.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarksBaseballCards.Api.Controllers;

[ApiController]
[Route("api/marketplace")]
[AllowAnonymous]
public class MarketplaceController : ControllerBase
{
    private readonly AppDbContext _db;

    public MarketplaceController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>Public listing of cards currently for sale (optionally including recently sold).</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MarketplaceCardDto>>> Get(
        [FromQuery] string? search, [FromQuery] bool includeSold = false)
    {
        var query = _db.Cards.AsNoTracking().Where(c => c.IsForSale || (includeSold && c.IsSold));
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c => EF.Functions.Like(c.PlayerName, $"%{term}%")
                                  || EF.Functions.Like(c.CardNumber, $"%{term}%"));
        }

        var cards = await query
            .OrderBy(c => c.IsSold)
            .ThenBy(c => c.PlayerName)
            .ToListAsync();

        return Ok(cards.Select(c => c.ToMarketplaceDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MarketplaceCardDto>> GetById(int id)
    {
        var card = await _db.Cards.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && (c.IsForSale || c.IsSold));
        return card is null ? NotFound() : Ok(card.ToMarketplaceDto());
    }
}
