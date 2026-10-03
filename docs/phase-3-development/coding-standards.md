# Frontend coding and content conventions

**Status:** guidance based on the delivered implementation.
**Updated:** October 3, 2026.

These conventions help keep changes consistent with the current client. They do not
claim a repository-wide formatter or lint policy that is not committed.

## Components, state and API calls

Keep route behavior in its page and reusable UI in `Components` or `Layout`.
Use the existing `ApiClient` and shared DTOs for live data. Keep the historical
archive behind `CollectionCatalog`. New API response shapes should be agreed with
Hank and documented in [API contracts](../phase-2-design/api-specification.md).

Track loading, errors and busy mutations explicitly. Require a successful server
response before showing a successful action. A busy button prevents a repeated
submission from this browser; it does not solve concurrent purchases from different
buyers. Keep confirmation before deletion/manual sale.

Dispose response messages, JS modules, subscriptions and observers according to their
existing lifecycles. Components overriding the localized base's disposal must call
the base cleanup. Preserve payment-page polling cancellation.

## Translations and formatting

Use `L["Card details"]` or `L.Format` for UI copy and add matching entries to
PT-BR and Spanish. Keep format arguments and markup out of translated data where
possible. The English key is the fallback, so changing it requires reviewing both tables.

Use `L.Usd` with an explicit USD label; it returns formatted numbers, not converted
currency. Use the existing locale/timezone helpers for dates. Keep route names,
filter values, API fields and action codes stable when translating their visible labels.

Do not automatically translate names, card numbers, seller descriptions or audit
details. Review boot-screen/error copy in `preferences.js` when those surfaces change.
The complete maintenance map is in [themes and languages](themes-and-languages.md).

## Styling, animation and accessibility

Use the shared app/theme styles and existing components before adding parallel
design systems. Include both themes and mobile layouts in a visual change. Keep
focus styles, labels, keyboard activation and dialog close behavior.

Respect reduced motion and release JS listeners on navigation/disposal. Use native
scrolling and the current observer/frame scheduling pattern for the existing effects.
Keep text readable before animation initialization. Retain component attribution
and [font notices](../security/compliance.md).

## Data truth and trust boundaries

Never label the archive as current stock, artwork as a scan, collector markers as a
certification, or filing percentages as full-set completion. Do not confirm payment
from the return URL alone.

Treat browser claims as UI hints. Keep server role enforcement and validation as the
authority. Avoid rendering untrusted HTML or adding script access to server secrets.
Client assets and storage are inspectable by the visitor.

## Review evidence

Record the commands/browser checks actually performed and any unavailable
integration checks. Use the [test plan](../phase-4-testing/test-plan.md) to separate
source evidence, earlier visual evidence and new execution results. Update docs and
an ADR if an established frontend boundary changes.
