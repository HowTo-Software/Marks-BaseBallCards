# Mark's Baseball Cards

An application for exploring Mark's 1991 Topps collection, viewing live sale listings,
buying through Stripe Checkout, and managing inventory through staff workspaces.
The HTS frontend uses Blazor WebAssembly with an editorial visual identity, light and
dark themes, and English, Brazilian Portuguese and Spanish interfaces.

**Documentation scope:** the frontend contribution delivered in commit
[`39a8ecc`](https://github.com/HowTo-Software/Marks-BaseBallCards/commit/39a8ecc90bcca4a0eb0d109360f7bae42d4f1e76).
Backend code is described where it affects the client. Hank's backend restructuring,
production infrastructure and operational procedures require his documentation and
validation. See the [assessment and handoff](docs/phase-1-inception/repository-assessment.md).

## Preview the frontend

Install the .NET 10 SDK, open a terminal at the repository root, then run:

```powershell
dotnet restore src/Client/MarksBaseballCards.Client.csproj
dotnet run --project src/Client --no-launch-profile --urls http://localhost:5248
```

Open [localhost:5248](http://localhost:5248). Home, the collection archive and the
collection story work with local assets. The marketplace, staff login, inventory,
dashboard and payments require the configured backend.

The client sends API requests to its own origin. The standalone preview does not
proxy requests to a separately started API. For the integrated experience, use the
frontend served by the API, as explained in [onboarding](docs/phase-3-development/onboarding.md)
and the [backend reference](docs/phase-5-deployment/backend-reference.md).

## What is in the application?

| Experience | Routes | Data and access |
| --- | --- | --- |
| Introduction and collection story | `/`, `/about` | Local content and original illustrations |
| Original archive | `/collection` | 64 historical records; search, filters, sorting, grid/list views and dialogs |
| Live marketplace | `/marketplace` | API listings, condition, notes, USD prices and Stripe Checkout |
| Staff access | `/login` | Owner-provided accounts; no public registration flow |
| Inventory workspace | `/admin` | `Admin` role; create/edit, list, record sales and delete |
| Statistics and activity | `/system` | `SystemAdmin` role; current inventory, six filing sets and up to 200 history entries |
| Payment status | `/buy/success` | Confirms payment only when the API reports `paid` |

`Admin` and `SystemAdmin` are distinct roles. The browser's route checks help guide
navigation; the API enforces authorization.

### How to interpret the collection

The archive is a snapshot of the supplied checklist, separate from current inventory.
It is not proof that a card is in stock or for sale. Covers are original record
illustrations, rather than physical card scans or grading certificates. Rookie and
Royals markers retain the collector's annotations. Filing percentages describe the
records currently in the database. Language selection changes formatting and copy;
prices remain in USD.

## Repository map

| Location | Responsibility |
| --- | --- |
| [src/Client](src/Client) | Blazor pages, components, browser authentication, localization, CSS and JavaScript |
| [src/Shared](src/Shared) | DTOs and role names shared with the client |
| [src/Api](src/Api) | ASP.NET Core API, SQL Server/EF Core, JWT, Stripe and hosting of the client |
| [docker-compose.yml](docker-compose.yml) | Existing web container configuration; no database service |
| [docs](docs/README.md) | Documentation organized by lifecycle phase |

There is no npm build pipeline in this repository. The frontend uses Razor, C#, CSS
and native browser JavaScript. Dependencies and hosting boundaries are detailed in
[architecture](docs/phase-2-design/architecture.md).

## Documentation

Start with the [documentation index](docs/README.md), or go directly to:

- [Environment and development setup](docs/phase-3-development/onboarding.md)
- [Frontend design and source map](docs/phase-2-design/technical-design.md)
- [Client/API contracts](docs/phase-2-design/api-specification.md)
- [Themes and translation maintenance](docs/phase-3-development/themes-and-languages.md)
- [Review evidence and manual checks](docs/phase-4-testing/test-plan.md)
- [Publishing the frontend with the existing host](docs/phase-5-deployment/deployment-guide.md)
- [Security findings and recommendations](docs/security/security-review.md)
- [User guide](docs/user/user-guide.md)
- [Preview screenshots](docs/screenshots/README.md)

## Local API development example

```powershell
$env:ConnectionStrings__Default = "Server=192.168.1.212,1433;Database=Website_Application_MarksBaseballCardsDb;User Id=sa;Password=...;TrustServerCertificate=True"
dotnet run --project src/Api
```

## Current validation and release limits

The October 2 implementation notes record a successful client build and public UI
inspection in Chromium, including themes, languages and mobile layouts. They also
record an existing API dependency warning. This documentation reorganization does
not constitute a new build, security audit or deployment verification.

No automated test suite is committed. The GitHub Actions workflow builds the API on
pull requests and deploys on pushes to `main` or manual dispatch; it does not verify
browser behavior. Authenticated staff mutations, a production database, real payments
and fulfillment were not exercised during the frontend review. See the
[CI/CD notes](docs/phase-5-deployment/ci-cd.md) and the
[handoff](docs/phase-1-inception/repository-assessment.md).

## Contributions, security and licensing

See [CONTRIBUTING.md](CONTRIBUTING.md), [SECURITY.md](SECURITY.md) and
[CHANGELOG.md](CHANGELOG.md). The repository had no owner-selected project license
at the reviewed revision; [LICENSE](LICENSE) records that pending decision and does
not grant reuse rights. Third-party font notices and component attribution are
preserved in the [licensing notes](docs/security/compliance.md).
