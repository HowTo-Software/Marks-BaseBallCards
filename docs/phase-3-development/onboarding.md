# Frontend onboarding

**Status:** instructions derived from repository configuration.
**Updated:** October 3, 2026.

## Prerequisites

For public frontend development, install the **.NET 10 SDK** and use a browser that
supports WebAssembly, ES modules, native dialogs and the browser APIs used in
[ui.js](../../src/Client/wwwroot/js/ui.js). No npm install, SQL instance or Stripe
credentials are required to preview the archive.

Use Git for contributions. Docker/Compose and an approved external SQL environment
are needed only for the existing containerized integration. Backend configuration
and staff credentials must come from Hank or the maintainer.

## Mode A: standalone frontend preview

Run these commands at the repository root:

```powershell
dotnet --version
dotnet restore src/Client/MarksBaseballCards.Client.csproj
dotnet build src/Client/MarksBaseballCards.Client.csproj --no-restore
dotnet run --project src/Client --no-launch-profile --urls http://localhost:5248
```

Open [localhost:5248](http://localhost:5248). Use Ctrl+C in the terminal to stop it.
The explicit URL avoids relying on IDE launch settings. The committed client
profiles also define HTTP 5248 and HTTPS 7190.

### What works in this mode?

| Available from local client assets | Requires the API |
| --- | --- |
| Home, collection story and 64-record archive | Marketplace listings and listing details |
| Archive search, filters, sorting, views and dialogs | Staff sign-in, inventory changes and statistics/history |
| Theme/language controls and public motion | Checkout creation, payment status and fulfillment |

An unavailable marketplace or sign-in message is expected without an API at this
origin. Do not insert fake listings or success responses to hide that state.

## Mode B: API-hosted frontend integration

Use a development database and configured staff/payment services supplied by Hank.
The required setting names and current seeding behavior are in
[backend-reference.md](../phase-5-deployment/backend-reference.md).

For a local process, supply configuration through the existing ASP.NET configuration
sources, such as process environment variables. A root `.env` file is loaded by
Compose; this code does not automatically load it for `dotnet run`.

Once those values are configured:

```powershell
dotnet restore src/Api/MarksBaseballCards.Api.csproj
dotnet run --project src/Api --launch-profile http
```

Open [localhost:5238](http://localhost:5238). The API serves both the frontend and
`/api` requests. Startup applies migrations/seeding, so use an approved development
database with suitable permissions. It can fail before serving the page if SQL is
unavailable. The HTTPS API profile declares 7272; certificate setup is outside this
frontend contribution.

Opening the standalone client at 5248 while the API runs at 5238 does **not** wire
them together. There is no client API URL override or proxy in the current source.

For the Docker path, see the [deployment guide](../phase-5-deployment/deployment-guide.md);
do not assume the sample private SQL address is available on your machine.

## Find the implementation

- Start with [technical design](../phase-2-design/technical-design.md) for route and component ownership.
- Read [themes and languages](themes-and-languages.md) before changing copy or palettes.
- Read [data meaning](../phase-2-design/data-model.md) before changing labels or progress metrics.
- Use [coding conventions](coding-standards.md) and the [manual review cases](../phase-4-testing/test-cases.md).
- Follow [Git workflow](git-workflow.md) for a reviewable change.

## Common setup failures

If a port is already in use, stop your existing preview or choose an available local
URL deliberately. If the archive fails, inspect the `data/collection.json` request.
If the UI is old after a rebuild, reload with browser cache disabled and compare the
served assets to the expected revision. More diagnostics are in the
[runbook](../phase-6-operations/runbook.md).

No automated test project exists. The committed workflow builds the API for pull
requests but does not test browser behavior. A build is a compilation check, not
confirmation of staff operations or payment.
