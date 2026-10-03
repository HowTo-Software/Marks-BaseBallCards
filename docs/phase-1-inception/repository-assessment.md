# Repository assessment and Hank handoff

**Status:** source inventory before documentation edits. **Updated:** October 3, 2026.
**Reviewed revision:** `39a8ecc90bcca4a0eb0d109360f7bae42d4f1e76`.

The repository and the local `best-document` structure were inspected before
reorganizing the documentation. The scope requested by the workspace owner is the
frontend already delivered. This assessment records the backend only as it appears
in the checked-out source. It cannot confirm Hank's restructuring outside this tree.

## Confirmed inventory

| Area | Evidence | What can be documented now |
| --- | --- | --- |
| Solution and technologies | [Solution](../../MarksBaseballCards.slnx), [client project](../../src/Client/MarksBaseballCards.Client.csproj), [API project](../../src/Api/MarksBaseballCards.Api.csproj) | .NET 10 Client/API/Shared; Razor/CSS/native JS frontend; EF Core SQL Server and Stripe server dependencies |
| Frontend routes and state | [Pages](../../src/Client/Pages), [layout](../../src/Client/Layout), [services](../../src/Client/Services) | Public and role-specific workflows, display preferences, errors and component behavior |
| Client data | [collection.json](../../src/Client/wwwroot/data/collection.json), [shared DTOs](../../src/Shared/Models) | 64-record archive; separate live inventory contracts; original illustrations |
| Database boundary | [AppDbContext](../../src/Api/Data/AppDbContext.cs), [migrations](../../src/Api/Data/Migrations), [DbSeeder](../../src/Api/Data/DbSeeder.cs) | SQL Server dependency, migration-at-startup and seeded records/users; complete restructured schema remains Hank's work |
| API and integrations | [controllers](../../src/Api/Controllers), [ApiClient](../../src/Client/Services/ApiClient.cs) | Same-origin REST calls, server roles, hosted Stripe redirect and status polling |
| Authentication | [client auth](../../src/Client/Auth), [API Program](../../src/Api/Program.cs), [AuthService](../../src/Api/Auth/AuthService.cs) | Browser bearer storage; API signature/issuer/audience/lifetime validation and role enforcement |
| Packaging and deploy | [Dockerfile](../../src/Api/Dockerfile), [Compose](../../docker-compose.yml), [.env.example](../../.env.example) | API publishes/serves client, one web container, external SQL; no proof of live infrastructure |
| Development history | [frontend commit](https://github.com/HowTo-Software/Marks-BaseBallCards/commit/39a8ecc90bcca4a0eb0d109360f7bae42d4f1e76), [baseline](https://github.com/HowTo-Software/Marks-BaseBallCards/commit/b08233bb6f6d38f18b64270e2ba0e0b88579364d) | Frontend delivery and original application baseline; no version tags in reviewed history |
| Quality evidence | [existing references](../phase-2-design/ui-references.md), [screenshots](../screenshots/README.md), [build/deploy workflow](../phase-5-deployment/ci-cd.md) | Earlier client build and public visual inspection; current workflow builds the API but no automated test suite is committed |
| Security | [preserved review](../security/security-review.md) | SEC-01–10 and DATA-01, recommendations and limits; source remediation of login return URL |
| Ownership and license | Repository URL and original root files | Repository under HowTo-Software; no project license found; licensing decision pending owner |

## Existing documentation preserved

The original security guide retains its findings and exact source locations.
Original README references use the immutable frontend revision, since root setup
content has been reorganized. Motion source hashes, attribution and usage notes
remain in the visual guide. The theme/language guide and preview gallery remain
available. Short entries at the former guide paths preserve existing links.

No application behavior was changed for this reorganization. The template's
`.github` directory, editor history, chat sessions, transcripts and logs were not copied.

## Handoff items for Hank / repository maintainer

| Topic | Current frontend dependency | Documentation or evidence still needed |
| --- | --- | --- |
| Backend restructuring | DTOs and routes listed in [API contracts](../phase-2-design/api-specification.md) | Confirm the new contracts, roles and migration/compatibility plan before updating the client |
| Database | API startup requires SQL; client never connects directly | Actual schema, permissions, connection procedure, data import policy and backup/restore runbook |
| Staff accounts | Existing login endpoint and `Admin`/`SystemAdmin` roles | Account provisioning, rotation/recovery, role policy and live authenticated review |
| Checkout and fulfillment | Server-priced session, Stripe redirect, status endpoint | Reservation/concurrent buyer handling, webhook/event processing, reconciliation and delivery/shipping policy |
| Production deployment | API-hosted client and existing Compose web service | Real domain, TLS/proxy, secret storage, host topology and approved deployment/rollback procedure |
| CI/CD and tests | API build/deploy workflow exists; no automated test suite | Runner/host ownership, integration environments, test coverage and required checks |
| Operations | Browser diagnostics and visible UI states | Telemetry, alert owners, service objectives, incident process and recovery targets |
| Security remediation | Open findings recorded in the prior review | Revalidation after backend changes, current package audit and effective runtime settings |
| Licensing and project policies | Component/font notices retained | Project license, private security contact, supported versions and contribution/merge policies |

## Boundaries of this assessment

No production database, private Stripe configuration, hosted deployment, external
CI system or incident platform was inspected. No application build or tests were
run for this documentation task. Historical validation is attributed to its original
guide or commit. A missing committed workflow does not prove that no external
pipeline exists; its details remain pending.
