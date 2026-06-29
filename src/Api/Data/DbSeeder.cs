using System.Reflection;
using System.Text.Json;
using MarksBaseballCards.Api.Entities;
using MarksBaseballCards.Shared.Common;
using MarksBaseballCards.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MarksBaseballCards.Api.Data;

/// <summary>Seeds the two admin accounts and the 1991 Topps card data on first run.</summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, IConfiguration config, ILogger logger)
    {
        await SeedUsersAsync(db, config, logger);
        await SeedConfiguredStaffAsync(db, config, logger);
        await SeedCardsAsync(db, logger);
    }

    private static async Task SeedUsersAsync(AppDbContext db, IConfiguration config, ILogger logger)
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var hasher = new PasswordHasher<AppUser>();
        var now = DateTime.UtcNow;

        var admin = new AppUser
        {
            Username = config["Seed:AdminUsername"] ?? "admin",
            Role = Roles.Admin,
            CreatedAtUtc = now
        };
        admin.PasswordHash = hasher.HashPassword(admin, config["Seed:AdminPassword"] ?? "ChangeMe!Admin123");

        var systemAdmin = new AppUser
        {
            Username = config["Seed:SystemAdminUsername"] ?? "sysadmin",
            Role = Roles.SystemAdmin,
            CreatedAtUtc = now
        };
        systemAdmin.PasswordHash = hasher.HashPassword(systemAdmin, config["Seed:SystemAdminPassword"] ?? "ChangeMe!System123");

        db.Users.AddRange(admin, systemAdmin);
        await db.SaveChangesAsync();
        logger.LogInformation("Seeded admin accounts: {Admin}, {SystemAdmin}.", admin.Username, systemAdmin.Username);
    }

    private static async Task SeedCardsAsync(AppDbContext db, ILogger logger)
    {
        if (await db.Cards.AnyAsync())
        {
            return;
        }

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.Data.seed-cards.json";
        await using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded seed resource '{resourceName}' was not found.");

        var seed = await JsonSerializer.DeserializeAsync<List<CardUpsertDto>>(
            stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        var now = DateTime.UtcNow;
        var cards = seed.Select(c => new Card
        {
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
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        }).ToList();

        db.Cards.AddRange(cards);
        await db.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} cards from 1991 Topps data.", cards.Count);
    }

    /// <summary>
    /// Reconciles managed staff accounts defined in configuration (Seed:StaffUsers).
    /// Runs every startup and is idempotent: creates the account if missing, otherwise
    /// updates its password and role to match config when they differ. Entries without a
    /// password are skipped (so a placeholder won't create or clobber an account).
    /// </summary>
    private static async Task SeedConfiguredStaffAsync(AppDbContext db, IConfiguration config, ILogger logger)
    {
        var staff = config.GetSection("Seed:StaffUsers").Get<List<StaffSeed>>() ?? new();
        if (staff.Count == 0)
        {
            return;
        }

        var hasher = new PasswordHasher<AppUser>();
        var changes = 0;

        foreach (var entry in staff)
        {
            var username = entry.Username?.Trim();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(entry.Password))
            {
                logger.LogWarning("Skipping staff entry with a missing username or password ({Username}).",
                    string.IsNullOrWhiteSpace(username) ? "(none)" : username);
                continue;
            }

            var role = NormalizeRole(entry.Role);
            var existing = await db.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (existing is null)
            {
                var user = new AppUser { Username = username, Role = role, CreatedAtUtc = DateTime.UtcNow };
                user.PasswordHash = hasher.HashPassword(user, entry.Password);
                db.Users.Add(user);
                changes++;
                logger.LogInformation("Seeded staff account {Username} with role {Role}.", username, role);
                continue;
            }

            // Reconcile an existing managed account to match configuration.
            var updated = false;
            if (hasher.VerifyHashedPassword(existing, existing.PasswordHash, entry.Password)
                == PasswordVerificationResult.Failed)
            {
                existing.PasswordHash = hasher.HashPassword(existing, entry.Password);
                existing.FailedLoginCount = 0;
                existing.LockoutEndUtc = null;
                updated = true;
                logger.LogInformation("Updated password for managed account {Username}.", username);
            }
            if (!string.Equals(existing.Role, role, StringComparison.Ordinal))
            {
                existing.Role = role;
                updated = true;
                logger.LogInformation("Updated role for managed account {Username} to {Role}.", username, role);
            }
            if (updated)
            {
                changes++;
            }
        }

        if (changes > 0)
        {
            await db.SaveChangesAsync();
        }
    }

    private static string NormalizeRole(string? role)
        => string.Equals(role, Roles.SystemAdmin, StringComparison.OrdinalIgnoreCase)
            ? Roles.SystemAdmin
            : Roles.Admin;

    private sealed class StaffSeed
    {
        public string? Username { get; set; }
        public string? Role { get; set; }
        public string? Password { get; set; }
    }
}
