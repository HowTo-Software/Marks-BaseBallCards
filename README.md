# ⚾ Mark's Baseball Cards

A .NET 10 web application for the **1991 Topps** baseball card set — browse a public
marketplace, buy cards online via **Stripe**, manage the collection as an admin, and
review statistics & history as a system admin. Everything runs in Docker and connects
to an existing T-SQL (SQL Server) database.

For code-level security findings, exact file locations, and remediation guidance, see the [Security Validation and Remediation Guide](SECURITY_REVIEW.md).

## Frontend experience

The frontend now has an editorial identity by HTS, with a public collection archive,
live marketplace, collection story, and matching staff workspaces. Motion is adapted
from OriginKit **Stagger Text Rise** and Skiper UI **Card stack scroll**.

Use the header controls to switch between light/dark themes and **English, Brazilian
Portuguese or Spanish**. Choices are saved in your browser. Prices remain in USD.
See [Themes and Languages](docs/THEMES_AND_LANGUAGES.md) for coverage and implementation.

For the component sources, licensing, data notes and implementation details, see
[Frontend Experience and Motion References](docs/UI_REFERENCES.md).

To preview the frontend with a .NET 10 SDK:

```powershell
dotnet run --project src/Client --no-launch-profile --urls http://localhost:5248
```

Open <http://localhost:5248>. The original 64-record archive works in this standalone
preview. The live marketplace, staff sign-in and payments need the configured API,
SQL Server and Stripe services described below. The API serves the client and API
from one origin; the standalone preview does not proxy API requests.

## Architecture

| Project | Type | Role |
|---|---|---|
| `src/Client` | Blazor WebAssembly | C# frontend (public + admin UI) |
| `src/Api` | ASP.NET Core Web API | REST API + JWT auth + Stripe; also hosts the WASM client |
| `src/Shared` | Class library | DTOs shared by client and API |

Data access uses **EF Core 10** against SQL Server. The API container serves both the
API (`/api/...`) and the compiled Blazor client from a single origin.

### Three experiences
- **Public (anonymous):** original collection archive, live marketplace and Stripe Checkout.
- **Admin (`Admin` role):** full CRUD over the collection, list/unlist, record sales.
- **System admin (`SystemAdmin` role):** statistics dashboard and full activity history.

## Prerequisites
- Docker + Docker Compose
- An existing SQL Server reachable at `192.168.1.212:1433`
- (Optional) A Stripe account for online payments

## Configuration
Copy the example env file and fill in real values (it is gitignored):

```powershell
Copy-Item .env.example .env
```

At minimum set `ConnectionStrings__Default`, a strong `Jwt__Key` (≥ 32 chars), and the
two seed passwords. Add Stripe keys to enable buying.

## Run with Docker

```powershell
docker compose up --build -d
```

Then open <http://localhost:8090> (or `http://<docker-host>:8090`, e.g.
<http://192.168.1.202:8090>). On first start the API creates the
`MarksBaseballCards` database, applies migrations, and seeds the 64 cards plus the two
admin accounts.

## Run locally (development)

```powershell
# set a dev connection string (PowerShell)
$env:ConnectionStrings__Default = "Server=192.168.1.212,1433;Database=MarksBaseballCards;User Id=sa;Password=...;TrustServerCertificate=True"
dotnet run --project src/Api
```

The dev `Jwt:Key` is preset in `appsettings.Development.json`.

## Default accounts
Created on first run from the seed passwords in `.env`:

| Username | Role | Password |
|---|---|---|
| `admin` | Admin | `Seed__AdminPassword` |
| `sysadmin` | SystemAdmin | `Seed__SystemAdminPassword` |

**Change these before any real deployment.**

## Login security
- PBKDF2 password hashing (`PasswordHasher`)
- Per-IP rate limiting on `/api/auth/login`
- Account lockout after 5 failed attempts (15 min)
- Sign-in error handling and dummy password verification; see SEC-07 in the security guide for the remaining account-disclosure concern.
- Short-lived HMAC-SHA256 JWTs with issuer/audience validation

## Stripe
1. Put your secret key in `Stripe__SecretKey` (test key `sk_test_...` is fine).
2. For fulfillment, configure a webhook to `POST /api/checkout/webhook` for the
   `checkout.session.completed` event and set `Stripe__WebhookSecret`.
   Locally you can use the Stripe CLI:
   ```powershell
   stripe listen --forward-to http://localhost:8090/api/checkout/webhook
   ```
   (As a fallback, the success page also finalizes paid orders idempotently.)

## Database migrations
```powershell
dotnet dotnet-ef migrations add <Name> --project src/Api --output-dir Data/Migrations
```
Migrations are applied automatically at API startup.
