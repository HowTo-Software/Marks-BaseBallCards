# Implemented frontend user stories

**Status:** retrospective map to source, not a test-result report.
**Updated:** October 3, 2026.

These stories describe behavior visible in the frontend. Live API stories require
a configured backend and the relevant staff role. Review procedures are documented
separately in the [manual cases](../phase-4-testing/test-cases.md).

| ID | As a user, I can... | Implementation evidence | Boundary |
| --- | --- | --- | --- |
| FE-01 | Explore the collection's background and original records | [Home](../../src/Client/Pages/Home.razor), [About](../../src/Client/Pages/About.razor) | Local historical content |
| FE-02 | Find an archive record by name/number, filter it and change the view | [Collection](../../src/Client/Pages/Collection.razor) | Search and view state stay in the browser |
| FE-03 | Open a record dialog or a direct record link | [Collection](../../src/Client/Pages/Collection.razor), [Modal](../../src/Client/Components/Modal.razor) | `/collection?card=1` is an archive card number, not a database ID |
| FE-04 | Check current listings, sort prices and inspect condition/notes | [Marketplace](../../src/Client/Pages/Marketplace.razor) | Real API response required; unavailable services do not create sample listings |
| FE-05 | Start checkout and see an honest payment-status result | [Marketplace](../../src/Client/Pages/Marketplace.razor), [BuySuccess](../../src/Client/Pages/BuySuccess.razor) | Stripe plus backend; UI confirmation requires `PaymentStatus == "paid"` |
| FE-06 | Sign in with an owner-provided staff account | [Login](../../src/Client/Pages/Login.razor) | Existing login API and browser token storage |
| FE-07 | Maintain inventory and confirm sale/deletion actions | [AdminCards](../../src/Client/Pages/Admin/AdminCards.razor) | `Admin` API authority; destructive actions require appropriate environment |
| FE-08 | Review totals, filing progress and recent activity | [SystemDashboard](../../src/Client/Pages/Admin/SystemDashboard.razor) | `SystemAdmin`; latest history request capped at 200 in this UI |
| FE-09 | Switch theme and interface language without losing current page state | [UiPreferences](../../src/Client/Services/UiPreferences.cs), [DisplayPreferences](../../src/Client/Components/DisplayPreferences.razor) | USD amounts and source-entered descriptions remain unchanged |
| FE-10 | Use keyboard navigation and reduced motion | [MainLayout](../../src/Client/Layout/MainLayout.razor), [Modal](../../src/Client/Components/Modal.razor), [ui.js](../../src/Client/wwwroot/js/ui.js) | Implemented aids; no accessibility certification is claimed |

## Acceptance guidance

Changes to these journeys should preserve local archive availability, clear live
service errors, source-data wording, current DTOs and both additional language
tables. A successful client build alone cannot demonstrate a successful staff
mutation or paid transaction. Record which stories were actually exercised in each
review, with the relevant revision and environment.
