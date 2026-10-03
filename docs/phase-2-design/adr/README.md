# Frontend Architecture Decision Records

**Status:** retrospective documentation of implemented choices.
**Recorded:** October 3, 2026.

These ADRs explain decisions supported by the frontend code, its existing reference
guides and delivery history. They do not claim that a formal design meeting or an
alternatives review happened. Where alternatives are mentioned, they describe the
current design boundary rather than an invented historical discussion.

| ADR | Implemented choice |
| --- | --- |
| [0001](0001-native-motion-in-blazor.md) | Adapt motion with Razor, CSS and native JavaScript in the existing Blazor client |
| [0002](0002-separate-archive-and-live-inventory.md) | Keep the historical public archive separate from API-backed inventory |
| [0003](0003-localized-ui-and-display-preferences.md) | Embed UI translations, persist display preferences and keep sale currency unchanged |

Backend architecture ADRs are **pending Hank**. The inspected source establishes
dependencies but does not establish the rationale behind his restructuring.
For future changes, add a record when the decision is concrete and supported by
evidence; retain a superseded record with a link to its replacement.
