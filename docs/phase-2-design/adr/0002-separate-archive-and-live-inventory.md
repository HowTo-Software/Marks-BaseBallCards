# ADR-0002: Separate the original archive from live inventory

**Status:** implemented; retrospective record.
**Implementation evidence:** frontend revision `39a8ecc`.
**Recorded:** October 3, 2026. **Scope:** public frontend data.

## Context

The repository includes a 64-record original seed/checklist and a mutable inventory
API. Visitors need to explore the collection in a standalone frontend preview, while
availability, price and sales depend on the configured backend. A historical record
must not be mistaken for a currently purchasable item.

## Implemented decision

Commit a public archive containing only card number, player name and the two
collector markers. Load it through `CollectionCatalog`. Keep marketplace and staff
data API-backed. Label the archive, record illustrations and current-inventory
filing percentages according to what they represent.

When live services cannot be reached, explain the unavailable state and offer the
archive rather than inventing listings.

## Consequences

Home, story and archive remain usable without SQL or Stripe. Public archive data
does not expose staff filing information or account details. Changes to inventory
do not update the historical snapshot; maintaining the archive requires an explicit
source change and review.

A card-number link can help a visitor search the marketplace, but it is not a stock
guarantee or a database relationship. Buying uses the live row ID. Independently
certifying checklist classifications remains separate work.

## Evidence

- [CollectionCatalog](../../../src/Client/Services/CollectionCatalog.cs)
- [Public archive](../../../src/Client/wwwroot/data/collection.json)
- [Collection](../../../src/Client/Pages/Collection.razor) and [Marketplace](../../../src/Client/Pages/Marketplace.razor)
- [Data model](../data-model.md) and [DATA-01](../../security/security-review.md#data-01--the-seeded-64-cards-are-not-the-full-1991-topps-base-set)
