# Themes and languages

The HTS frontend supports a light paper palette and a dark navy palette, alongside English, Brazilian Portuguese and Spanish. These preferences apply to public pages and staff workspaces.

## Using the controls

The controls sit in the header, including on mobile:

- Select the moon or sun button to switch between dark and light themes.
- Select **EN**, **PT-BR** or **ES** to change the interface language.

The first visit follows the browser's language and system appearance. Unsupported browser languages fall back to English. Once you choose a theme or language, that choice takes priority and is saved for subsequent visits.

Changing language updates the current page in place. It keeps the search, category, sort order, entered form values and open record dialog. Navigation paths and API field names remain stable across languages.

## What is translated

Navigation, page headings and titles, collection filters, record details, explanatory copy, FAQs, staff forms, confirmation dialogs, dashboard labels, validation messages, known API errors and payment status messages all use the selected language. The loading screen and application reload notice are translated too.

Player names, card numbers, the Mark's Baseball Cards brand, seller-entered condition and listing notes, account names and recorded audit details retain their original content. This preserves the source records and avoids silently rewriting a seller's description. Action labels in the activity table are translated; the stored action codes are unchanged. Unknown server messages fall back to their original text.

## Prices and dates

**Prices remain in USD. Language selection does not convert currencies or change checkout amounts.** The marketplace explicitly shows USD, and the inventory table and sales summary identify the same currency.

Displayed numbers use the chosen locale: `1,234.56` in English and `1.234,56` in Brazilian Portuguese and Spanish. Dates use the selected locale and the browser's local timezone. HTML numeric fields still submit numbers through the existing API contract.

## Theme design

The dark theme uses a navy canvas, cream text, lighter coral accents, separate panel tones, muted borders and adjusted status colors. Inputs, empty states, loading placeholders, dialogs, tables, dashboard tiles, mobile navigation and the footer have explicit dark styles.

Printed collection artwork retains its original paper, red, green, gold and navy colors. It remains an illustration of a record, rather than a photograph of a physical card. Theme changes do not alter that distinction.

Both themes retain keyboard focus indicators and reduced-motion support. Preferences are applied before styles and Blazor load, avoiding a flash of the other theme on a saved dark preference.

## Implementation map

| File | Responsibility |
| --- | --- |
| [DisplayPreferences.razor](../src/Client/Components/DisplayPreferences.razor) | Header controls, localized labels and current selection |
| [UiPreferences.cs](../src/Client/Services/UiPreferences.cs) | Shared preferences, translations, formatting and update notifications |
| [translations.json](../src/Client/Localization/translations.json) | Brazilian Portuguese and Spanish copy, keyed by the original English text |
| [LocalizedComponentBase.cs](../src/Client/Components/LocalizedComponentBase.cs) | Refreshes translated copy without remounting the page |
| [LocalizedValidationMessage.razor](../src/Client/Components/LocalizedValidationMessage.razor) | Translates the existing form validation messages |
| [preferences.js](../src/Client/wwwroot/js/preferences.js) | Initial appearance, language, browser storage and document metadata |
| [themes.css](../src/Client/wwwroot/css/themes.css) | Theme controls, dark palette and responsive adjustments |
| [Program.cs](../src/Client/Program.cs) | Registers and initializes preferences before the first application render |

The translation catalog is embedded in the client assembly, so switching language does not call a translation service or fetch another file. The client includes .NET globalization data for all three locales.

Browser storage keys are `hts.language` and `hts.theme`. Storage access is guarded: if the browser disables local storage, the selected preferences still work for the current page session. These preferences contain no account or payment information.

## Editing copy

Use `L["Original English text"]` for a label and `L.Format("Card #{0}", number)` for a sentence with values. Add the same English key to both language tables. Preserve placeholders such as `{0}` and `{1}` in each translation.

Keep filter values, route names and API field names independent of displayed translations. Translate a known server message at the presentation layer rather than rewriting the backend contract. Keep user-supplied data separate from the translation catalog.

## Local preview and review

Run the client using the command in the [main README](../README.md) and open [localhost:5248](http://localhost:5248). Reload an existing browser tab to load the updated client, then use the header controls.

The client build completed with zero errors and warnings. Public pages were visually inspected in Chromium; dark Portuguese and Spanish/light captures are linked in the [preview gallery](screenshots/README.md). No automated test suite was added or run for this change. Staff mutations and real payments still require the configured API, SQL Server and Stripe services and were not exercised.
