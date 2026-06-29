using MarksBaseballCards.Api.Data;
using MarksBaseballCards.Api.Mapping;
using MarksBaseballCards.Shared.Common;
using MarksBaseballCards.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarksBaseballCards.Api.Controllers;

[ApiController]
[Route("api/history")]
[Authorize(Roles = Roles.SystemAdmin)]
public class HistoryController : ControllerBase
{
    private readonly AppDbContext _db;

    public HistoryController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>Returns the most recent audit-trail entries (newest first).</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CardHistoryDto>>> Get(
        [FromQuery] int take = 200, [FromQuery] string? action = null)
    {
        take = Math.Clamp(take, 1, 1000);
        var query = _db.History.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(action))
        {
            var a = action.Trim();
            query = query.Where(h => h.Action == a);
        }

        var entries = await query
            .OrderByDescending(h => h.ChangedAtUtc)
            .Take(take)
            .ToListAsync();

        return Ok(entries.Select(h => h.ToDto()));
    }
}
