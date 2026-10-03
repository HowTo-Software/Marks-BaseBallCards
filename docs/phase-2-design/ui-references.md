# Frontend Experience and Motion References

**Project:** Mark’s Baseball Cards · HTS<br />
**Updated:** October 2, 2026<br />
**Stack:** Blazor WebAssembly / .NET 10

## The direction

The site now feels like a collector’s editorial archive: warm paper, deep blue, a restrained baseball red, and large serif headings. The visual identity stays tied to the collection rather than to a generic application dashboard. The client’s Mark’s Baseball Cards name is preserved, with an HTS credit in the footer.

There is one shared visual system for the public site and the staff areas. Search, filtering, loading, unavailable services, dialogs and confirmations have their own consistent states.

## What was collected and adapted

Both sources were inspected before implementation. The original examples use React and animation libraries; their behaviors were translated into Razor components, CSS and a small native JavaScript module to fit the existing Blazor application.

| Source | Collected component | How this site uses it | Implementation |
|---|---|---|---|
| [OriginKit](https://www.originkit.dev/components/stagger-text-rise) | **Stagger Text Rise** | A short, staggered entrance for the home heading. Text remains a real, accessible heading. | [AnimatedText.razor](../../src/Client/Components/AnimatedText.razor), `rise()` in [ui.js](../../src/Client/wwwroot/js/ui.js) |
| [Skiper UI](https://skiper-ui.com/v1/skiper16) | **Card stack scroll / StickyCard_001** | The three collecting steps stay in view and gently scale as the next step arrives. | Home’s collecting section, `updateScroll()` in [ui.js](../../src/Client/wwwroot/js/ui.js), sticky positioning in [app.css](../../src/Client/wwwroot/css/app.css) |

### OriginKit acquisition

The Stagger Text Rise source was retrieved through the site’s **Get this Component** action using the existing Firefox session explicitly authorized by the workspace owner. The authenticated source endpoint returned the Framer and Next.js implementations on October 2, 2026.

The adaptation preserves the source’s character splitting, vertical offset, opacity interpolation and stagger sequence. The initial offset is 18px, the duration is 580ms, and the total stagger delay is capped at 220ms for this site. Native Web Animations provide the motion. Words stay together when the heading wraps.

SHA-256 of the retrieved `source` field as UTF-8:

`3318c8ff7a2a05722a1b7c42713e457110946e1d3f7763038e15384b78c03d3d`

The [OriginKit usage terms](https://www.originkit.dev/docs/licensing) allow adapting components for a client’s application. This project contains the application-specific adaptation. The footer gives a factual reference credit without implying a partnership.

### Skiper UI acquisition

The free registry item was downloaded from [the Skiper16 registry endpoint](https://skiper-ui.com/r/skiper16.json). Its source declares **Gurvinder Singh / @gurvinder-singh02** as the author and permits commercial modification with attribution for free usage.

The adaptation uses the original progress-to-scale relationship and indexed ranges, with a smaller scale change suitable for reading. The page keeps native scrolling. The illustrations from Skiper’s sample are separate assets and are not used here.

SHA-256 of the retrieved registry file’s `content` field as UTF-8:

`b4c23c065b5943b5165ed246d375be626558b615aae71cd243dc856338b55ddb`

The visible **Skiper UI** footer link supplies attribution. See [Skiper’s licensing instructions](https://skiper-ui.com/docs/quick-start).

## The pages

| Route | Purpose |
|---|---|
| `/` | Editorial introduction, actual archive selections, collection story, buying steps and FAQs. |
| `/collection` | Original 64-record archive with immediate search, category filters, sorting, grid/list layouts and record dialogs. |
| `/marketplace` | Live listings from the existing API; search, filters, sorting, sold visibility, listing details and Stripe checkout. |
| `/about` | Collection background and clear notes on what the data and illustrations represent. |
| `/login` | Staff sign-in with input validation, password visibility, progress and recoverable errors. |
| `/admin` | Existing Admin role: create, edit, list/unlist, record sales and delete records with confirmation. |
| `/system` | Existing SystemAdmin role: inventory statistics, filing progress and the latest 200 activity entries. |
| `/buy/success` | Server-confirmed payment status, a retry action and cancellation of polling when the page is left. |
| Error routes | Styled not-found, access-restricted and recovery states. |

All staff actions continue to use the existing protected API endpoints.

## Data that visitors can trust

The public archive is a deliberately separate snapshot of the original checklist. [collection.json](../../src/Client/wwwroot/data/collection.json) copies only card number, player name, rookie marker and Royals marker from [seed-cards.json](../../src/Api/Data/seed-cards.json). It publishes no staff credentials, sale prices, checkout sessions or private filing information.

The archive is available when the client is run on its own. It does not indicate that a card is currently owned, available for sale or in a particular condition. Inventory changes in the admin area do not change this historical snapshot.

The marketplace uses real API data. If the API is unavailable, the page explains that availability could not be checked and offers a retry and a link to the archive. An unavailable service never produces invented listings or successful payments.

Individual card scans are not present in the original data model. The covers are original SVG/CSS record illustrations and are labeled accordingly. The artwork is not a Topps card reproduction or a grading certificate. Rookie and Royals markers retain the original collector annotations, without claiming independent certification.

The UI uses USD, matching the repository’s current `Stripe:Currency = usd`. Changing the settlement currency requires updating the display formatting and labels together.

## Motion and accessibility

- The staggered title helps introduce the collection; it runs once as the heading enters view.
- A small pointer tilt adds depth to the home card stack on devices with a precise pointer.
- Record hover and focus states show that a card opens.
- Sticky collecting steps keep the current reading context visible.
- Scroll reveals use IntersectionObserver and scroll scaling uses a single requestAnimationFrame.
- `prefers-reduced-motion` disables those animations and is respected when the preference changes.
- Content is readable before JavaScript initializes.
- Native dialogs provide focus containment; Escape closes them and focus returns to the opener.
- Search fields, navigation, view controls and status messages have accessible labels.
- A skip link, visible focus styles and mobile navigation support keyboard use.

## Frontend reliability improvements

The frontend also includes light/dark palettes and English, Brazilian Portuguese and Spanish UI copy. Saved preferences, coverage, formatting and implementation are documented in [Themes and Languages](../phase-3-development/themes-and-languages.md).

The refresh also makes network and API failures recoverable in the UI. Mutation responses are disposed, checkout cannot be started repeatedly while a request is in flight, and a successful server response is required before showing a completed action.

Post-login return addresses are restricted to the current application. The checkout redirect must be an HTTPS URL on `checkout.stripe.com`. Malformed, missing-expiry and out-of-range browser JWT payloads return the UI to anonymous state; server-side token and role validation remain the authority.

The dashboard describes set percentages as filing progress over the current inventory records. It does not present the 64-record seed as a complete Topps set.

## Local preview

With a .NET 10 SDK:

```powershell
dotnet run --project src/Client --no-launch-profile --urls http://localhost:5248
```

Open [the local preview](http://localhost:5248). Home, the archive, collection details and the story are served entirely by the client.

To use live inventory, staff authentication and checkout, run the API with the SQL Server and Stripe settings described in the [backend integration reference](../phase-5-deployment/backend-reference.md). The API hosts the frontend and endpoints on the same origin. The standalone client does not proxy API requests.

## Fonts and artwork

DM Sans and Instrument Serif are self-hosted. Their SIL Open Font License files are included under [wwwroot/fonts](../../src/Client/wwwroot/fonts). Font assets and license files came from Google Fonts. Icons, cover illustrations, the diamond drawing, favicon and the paper grain are local SVG/CSS assets.

## Review and remaining limits

The build and browser observations below were recorded during the October 2 frontend implementation. Reorganizing the documentation on October 3 did not repeat those checks.

The client build completed with zero warnings and errors. The solution also compiled, with the existing `Microsoft.OpenApi 2.0.0` security warning described in [SECURITY_REVIEW.md](../security/security-review.md).

Desktop and mobile captures are listed in [the preview gallery](../screenshots/README.md). Public pages were inspected in Chromium with local assets. A production database, authenticated staff mutations, real Stripe payments, fulfillment, shipping and production deployment were not verified. No automated test suite or penetration test was run.

The backend security findings in the security guide still need their own remediation work. This frontend refresh addresses the local return-URL behavior and clarifies product wording; it does not certify the application for production.
