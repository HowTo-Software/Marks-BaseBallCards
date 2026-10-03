# Frontend threat model and trust boundaries

**Status:** source-based frontend model, not a penetration-test result.
**Updated:** October 3, 2026.

## Assets and boundaries

The browser holds public archive data, user-entered search/form state, display
preferences and a staff bearer token after login. It receives live inventory,
statistics and history according to server access. The server owns authentication,
authorization, price/stock decisions, signing secrets, SQL access and Stripe calls.

All browser code, assets, storage and UI state are inspectable or modifiable by the
visitor. The [architecture diagram](../phase-2-design/architecture.md) shows these
dependencies. Public archive content crosses no database authentication boundary;
it is intentionally downloadable.

## Observed controls and residual risks

| Scenario | Source-confirmed behavior | Residual risk / next step |
| --- | --- | --- |
| Visitor changes browser role/claims | Browser reads expiry/claims for UI; [API validates JWT](../../src/Api/Program.cs) and controllers enforce roles | Keep server enforcement; browser decoding is not signature verification |
| Script reads a staff token | [TokenStore](../../src/Client/Auth/TokenStore.cs#L15) stores `mbc_token` in localStorage | SEC-08: XSS exposure and copied-token lifetime; logout does not revoke a stolen copy |
| Login return address points outside the app | [Login](../../src/Client/Pages/Login.razor#L48) restricts base URI, rejects backslashes and avoids login loops | SEC-10 source remediation; authenticated integration cases remain pending |
| Checkout response redirects to an unexpected site | [Marketplace](../../src/Client/Pages/Marketplace.razor) requires HTTPS `checkout.stripe.com` | Server's return URL construction is a separate SEC-06 concern |
| Return URL suggests a paid order | [BuySuccess](../../src/Client/Pages/BuySuccess.razor#L39) checks server payment status | Missing/unconfirmed reference must remain honest; backend fulfillment/reconciliation still required |
| Two buyers try one listing | Busy state prevents repeat UI submission in one client | SEC-02 remains: no cross-buyer reservation is established by the frontend |
| Historical data is mistaken for stock | Separate archive and live API views, explicit artwork/marker wording | Preserve wording; independent physical-card/checklist certification not performed |
| Browser storage is denied or token malformed | Auth provider returns anonymous for supported malformed/storage failures; preferences guard storage | Public browsing can continue; login may fail if token cannot be stored |

## Data handling

No database password, signing key or Stripe secret belongs in `wwwroot`, translated
copy, screenshots or client-side configuration. Preferences contain only display
choices. Seller notes/conditions and history details are kept in their original form;
they should remain text-rendered content rather than trusted HTML.

Do not log or publish a full bearer token. A checkout reference is not an account
credential, but it can expose transaction context and should be redacted in public
review artifacts. No new telemetry or security-header policy is introduced by this
documentation task.

## Related backend findings

The preserved [security guide](security-review.md) covers known credentials/defaults,
HTTP transport, SQL privileges/encryption, OpenAPI dependency alert, login disclosure,
concurrent checkout and delayed-event handling. Its remediation instructions remain
recommendations until implementation and verification are recorded.

**Pending Hank:** restructured backend threat model, runtime secrets/TLS/host policy,
reservation/order design, staff recovery and security remediation evidence. This
frontend record is not a compliance attestation or a complete backend assessment.
