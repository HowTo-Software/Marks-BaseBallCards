# ADR-0003: Embed translations and persist display preferences

**Status:** implemented; retrospective record.
**Implementation evidence:** frontend revision `39a8ecc`.
**Recorded:** October 3, 2026. **Scope:** themes, languages and formatting.

## Context

The approved frontend was followed by a request for a dark appearance and Brazilian
Portuguese and Spanish interfaces. Changing these preferences should not reset a
search, form or dialog, and the displayed language must not change a purchase amount.

## Implemented decision

Use the original English text as translation keys/fallback. Embed the PT-BR and
Spanish dictionaries in the client assembly. `UiPreferences` supplies formatting
and change notifications; localized components refresh in place.

Run `preferences.js` before CSS and Blazor to choose saved/browser defaults and set
document appearance/language. Persist choices under `hts.language` and `hts.theme`,
with guarded storage access. Use locale-specific numbers/dates while retaining USD
labels and amounts.

## Consequences

Switching language uses no translation service or extra catalog fetch. Both
additional language tables must be maintained when English UI text changes. Boot
copy in JavaScript is a separate surface that must stay consistent with the catalog.

The client includes .NET globalization data. Browser settings contain display
preferences, not account/payment information. A denied storage operation leaves
display preferences available in the page session. Staff token storage has its own
limitations and should not be conflated with preference persistence.

Seller-entered condition/notes, names and recorded audit details are preserved
rather than translated automatically. No exchange-rate or automatic content
translation service is part of this implementation.

## Evidence

- [UiPreferences](../../../src/Client/Services/UiPreferences.cs)
- [translations.json](../../../src/Client/Localization/translations.json)
- [preferences.js](../../../src/Client/wwwroot/js/preferences.js)
- [Client project resource/globalization settings](../../../src/Client/MarksBaseballCards.Client.csproj)
- [Themes and languages](../../phase-3-development/themes-and-languages.md)
