# API contracts used by the frontend

**Status:** observed client/controller contracts; backend restructuring pending Hank.
**Updated:** October 3, 2026.

This is the frontend integration reference, not a replacement for a complete backend
specification. Sources are [ApiClient](../../src/Client/Services/ApiClient.cs),
[ClientAuthService](../../src/Client/Auth/ClientAuthService.cs), shared DTOs and
[controllers](../../src/Api/Controllers).

## Transport and credentials

The registered HTTP client uses the browser application's base address and a
20-second timeout. Paths below are relative to that origin.
[BearerTokenHandler](../../src/Client/Auth/BearerTokenHandler.cs) attaches the stored
`mbc_token` when present. A storage-read failure allows anonymous requests to
continue; it does not authorize protected endpoints.

No API hostname override or proxy is configured. Use the API-hosted client for
integrated work. [Onboarding](../phase-3-development/onboarding.md) explains both modes.

## Consumed routes

| Method and path | API access | Client input | Response / consumer |
| --- | --- | --- | --- |
| `GET /api/marketplace` | Anonymous | `includeSold`; optional `search` supported by wrapper | `List<MarketplaceCardDto>`; marketplace |
| `POST /api/auth/login` | Anonymous, login rate limit | `LoginRequest` | `LoginResponse`; staff sign-in |
| `GET /api/cards` | `Admin` | Optional `search` | `List<CardDto>`; inventory |
| `POST /api/cards` | `Admin` | `CardUpsertDto` | `CardDto`; new inventory record |
| `PUT /api/cards/{id}` | `Admin` | Integer ID and `CardUpsertDto` | `CardDto`; edit inventory |
| `DELETE /api/cards/{id}` | `Admin` | Integer ID | Success without a JSON value; wrapper returns `ApiResult<bool>` |
| `POST /api/cards/{id}/sell` | `Admin` | `SellRequest` with recorded `SoldPrice` | `CardDto`; manual sale |
| `GET /api/statistics` | `SystemAdmin` | None | `StatisticsDto`; dashboard |
| `GET /api/history?take=200` | `SystemAdmin` | Requested entry limit | `List<CardHistoryDto>`; dashboard activity |
| `POST /api/checkout/{cardId}` | Anonymous | Integer live record ID; no client price | `CheckoutSessionResponse`; hosted redirect |
| `GET /api/checkout/status/{sessionId}` | Anonymous | URI-escaped session reference | `CheckoutStatusDto`; return-page polling |

The marketplace and inventory pages currently load records then perform their
visible search/filter/sort operations in the client. Include-sold changes reload
marketplace data. Do not assume server-side pagination is present in these UI calls.

## DTO locations

- [Inventory, marketplace, statistics, history and sale models](../../src/Shared/Models)
- [Login models](../../src/Shared/Auth) and [role constants](../../src/Shared/Common/Roles.cs)
- [Checkout responses](../../src/Shared/Models/CheckoutDtos.cs)
- [Editable field limits](data-model.md)

Paths containing braces in the table describe route parameters, not template
placeholders. Values come from actual records or the Stripe return reference.

## Response and failure behavior

Mutations return `ApiResult<T>`. The wrapper reads an `error` field where available
and otherwise supplies a recoverable status-based message. It handles 429, 401,
403, 409 and 503 with specific fallbacks; remaining failures use a generic message.
A successful JSON mutation must return a value to be shown as completed.
Deletion accepts a successful empty response.

Reads use JSON deserialization and can throw. Pages distinguish those failures
from an empty successful response. Checkout status returns null for a non-success
HTTP status. Login has its own response handling and busy/error UI. Unknown server
text may remain in its original language; known messages have translation entries.

Server ProblemDetails and validation formats may vary by endpoint. The current
client is not a general ProblemDetails parser. If Hank changes response envelopes,
update the wrapper, localized copy and affected pages together.

## Integration boundaries

The server also defines `GET /api/auth/me`, single-record card/marketplace reads and
`POST /api/checkout/webhook`. They are not called directly by this UI. The webhook
is a Stripe-to-server integration, not a browser endpoint to invoke for fulfillment.

The server decides listing state and price. The UI only accepts checkout redirects
using HTTPS and the `checkout.stripe.com` hostname. That check does not establish
inventory reservation or refund handling. The relevant backend concerns remain
SEC-02, SEC-06 and SEC-09 in the [security review](../security/security-review.md).

**Pending Hank:** comprehensive backend API specification, any restructured route/DTO
changes, order lifecycle, production payment methods and authenticated integration evidence.
