using MarksBaseballCards.Api.Data;
using MarksBaseballCards.Api.Entities;
using MarksBaseballCards.Api.Mapping;
using MarksBaseballCards.Shared.Common;
using MarksBaseballCards.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarksBaseballCards.Api.Controllers;

[ApiController]
[Route("api/statistics")]
[Authorize(Roles = Roles.SystemAdmin)]
public class StatisticsController : ControllerBase
{
    private readonly AppDbContext _db;

    public StatisticsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<StatisticsDto>> Get()
    {
        var cards = await _db.Cards.AsNoTracking().ToListAsync();
        var recent = await _db.History.AsNoTracking()
            .OrderByDescending(h => h.ChangedAtUtc)
            .Take(15)
            .ToListAsync();

        var stats = new StatisticsDto
        {
            TotalCards = cards.Count,
            CardsInPlastic = cards.Count(c => c.InPlastic),
            RookieCards = cards.Count(c => c.IsRookie),
            RoyalsCards = cards.Count(c => c.IsRoyals),
            TotalDoubles = cards.Sum(c => c.Doubles),
            CardsForSale = cards.Count(c => c.IsForSale),
            CardsSold = cards.Count(c => c.IsSold),
            ForSaleValue = cards.Where(c => c.IsForSale).Sum(c => c.Price ?? 0m),
            TotalSalesValue = cards.Where(c => c.IsSold).Sum(c => c.SoldPrice ?? 0m),
            GeneratedAtUtc = DateTime.UtcNow,
            SetProgress = Enumerable.Range(1, 6).Select(n => new SetProgressDto
            {
                SetNumber = n,
                TotalCards = cards.Count,
                CardsFiled = cards.Count(c => IsInSet(c, n))
            }).ToList(),
            RecentActivity = recent.Select(h => h.ToDto()).ToList()
        };

        return Ok(stats);
    }

    private static bool IsInSet(Card c, int setNumber) => setNumber switch
    {
        1 => c.InSet1,
        2 => c.InSet2,
        3 => c.InSet3,
        4 => c.InSet4,
        5 => c.InSet5,
        6 => c.InSet6,
        _ => false
    };
}
