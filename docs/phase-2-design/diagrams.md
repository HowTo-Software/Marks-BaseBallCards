# Frontend flows and contract diagrams

**Status:** diagrams of inspected source. **Updated:** October 3, 2026.

The [architecture overview](architecture.md) contains the system context diagram.
These diagrams describe frontend behavior; they are not a production infrastructure
map or a complete database ERD.

## Display preferences before the first render

```mermaid
sequenceDiagram
    participant HTML as index.html
    participant JS as preferences.js
    participant Storage as Browser storage and settings
    participant CSS as Styles
    participant WASM as Blazor startup
    participant Pref as UiPreferences
    participant UI as Razor components

    HTML->>JS: Run before stylesheet links
    JS->>Storage: Read saved language/theme if available
    Storage-->>JS: Saved choices or browser defaults
    JS->>HTML: Set lang, data-theme and boot metadata/copy
    HTML->>CSS: Load local font, app and theme styles
    HTML->>WASM: Load blazor.webassembly.js
    WASM->>Pref: InitializeAsync
    Pref->>JS: htsPreferences.read
    JS-->>Pref: Current choices
    WASM->>UI: First render
    UI->>Pref: SetLanguageAsync or ToggleThemeAsync
    Pref->>JS: Apply and attempt persistence
    Pref-->>UI: Changed event; refresh copy in place
```

Source: [index.html](../../src/Client/wwwroot/index.html),
[preferences.js](../../src/Client/wwwroot/js/preferences.js),
[Program.cs](../../src/Client/Program.cs), [UiPreferences](../../src/Client/Services/UiPreferences.cs).
Storage failure leaves display preferences usable for the current session.

## Staff sign-in and server authority

```mermaid
sequenceDiagram
    participant Staff as Staff member
    participant Page as Login page
    participant Auth as ClientAuthService
    participant API as Auth API
    participant Store as TokenStore
    participant State as Browser auth state
    participant Protected as Protected API

    Staff->>Page: Submit validated username/password
    Page->>Auth: LoginAsync
    Auth->>API: POST api/auth/login
    alt Rejected or unavailable
        API-->>Auth: Error
        Auth-->>Page: Recoverable sign-in message
    else Successful login
        API-->>Auth: Token and account response
        Auth->>Store: Save mbc_token
        Auth->>State: NotifyStateChanged
        State->>Store: Read token and inspect expiry/claims
        State-->>Page: UI role/name state
        Page->>Page: Restrict ReturnUrl or choose role destination
        Page->>Protected: Subsequent bearer request
        Protected->>Protected: Validate signature, issuer, audience, expiry and role
        Protected-->>Page: Authorized response or access error
    end
```

Browser claims are UI state, not verified authorization.
See [auth sources](../../src/Client/Auth) and [API validation](../../src/Api/Program.cs).

## Buyer checkout and payment status

```mermaid
sequenceDiagram
    participant Buyer
    participant UI as Marketplace / return page
    participant API as Checkout API
    participant Stripe as Stripe Checkout
    Buyer->>UI: Buy a live listing
    UI->>API: POST api/checkout/cardId
    API->>API: Read current listing state and server price
    API->>Stripe: Create Checkout Session
    Stripe-->>API: Hosted URL and session ID
    API-->>UI: CheckoutSessionResponse
    UI->>UI: Require HTTPS checkout.stripe.com destination
    UI->>Stripe: Redirect browser
    Buyer->>Stripe: Complete or leave checkout
    Stripe-->>API: Webhook handled by server
    Stripe-->>UI: Return with session_id
    loop Up to five attempts, until paid
        UI->>API: GET api/checkout/status/sessionId
        API-->>UI: CheckoutStatusDto
        UI->>UI: One-second delay between unpaid attempts
    end
    alt PaymentStatus is paid
        UI-->>Buyer: Payment confirmed
    else Missing reference or confirmation unavailable
        UI-->>Buyer: Honest pending/retry guidance
    end
```

Payment and webhook ordering can vary. A return visit is not a payment guarantee.
Backend reservation, delayed-payment events and reconciliation limitations are in
[SEC-02 and SEC-09](../security/security-review.md). Leaving the return page cancels
its polling loop; it may not abort a status request already in flight.

## Client contract map

```mermaid
classDiagram
    class CollectionCatalog {
        GetAsync()
    }
    class CollectionRecord {
        string CardNumber
        string PlayerName
        bool IsRookie
        bool IsRoyals
    }
    class ApiClient {
        GetMarketplaceAsync()
        GetCardsAsync()
        GetStatisticsAsync()
        GetHistoryAsync()
        StartCheckoutAsync()
        GetCheckoutStatusAsync()
    }
    class MarketplaceCardDto {
        int Id
        string CardNumber
        string PlayerName
        bool IsRookie
        bool IsRoyals
        decimal Price
        string Condition
        string ListingNotes
        bool IsSold
    }
    class CardDto
    class StatisticsDto
    class CardHistoryDto
    class CheckoutStatusDto
    CollectionCatalog --> CollectionRecord : loads historical records
    ApiClient ..> MarketplaceCardDto : reads listings
    ApiClient ..> CardDto : reads and mutates inventory
    ApiClient ..> StatisticsDto : reads aggregate
    ApiClient ..> CardHistoryDto : reads activity
    ApiClient ..> CheckoutStatusDto : checks payment
```

Nullable price, condition and notes are simplified in the diagram; exact C# types
are in [MarketplaceCardDto](../../src/Shared/Models/MarketplaceCardDto.cs).
No database relationship between archive records and live inventory is implied.
