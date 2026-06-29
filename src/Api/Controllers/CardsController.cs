using MarksBaseballCards.Api.Data;
using MarksBaseballCards.Api.Entities;
using MarksBaseballCards.Api.Mapping;
using MarksBaseballCards.Api.Services;
using MarksBaseballCards.Shared.Common;
using MarksBaseballCards.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarksBaseballCards.Api.Controllers;

[ApiController]
[Route("api/cards")]
[Authorize(Roles = Roles.Admin)]
public class CardsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly HistoryWriter _history;

    public CardsController(AppDbContext db, HistoryWriter history)
    {
        _db = db;
        _history = history;
    }

    private string CurrentUser => User.Identity?.Name ?? "unknown";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CardDto>>> GetAll([FromQuery] string? search)
    {
        var query = _db.Cards.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c => EF.Functions.Like(c.PlayerName, $"%{term}%")
                                  || EF.Functions.Like(c.CardNumber, $"%{term}%"));
        }

        var cards = await query.OrderBy(c => c.Id).ToListAsync();
        return Ok(cards.Select(c => c.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CardDto>> GetById(int id)
    {
        var card = await _db.Cards.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return card is null ? NotFound() : Ok(card.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<CardDto>> Create([FromBody] CardUpsertDto dto)
    {
        var number = dto.CardNumber.Trim();
        if (await _db.Cards.AnyAsync(c => c.CardNumber == number))
        {
            return Conflict(new { error = $"Card number '{number}' already exists." });
        }

        var card = new Card { CreatedAtUtc = DateTime.UtcNow };
        card.ApplyUpsert(dto);
        card.UpdatedAtUtc = DateTime.UtcNow;

        _db.Cards.Add(card);
        await _db.SaveChangesAsync();
        await _history.LogAsync("Created", CurrentUser, $"#{card.CardNumber} {card.PlayerName}", card);

        return CreatedAtAction(nameof(GetById), new { id = card.Id }, card.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CardDto>> Update(int id, [FromBody] CardUpsertDto dto)
    {
        var card = await _db.Cards.FirstOrDefaultAsync(c => c.Id == id);
        if (card is null)
        {
            return NotFound();
        }

        var number = dto.CardNumber.Trim();
        if (!string.Equals(card.CardNumber, number, StringComparison.Ordinal)
            && await _db.Cards.AnyAsync(c => c.CardNumber == number && c.Id != id))
        {
            return Conflict(new { error = $"Card number '{number}' already exists." });
        }

        var wasForSale = card.IsForSale;
        card.ApplyUpsert(dto);
        card.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var action = wasForSale switch
        {
            false when card.IsForSale => "Listed",
            true when !card.IsForSale => "Unlisted",
            _ => "Updated"
        };
        await _history.LogAsync(action, CurrentUser, $"#{card.CardNumber} {card.PlayerName}", card);

        return Ok(card.ToDto());
    }

    [HttpPost("{id:int}/sell")]
    public async Task<ActionResult<CardDto>> Sell(int id, [FromBody] SellRequest request)
    {
        var card = await _db.Cards.FirstOrDefaultAsync(c => c.Id == id);
        if (card is null)
        {
            return NotFound();
        }
        if (card.IsSold)
        {
            return Conflict(new { error = "Card is already marked as sold." });
        }

        card.IsSold = true;
        card.IsForSale = false;
        card.SoldAtUtc = DateTime.UtcNow;
        card.SoldPrice = request.SoldPrice ?? card.Price;
        card.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var note = string.IsNullOrWhiteSpace(request.Note) ? string.Empty : $" ({request.Note.Trim()})";
        await _history.LogAsync("Sold", CurrentUser, $"Manual sale for {card.SoldPrice:C}{note}", card);

        return Ok(card.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var card = await _db.Cards.FirstOrDefaultAsync(c => c.Id == id);
        if (card is null)
        {
            return NotFound();
        }

        // Persist the audit entry first; the FK is configured to null CardId on delete.
        await _history.LogAsync("Deleted", CurrentUser, $"#{card.CardNumber} {card.PlayerName}", card);
        _db.Cards.Remove(card);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
