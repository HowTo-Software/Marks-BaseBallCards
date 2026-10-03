# Backend integration reference for frontend developers

**Status:** existing source/configuration summary; Hank must validate restructured backend.
**Updated:** October 3, 2026.

This preserves the useful setup context from the original README without presenting
its sample LAN addresses or credentials as verified infrastructure.
[The original README at the frontend revision](https://github.com/HowTo-Software/Marks-BaseBallCards/blob/39a8ecc90bcca4a0eb0d109360f7bae42d4f1e76/README.md)
remains available for historical references.

## What the frontend needs

For live listings, staff workspaces or checkout, run the API-hosted client with the
appropriate services. The API does not finish startup until SQL migration/seeding
succeeds. Its startup retry loop allows 12 attempts with five-second delays between
failed attempts.

The current server uses SQL Server through EF Core; API authentication uses JWTs;
Stripe Checkout is created server-side. No frontend environment variable should
contain a connection string, seed password, signing key or Stripe secret.

## Existing configuration names

| Setting / environment variable | Use confirmed in source |
| --- | --- |
| `ConnectionStrings:Default` / `ConnectionStrings__Default` | SQL connection; required at startup |
| `Jwt:Key` / `Jwt__Key` | Signing key; startup enforces minimum 32 characters |
| `Jwt:Issuer`, `Jwt:Audience` / double-underscore equivalents | Token issuer/audience validation |
| `Seed:AdminPassword` / `Seed__AdminPassword` | Initial built-in Admin password |
| `Seed:SystemAdminPassword` / `Seed__SystemAdminPassword` | Initial built-in SystemAdmin password |
| `Seed:StaffUsers` / indexed `Seed__StaffUsers__0__Username`, `Role`, `Password` | Optional managed accounts reconciled on startup |
| `Stripe:SecretKey` / `Stripe__SecretKey` | Server Checkout calls; blank disables checkout configuration |
| `Stripe:WebhookSecret` / `Stripe__WebhookSecret` | Signature verification for webhook requests |
| `Stripe:Currency` / `Stripe__Currency` | Defaults to `usd`; frontend labels assume USD |
| `Stripe:PublishableKey` / `Stripe__PublishableKey` | Present in options/example, but this hosted-redirect frontend does not read or use it |

Sources: [.env.example](../../.env.example), [API Program](../../src/Api/Program.cs),
[JwtOptions](../../src/Api/Auth/JwtOptions.cs), [StripeOptions](../../src/Api/Payments/StripeOptions.cs),
[DbSeeder](../../src/Api/Data/DbSeeder.cs).

The root `.env` is gitignored and used by Compose's `env_file`. It is not
automatically imported into a local `dotnet run` process. Provide local settings
through ASP.NET configuration, coordinated with Hank. Do not publish those values
in documentation or commit them.

## Data and account startup behavior

The existing seeder imports the embedded 64-record seed only if the Cards table has
no rows. A populated database is not refreshed from that file on every restart.

The built-in `admin` (`Admin`) and `sysadmin` (`SystemAdmin`) accounts are seeded
when the user table is empty. Changing their initial seed-password variables later
does not rotate those existing accounts. Optional managed-staff entries with
nonblank passwords are reconciled on startup and can change credentials/roles.

The source contains password/key defaults and permissive SQL samples that are
recorded as findings, rather than safe deployment instructions. See SEC-01 and
SEC-04 in the [security guide](../security/security-review.md). Actual account
provisioning and recovery remain Hank's procedures.

## Stripe and purchase semantics

The browser requests a session for a live card ID; the server reads its price.
Checkout returns a hosted URL/session ID. The browser does not collect card-payment
details through a local form.

The current server exposes `POST /api/checkout/webhook`, verifies
`Stripe-Signature`, and handles paid `checkout.session.completed` events. The
status endpoint also invokes server-side paid fulfillment when a customer returns.
The UI polling is therefore useful but cannot replace reliable webhook fulfillment.

For the existing Docker mapping, the original local forwarding command was:

```powershell
stripe listen --forward-to http://localhost:8090/api/checkout/webhook
```

This command requires an installed/authenticated Stripe CLI and an approved Stripe
test environment. The signing secret issued for that listener must match the API
configuration. No Stripe account or webhook delivery was verified for this
documentation task.

Concurrent purchases, delayed-payment events, allowed host/proxy configuration and
payment reconciliation remain open backend topics in SEC-02, SEC-06 and SEC-09.

## Database and backend changes

The API calls `MigrateAsync` before seeding. Existing migration files are under
[Data/Migrations](../../src/Api/Data/Migrations). The root
[dotnet-tools.json](../../dotnet-tools.json) declares EF tooling, but is outside the
conventional `.config` location. This frontend guide does not prescribe a new
migration-authoring workflow; Hank must document the correct manifest invocation
and migration/recovery process for his restructured backend.

OpenAPI is mapped only in Development in the current Program. The full API design,
SQL permissions and production configuration are pending Hank. Use the
[client contract table](../phase-2-design/api-specification.md) when coordinating changes.
