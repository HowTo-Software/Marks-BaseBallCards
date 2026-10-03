# Frontend technical design and source map

**Status:** implemented behavior. **Updated:** October 3, 2026.

## Page responsibilities

| Route | Source | Responsibility |
| --- | --- | --- |
| `/` | [Home](../../src/Client/Pages/Home.razor) | Local introduction, archive selections, story, collecting steps and FAQ |
| `/collection` | [Collection](../../src/Client/Pages/Collection.razor) | Historical archive, name/number search, marker filters, number/name sorting, grid/list and dialogs |
| `/about` | [About](../../src/Client/Pages/About.razor) | Provenance, collection context and explicit artwork/data notes |
| `/marketplace` | [Marketplace](../../src/Client/Pages/Marketplace.razor) | Live listings, local filtering/sorting, include-sold reload, details and checkout |
| `/login` | [Login](../../src/Client/Pages/Login.razor) | Validated staff form, password visibility, busy/error states and restricted return path |
| `/admin` | [AdminCards](../../src/Client/Pages/Admin/AdminCards.razor) | `Admin` inventory form/table and confirmed mutations |
| `/system` | [SystemDashboard](../../src/Client/Pages/Admin/SystemDashboard.razor) | `SystemAdmin` tiles, six filing progress bars and latest history |
| `/buy/success` | [BuySuccess](../../src/Client/Pages/BuySuccess.razor) | Status checks using the `session_id` query parameter |
| `/access-denied`, `/error`, `/not-found` | [Error pages](../../src/Client/Pages/Errors) | Access restrictions and recovery; unmatched routes use the not-found UI |

Archive and marketplace links use card **numbers** for `?card=1`. Checkout mutations
use the API record's integer **ID**. Do not substitute one identifier for the other.

## Shared components and layout

| Source | Responsibility |
| --- | --- |
| [MainLayout](../../src/Client/Layout/MainLayout.razor) | Skip link, frame/footer, error boundary, preference events and JS initialization/disposal |
| [NavMenu](../../src/Client/Layout/NavMenu.razor) | Public/role-aware navigation, mobile menu and closing the menu after navigation |
| [DisplayPreferences](../../src/Client/Components/DisplayPreferences.razor) | Theme and language controls |
| [AnimatedText](../../src/Client/Components/AnimatedText.razor) | Word-preserving character spans and an accessible complete heading |
| [CardArt](../../src/Client/Components/CardArt.razor) | Original illustrated record cover, not a physical card image |
| [Icon](../../src/Client/Components/Icon.razor) | Local SVG icon vocabulary |
| [Modal](../../src/Client/Components/Modal.razor) | Native dialog, close/Escape handling and JS lifecycle |
| [LocalizedComponentBase](../../src/Client/Components/LocalizedComponentBase.cs) | Subscription to preference changes and disposal |
| [LocalizedValidationMessage](../../src/Client/Components/LocalizedValidationMessage.razor) | Translation of existing EditContext validation messages |

Most Razor components inherit the localized base through
[_Imports.razor](../../src/Client/_Imports.razor). The layout has its own subscription;
the icon does not need localized lifecycle behavior. Components that override
disposal must preserve subscription cleanup, as the modal and payment page do.

## Styling and motion

- [fonts.css](../../src/Client/wwwroot/css/fonts.css): local DM Sans and Instrument Serif.
- [app.css](../../src/Client/wwwroot/css/app.css): typography, editorial layout,
  artwork, states, controls, responsive pages and reduced-motion rules.
- [themes.css](../../src/Client/wwwroot/css/themes.css): display controls and dark
  variants for panels, forms, dialogs, tables, status colors and footer.
- [ui.js](../../src/Client/wwwroot/js/ui.js): native Web Animations for the heading,
  IntersectionObserver reveals, a single animation-frame scroll update, fine-pointer
  tilt and native dialog/focus helpers.

[The source reference guide](ui-references.md) preserves component acquisition,
hashes, authors, attribution and adaptation details. Content remains readable before
animation initialization. Motion respects `prefers-reduced-motion`, including a
preference change after the page loads. Navigation/disposal must release observers
and event handlers; avoid adding a new global listener per render.

## Loading, failure and mutation state

Pages distinguish loading, no results, no records, unavailable services and access
errors. Archive failures reset the cached load in
[CollectionCatalog](../../src/Client/Services/CollectionCatalog.cs), allowing a retry.
Live pages use the API; they do not populate invented listings after a network failure.

[ApiClient](../../src/Client/Services/ApiClient.cs) returns `ApiResult<T>` for mutations,
disposes responses and reports HTTP/network/JSON errors. Read methods can throw;
their page callers display a recoverable state. Known error strings are localized,
while unknown server messages and staff-entered text remain original.

Inventory and checkout actions track a busy state to prevent repeated UI submissions.
This UI guard does not reserve inventory against another buyer; the backend concern
is SEC-02 in the security review.

## Payment-status behavior

[BuySuccess](../../src/Client/Pages/BuySuccess.razor) makes up to five status requests,
with one-second delays between unpaid attempts. HTTP request time is additional.
A missing reference is explained; an unconfirmed response is not described as a
failed payment. Only `PaymentStatus == "paid"` enables the confirmed view.

Disposal cancels the polling loop/delays. The status method does not receive that
cancellation token, so an already-running HTTP request may still finish. The
client's 20-second HTTP timeout remains applicable. Shipping, refunds and order
reconciliation are backend/operations topics pending Hank.

## Preferences and formatting

[UiPreferences](../../src/Client/Services/UiPreferences.cs) shares language/theme state
and refresh notifications. [preferences.js](../../src/Client/wwwroot/js/preferences.js)
runs before styles and stores `hts.language`/`hts.theme`. The catalog is an embedded
resource and .NET globalization data is enabled in the client project.

The number formatter returns localized numeric text; UI labels supply USD.
Dates use the browser's local timezone. Preserve these semantics when editing a
price or history component. See [theme/language maintenance](../phase-3-development/themes-and-languages.md).
