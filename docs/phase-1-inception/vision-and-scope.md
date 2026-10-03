# Vision and scope of the frontend contribution

**Status:** implemented scope, documented retrospectively. **Updated:** October 3, 2026.

## Purpose

Give the existing card application a recognizable collector-oriented interface:
warm paper and navy palettes, serif headings, original record artwork, responsive
staff workspaces, and motion that helps introduce content and maintain reading
context. The implementation retains the existing Blazor application and API contracts.

The requested delivery included a refreshed frontend informed by OriginKit and
Skiper UI, followed by a dark version and Brazilian Portuguese and Spanish interfaces.
The [delivery commit](https://github.com/HowTo-Software/Marks-BaseBallCards/commit/39a8ecc90bcca4a0eb0d109360f7bae42d4f1e76)
records the changes and approval as reported by the workspace owner.

## People and supported journeys

| Audience | Journey supported in source |
| --- | --- |
| Visitor or collector | Learn about the collection, search the historical archive and inspect illustrated records |
| Buyer | Inspect API-backed listings and USD prices, leave for Stripe Checkout, return to a server-checked payment status |
| Inventory staff (`Admin`) | Maintain records, filing flags, duplicate counts and listing details; record sales or delete with confirmation |
| Reporting staff (`SystemAdmin`) | Inspect current inventory statistics, six filing percentages and recent activity |

The two staff roles are separate. Browsing and purchasing do not require login.
There is no public account registration or password-reset interface in this contribution.

## Delivered frontend boundaries

- Public home, collection and story pages using local assets.
- API-backed marketplace, staff login, inventory workspace, statistics and payment status.
- Shared navigation, dialogs, illustrations, icons and translated validation messages.
- Light/dark styling, saved display preferences and three UI languages.
- Recoverable loading, empty, unavailable, unauthorized and confirmation states.
- Local login-return restrictions, Stripe redirect destination checks and payment polling.
- Historical data wording, component provenance and the existing security review.

The archive contains 64 records from the supplied checklist. It is separate from
live stock and prices. Record artwork does not certify the physical card. The data
and product wording rules are in the [client data model](../phase-2-design/data-model.md).

## Scope requiring Hank's documentation

Backend restructuring, production schema, order/reservation handling, staff account
operations, SQL infrastructure, live Stripe setup, deployment automation and
monitoring are outside this frontend handoff. The code inspected here explains
client dependencies; it does not establish that production is configured the same way.

See the [handoff table](repository-assessment.md) for specific gaps. A product
roadmap, revenue targets, launch dates and formal service objectives are **N/A**
because none were provided or confirmed in this repository.
