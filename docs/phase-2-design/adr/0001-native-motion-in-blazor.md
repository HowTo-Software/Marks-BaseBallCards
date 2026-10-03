# ADR-0001: Adapt motion within the existing Blazor client

**Status:** implemented; retrospective record.
**Implementation evidence:** frontend revision `39a8ecc`, October 2 reference guide.
**Recorded:** October 3, 2026. **Scope:** frontend.

## Context

The requested frontend refresh used OriginKit and Skiper UI as design/component
references. Their collected examples use React and animation libraries, while this
application is a Blazor WebAssembly project with no npm pipeline.

## Implemented decision

Keep the existing client platform. Translate OriginKit's Stagger Text Rise and
Skiper's card-stack progress/scale behavior into Razor components, CSS and native
browser APIs. Use a small JS module for Web Animations, observers, scroll updates,
pointer behavior and native dialogs. Preserve source provenance and attribution.

## Consequences

The client remains buildable with the existing .NET tooling. Motion has
reduced-motion handling, native scrolling and content that is readable before
initialization. Navigation and disposal must clean up listeners and observers.

The adaptation is application-specific: upstream React updates do not drop directly
into this repository. Maintainers must understand the translated behavior and
review it when changing layouts. No React or Framer runtime is introduced by this
contribution.

## Evidence

- [Motion reference guide, source hashes and license notes](../ui-references.md)
- [AnimatedText](../../../src/Client/Components/AnimatedText.razor)
- [ui.js](../../../src/Client/wwwroot/js/ui.js)
- [Client project dependencies](../../../src/Client/MarksBaseballCards.Client.csproj)

A platform migration or adopting a new animation runtime would need a separate
decision. There is no evidence of such a migration being agreed in this delivery.
