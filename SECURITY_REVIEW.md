# Security Validation and Remediation Guide

**Project:** Mark's Baseball Cards<br />
**Review date:** October 1, 2026<br />
**Review type:** Source-code security and data-accuracy review

This guide records what I checked, what the code currently does, why each finding matters, and how I recommend addressing it. The goal is to give the project a clear, practical path toward a safer release.

The October 1 assessment was a read-only review of the application code and configuration. I did not change application behavior, modify the frontend, connect to the SQL Server or Stripe account, or run automated tests. No real `.env` file was present in the workspace, so deployment secrets and live service settings could not be checked.

## Frontend follow-up — October 2, 2026

The frontend has since been refreshed. **SEC-10 is remediated in the current login page**, and the dashboard now labels percentages as filing progress over the current inventory. Public record artwork and the original archive are explicitly distinguished from card scans and live sale listings. See [Frontend Experience and Motion References](docs/UI_REFERENCES.md) for the changes and runtime limits. The other backend and deployment findings remain open.

## Summary

The project already has several good foundations: passwords are hashed with ASP.NET Core's `PasswordHasher`, admin API routes enforce roles, JWT signatures and issuer/audience/lifetime are validated, and the Stripe webhook verifies its signature. The main concerns are the production defaults, sale concurrency, transport security, and a vulnerable transitive package.

I recommend fixing the High findings before exposing the application outside a trusted network. The data review also found that the 64 seeded records are internally consistent with the supplied workbook, but they do not represent the complete 1991 Topps base checklist.

## How to read priorities

- **High:** Resolve before an internet-facing or otherwise untrusted deployment.
- **Medium:** Fix during the next security hardening pass; some findings depend on deployment settings.
- **Data accuracy:** Clarify the product's intended meaning so displayed numbers are not mistaken for full-set completion.

## Findings

### SEC-01 — Known seed passwords and sample JWT key can be accepted

**Priority: High**

The seeder falls back to passwords written in source code when the seed password settings are absent. The `.env.example` also contains fixed `CHANGE_ME` values. The application checks only that the JWT key is at least 32 characters long; it does not reject the published sample value. As a result, copying the sample configuration without replacing every placeholder can create predictable admin credentials and a predictable token-signing key.

There is a second operational trap: the default accounts are seeded only when the user table is empty. Changing the seed password environment variables after those accounts have been created does not rotate their existing passwords. The optional managed-staff seeder does reconcile passwords on each startup, but the two built-in accounts are not managed that way by default.

**Code locations:**

- [`DbSeeder.cs:23`](src/Api/Data/DbSeeder.cs#L23), [`DbSeeder.cs:37`](src/Api/Data/DbSeeder.cs#L37), [`DbSeeder.cs:45`](src/Api/Data/DbSeeder.cs#L45)
- [`.env.example:12`](.env.example#L12), [`.env.example:17`](.env.example#L17), [`.env.example:18`](.env.example#L18)
- [`Program.cs:25`](src/Api/Program.cs#L25)
- [`appsettings.Development.json:9`](src/Api/appsettings.Development.json#L9) contains a source-controlled development signing key. That key is appropriate only for an isolated local environment.

**Recommended fix:**

1. In Production, fail startup unless unique seed passwords and a randomly generated JWT key are supplied.
2. Reject known placeholder values explicitly; a length check alone is not enough.
3. Remove hard-coded password fallbacks from the production path. If local development needs convenient defaults, restrict them to Development and bind the service to localhost.
4. Provide and document a safe rotation procedure for already-created accounts. Do not imply that changing the initial seed values rotates existing users.
5. Keep the development signing key out of any environment that can be reached by other machines.

**How to verify the fix:** Start a fresh Production instance with a missing value and with a `CHANGE_ME` value; both should fail before creating users or issuing tokens. Confirm that rotating a managed account changes its usable password without logging the new password.

### SEC-02 — One card can have multiple paid Checkout Sessions

**Priority: High**

The checkout endpoint checks that a card is listed and unsold, then creates a new Stripe Checkout Session. It does not reserve the card before the Stripe request and does not reject an existing open session. Two buyers can therefore create separate sessions and both pay for the same single listing.

Fulfillment then returns early when the card is already marked sold. That prevents a second database update, but it does not refund or otherwise reconcile a second successful payment. The `StripeCheckoutSessionId` field stores a session ID, but the create path does not use it to prevent concurrent sessions.

**Code locations:**

- [`CheckoutController.cs:50`](src/Api/Controllers/CheckoutController.cs#L50) checks current sale state.
- [`CheckoutController.cs:89`](src/Api/Controllers/CheckoutController.cs#L89) creates a session without first reserving the item.
- [`Card.cs:33`](src/Api/Entities/Card.cs#L33) stores the most recently assigned Checkout Session ID.
- [`CheckoutController.cs:173`](src/Api/Controllers/CheckoutController.cs#L173) ignores fulfillment when the card is already sold.
- [`CheckoutController.cs:178`](src/Api/Controllers/CheckoutController.cs#L178) marks the card sold only after payment.

**Recommended fix:**

1. Represent the purchase lifecycle explicitly, for example `Available`, `CheckoutPending`, `Sold`, and `CheckoutExpired`.
2. Atomically reserve the card in the database before creating a Checkout Session. Use a transaction or a conditional update so only one request can reserve it.
3. Set and handle session expiration. Release a reservation when a session expires or payment fails.
4. Record each order/session and its Stripe event ID so retries are idempotent and an already-paid conflict can be investigated and refunded.
5. Use a Stripe idempotency key for retries of the same session-creation operation. Stripe documents idempotency for safely retrying POST requests, but an idempotency key by itself does not reserve inventory against two different buyers. See [Stripe's idempotent request guidance](https://docs.stripe.com/api/idempotent_requests).

**How to verify the fix:** In Stripe test mode, try to create two sessions for one card at nearly the same time. Only one buyer should be able to complete checkout for that inventory unit; every other outcome should be clearly reconciled, including a refund or an operator alert.

### SEC-03 — The documented web deployment serves authentication over HTTP

**Priority: High when reachable over an untrusted network**

The Docker image listens on plain HTTP, and Compose publishes that port as host port 8090. The README also documents `http://` URLs. If users can reach that port over a shared or public network, their login credentials and bearer tokens can be observed or altered in transit.

**Code locations:**

- [`Dockerfile:21`](src/Api/Dockerfile#L21) binds Kestrel to `http://+:8080`.
- [`docker-compose.yml:14`](docker-compose.yml#L14) publishes `8090:8080`.
- [`docker-compose.yml:17`](docker-compose.yml#L17) explicitly configures HTTP.
- [`README.md`](README.md) documents access using `http://`.

**Recommended fix:** Serve the public site through HTTPS. If TLS terminates at a reverse proxy, restrict direct access to port 8090, configure ASP.NET Core forwarded headers for only the trusted proxy addresses, and make the proxy enforce HTTPS. Add HSTS once the HTTPS deployment is confirmed. The entire authenticated session should use TLS, not only the sign-in request; see the [OWASP Session Management guidance](https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html).

**How to verify the fix:** From outside the trusted host, the app should be reachable only over HTTPS. Confirm that HTTP is redirected or blocked, and that requests cannot bypass the TLS proxy by connecting directly to the published container port.

### SEC-04 — SQL connection example uses `sa` and disables encryption

**Priority: High for a shared or production SQL Server**

The sample connection string uses the highly privileged `sa` account and explicitly sets `Encrypt=False`. Depending on SQL Server configuration, this can leave database traffic unencrypted. If the server forces encryption, `TrustServerCertificate=True` allows encryption without validating the server certificate, which leaves the connection open to server-impersonation risks.

**Code locations:**

- [`.env.example:8`](.env.example#L8)
- [`README.md:80`](README.md#L80) also demonstrates `User Id=sa` and `TrustServerCertificate=True`.

Microsoft's [ADO.NET connection-string documentation](https://learn.microsoft.com/en-us/sql/connect/ado-net/connection-string-syntax?view=sql-server-ver17) explains that `Encrypt=False` can mean no encryption when the server does not require it, and that `TrustServerCertificate=True` bypasses certificate-chain validation when TLS is used.

**Recommended fix:** Create a dedicated application login with only the database permissions the app needs. For shared or production environments, use `Encrypt=True;TrustServerCertificate=False` with a certificate trusted by the container. Keep any certificate-bypass setting limited to isolated local development.

**How to verify the fix:** Inspect the effective runtime connection string without printing its password, confirm the application login is not `sa`/sysadmin, and verify that the SQL connection negotiates TLS with certificate validation.

### SEC-05 — A high-severity OpenAPI dependency advisory is present

**Priority: High dependency alert; direct exploitability in this app was not confirmed**

The NuGet vulnerability check resolved `Microsoft.OpenApi` **2.0.0** through `Microsoft.AspNetCore.OpenApi` **10.0.9**. The [GHSA advisory](https://github.com/advisories/GHSA-v5pm-xwqc-g5wc) marks the package version as affected: a crafted OpenAPI document with circular schema references can terminate a process during document parsing. The advisory lists `Microsoft.OpenApi` **2.7.5 or later** as patched for the 2.x line.

The API calls `AddOpenApi` and maps an OpenAPI endpoint in Development, but I did not find application code that parses OpenAPI documents supplied by users. So this is a confirmed vulnerable dependency, while a remotely reachable parser path was not confirmed in the current app.

**Code locations:**

- [`MarksBaseballCards.Api.csproj:12`](src/Api/MarksBaseballCards.Api.csproj#L12)
- [`Program.cs:77`](src/Api/Program.cs#L77), [`Program.cs:87`](src/Api/Program.cs#L87)

**Recommended fix:** Update the ASP.NET OpenAPI package and ensure the final resolved `Microsoft.OpenApi` version is at least 2.7.5, or use a compatible patched major version. Re-run the vulnerability check after restore; do not assume a top-level package update changed the resolved transitive version.

### SEC-06 — Stripe return URLs are built from the request Host header

**Priority: Medium**

The Checkout endpoint builds its success and cancel URLs using `Request.Scheme` and `Request.Host`. The app configuration allows every host with `AllowedHosts: "*"`. A caller who supplies an unexpected `Host` value can therefore influence the domain Stripe sends the customer back to after checkout. This can be used to create confusing or attacker-controlled return links.

**Code locations:**

- [`CheckoutController.cs:59`](src/Api/Controllers/CheckoutController.cs#L59), [`CheckoutController.cs:63`](src/Api/Controllers/CheckoutController.cs#L63), [`CheckoutController.cs:64`](src/Api/Controllers/CheckoutController.cs#L64)
- [`appsettings.json:8`](src/Api/appsettings.json#L8)

Microsoft's [ASP.NET Core host and proxy guidance](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-10.0) notes that a wildcard accepts all non-empty hosts and that unrestricted hosts can allow spoofed links.

**Recommended fix:** Add a required canonical public base URL such as `https://cards.example.com` and build Stripe return URLs from that setting. Restrict `AllowedHosts` to the real application domain. If running behind a proxy, trust forwarded headers only from known proxies and networks.

**How to verify the fix:** Send a checkout request with a forged `Host` header. The generated Stripe success and cancel URLs must still use the configured public domain.

### SEC-07 — Locked accounts receive a different login error

**Priority: Medium**

The original README claimed generic login errors without account disclosure. The README wording has since been corrected, but the backend still distinguishes locked accounts: an existing username receives a “temporarily locked” response, while an unknown username and a wrong password receive the generic error. After five failed attempts, the account is locked for 15 minutes. This reveals valid usernames and lets an unauthenticated caller deliberately lock an admin account.

**Code locations:**

- [`AuthService.cs:16`](src/Api/Auth/AuthService.cs#L16), [`AuthService.cs:56`](src/Api/Auth/AuthService.cs#L56)
- [`AuthService.cs:49`](src/Api/Auth/AuthService.cs#L49), [`AuthService.cs:70`](src/Api/Auth/AuthService.cs#L70)
- [`README.md:99`](README.md#L99), [`README.md:100`](README.md#L100)
- The login route opts into rate limiting at [`AuthController.cs:23`](src/Api/Controllers/AuthController.cs#L23). The per-IP policy is configured at [`Program.cs:61`](src/Api/Program.cs#L61) and keys requests from `Connection.RemoteIpAddress` at [`Program.cs:66`](src/Api/Program.cs#L66). `AuthController.cs:26` separately records the address in the audit history.

OWASP recommends the same public failure response for incorrect credentials, unknown accounts, and locked accounts, and warns that lockout can itself be abused for denial of service. See the [OWASP Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html).

**Recommended fix:** Return the same status and message for all login failures, including lockout. Use throttling that balances password guessing resistance with the risk of locking out a real admin; add MFA for administrative accounts if the app will be reachable beyond a trusted network. If deployed behind a proxy, configure trusted forwarded headers so per-IP limits use the real client IP rather than one shared proxy address.

### SEC-08 — The admin JWT is stored in browser `localStorage`

**Priority: Medium**

The token store writes the JWT to `localStorage`. Any JavaScript running on the same origin can read it. If an XSS bug is introduced now or later, the attacker can copy the admin token and use it until it expires. The token lifetime is 120 minutes, and logout removes it from the browser but does not revoke a copy already stolen.

**Code locations:**

- [`TokenStore.cs:5`](src/Client/Auth/TokenStore.cs#L5), [`TokenStore.cs:13`](src/Client/Auth/TokenStore.cs#L13), [`TokenStore.cs:15`](src/Client/Auth/TokenStore.cs#L15)
- [`JwtOptions.cs:14`](src/Api/Auth/JwtOptions.cs#L14)

The [OWASP guidance](https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html) advises against storing authentication tokens in browser storage because scripts can access it.

**Recommended fix:** For a security-focused deployment, consider a Backend-for-Frontend (BFF) with `HttpOnly`, `Secure`, and appropriate `SameSite` cookies, plus CSRF protections. If keeping a browser-held bearer token, reduce the exposure window, maintain strict output encoding and Content Security Policy, and document that logout does not invalidate a copied token.

### SEC-09 — Delayed Stripe payment events are not handled

**Priority: Medium when delayed payment methods are enabled**

The webhook only fulfills `checkout.session.completed` when the session is already `paid`. Stripe also sends `checkout.session.async_payment_succeeded` when a delayed payment method completes later. That event is not handled here. The success page polls for a few seconds and can call the status endpoint as a fallback, but the customer may close the browser before returning. In that case a valid delayed payment can remain unrecorded in the inventory.

**Code locations:**

- [`CheckoutController.cs:99`](src/Api/Controllers/CheckoutController.cs#L99), [`CheckoutController.cs:121`](src/Api/Controllers/CheckoutController.cs#L121)
- [`BuySuccess.razor:36`](src/Client/Pages/BuySuccess.razor#L36), [`BuySuccess.razor:39`](src/Client/Pages/BuySuccess.razor#L39)
- [`README.md:105`](README.md#L105), [`README.md:106`](README.md#L106), [`README.md:111`](README.md#L111)

Stripe documents the `checkout.session.async_payment_succeeded` event and recommends webhook-based fulfillment rather than relying only on a success-page redirect. See [Stripe's Checkout payment guidance](https://docs.stripe.com/payments/existing-customers?platform=web&ui=stripe-hosted).

**Recommended fix:** Handle `checkout.session.async_payment_succeeded` and `checkout.session.async_payment_failed` as well as `checkout.session.completed`. Keep signature verification, persist processed event IDs, and make fulfillment idempotent at the order level.

### SEC-10 — The post-login return address is now restricted to the app

**Original priority: Low to Medium · Status: Remediated in source on October 2, 2026**

The original login page accepted a query-string return address without restricting its destination. That allowed an external redirect after sign-in, creating an open-redirect/phishing risk.

The refreshed login page resolves the address against the application base URI, checks that the destination remains inside that base, rejects backslashes and avoids returning to the login route. An invalid destination falls back to the user's role-specific dashboard. The value returned to navigation contains only the accepted local path, query and fragment.

**Current code locations:**

- [`Login.razor:22`](src/Client/Pages/Login.razor#L22) — query-string input.
- [`Login.razor:44`](src/Client/Pages/Login.razor#L44) — destination selection.
- [`Login.razor:48`](src/Client/Pages/Login.razor#L48) — base-URI restriction and backslash rejection.

**Follow-up validation:** When a real staff account and backend are available, try external absolute addresses, protocol-relative addresses and backslash variations; each should land on the role-specific page. Confirm that legitimate local return paths still work. The current source compiles; this behavior has not been exercised against a live authenticated backend.

## Data accuracy and product wording

### DATA-01 — The seeded 64 cards are not the full 1991 Topps base set

The spreadsheet contains 64 data rows, and the JSON seed contains 64 rows. I compared the mapped card number, player name, plastic/set flags, doubles, rookie, and Royals fields across the workbook and seed: **there were no mismatches in the 12 mapped fields**.

That confirms the application seed matches its supplied workbook. It does not independently verify every name or classification against an authoritative card checklist. The [PSA CardFacts profile](https://www.psacard.com/cardfacts/baseball-cards/1991-topps/354) identifies 1991 Topps as a 792-card set and confirms the 40th-anniversary context. So the “40 Years of Baseball” wording is reasonable, while “complete set” progress needs a clearer data model.

**Code locations:**

- [`1991 Topps (40 years of Baseball).xlsx`](<1991 Topps (40 years of Baseball).xlsx>) — 64 data rows plus a header.
- [`seed-cards.json`](src/Api/Data/seed-cards.json) — 64 seeded records.
- [`DbSeeder.cs:54`](src/Api/Data/DbSeeder.cs#L54), [`DbSeeder.cs:60`](src/Api/Data/DbSeeder.cs#L60), [`DbSeeder.cs:88`](src/Api/Data/DbSeeder.cs#L88)
- [`StatisticsController.cs:35`](src/Api/Controllers/StatisticsController.cs#L35), [`StatisticsController.cs:45`](src/Api/Controllers/StatisticsController.cs#L45), [`StatisticsController.cs:48`](src/Api/Controllers/StatisticsController.cs#L48), [`StatisticsController.cs:49`](src/Api/Controllers/StatisticsController.cs#L49)

The dashboard uses the number of database rows as `TotalCards` for each set. With the supplied seed, that denominator is 64, not 792. Missing cards are not represented as rows, so the current percentage cannot be interpreted as progress toward the canonical 1991 Topps base set.

**Frontend status, October 2:** The dashboard now explicitly describes filing progress over the current inventory. The public archive also identifies its 64 records as the original checklist, and the illustrated covers as artwork. This resolves the misleading presentation; independent certification of checklist names and markers remains outside the review.

**Recommended product decision:** Decide whether the system tracks a personal inventory or a complete checklist. For a full-set completion percentage, load the canonical checklist and store ownership/set membership separately. If the 64 records are intentionally the current personal collection, label the dashboard as progress over the imported collection rather than completion of the full Topps set.

## Package modernization snapshot

The NuGet checks run on October 1, 2026 reported these available updates:

| Package group | Resolved in project | Latest reported by NuGet | Recommendation |
|---|---:|---:|---|
| Microsoft .NET 10 packages in API and client | 10.0.9 | 10.0.12 | Apply the current servicing updates together, then rerun the vulnerability audit. |
| `Microsoft.OpenApi` transitive dependency | 2.0.0 | 3.10.2 | Ensure the resolved package is on a patched version; the advisory's 2.x fix is 2.7.5+. |
| `Stripe.net` | 52.1.0 | 53.0.0 | Review the major-version changes, then validate Checkout and webhook flows in Stripe test mode. |

The package audit found the `Microsoft.OpenApi` issue in the API project. The Client and Shared projects had no vulnerable packages reported by that audit. Package-version results change over time, so rerun the commands before deployment:

```powershell
dotnet list src/Api/MarksBaseballCards.Api.csproj package --vulnerable --include-transitive --no-restore
dotnet list src/Client/MarksBaseballCards.Client.csproj package --vulnerable --include-transitive --no-restore
dotnet list src/Shared/MarksBaseballCards.Shared.csproj package --vulnerable --include-transitive --no-restore

dotnet list src/Api/MarksBaseballCards.Api.csproj package --outdated --include-transitive --no-restore
dotnet list src/Client/MarksBaseballCards.Client.csproj package --outdated --no-restore
```

## Security controls already present

These are useful controls to keep while fixing the findings:

- Passwords use ASP.NET Core `PasswordHasher`, not plaintext storage: [`AuthService.cs`](src/Api/Auth/AuthService.cs).
- JWT validation checks issuer, audience, signing key, and lifetime: [`Program.cs`](src/Api/Program.cs).
- Card administration and system reports are protected by API-side role checks: [`CardsController.cs:15`](src/Api/Controllers/CardsController.cs#L15), [`StatisticsController.cs`](src/Api/Controllers/StatisticsController.cs), [`HistoryController.cs`](src/Api/Controllers/HistoryController.cs).
- The Stripe webhook verifies the `Stripe-Signature` before processing the event: [`CheckoutController.cs:113`](src/Api/Controllers/CheckoutController.cs#L113).
- The server reads the card price from the database before constructing the Stripe line item: [`CheckoutController.cs`](src/Api/Controllers/CheckoutController.cs).
- Card edits use SQL Server row-version concurrency: [`AppDbContext.cs`](src/Api/Data/AppDbContext.cs).

These controls help, but they do not replace deployment TLS, safe secrets, an atomic inventory reservation, or patched dependencies.

## Review limits

This review covers the source and configuration present in the workspace. It does not verify the live SQL Server permissions, the actual production environment variables, Stripe Dashboard payment-method settings, proxy/firewall rules, or whether a production reverse proxy terminates TLS. No automated tests or penetration tests were run.
