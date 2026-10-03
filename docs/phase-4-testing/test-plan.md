# Frontend review plan and existing evidence

**Status:** documented evidence plus proposed manual procedures.
**Updated:** October 3, 2026.

This file separates what the earlier delivery records from what a future reviewer
should execute. No application build, automated tests, staff mutations or payment
tests were run for the documentation reorganization.

## Recorded implementation evidence

| Evidence | Source | What it supports | Limit |
| --- | --- | --- | --- |
| Client build with zero warnings/errors | [October 2 reference guide](../phase-2-design/ui-references.md), [delivery commit](https://github.com/HowTo-Software/Marks-BaseBallCards/commit/39a8ecc90bcca4a0eb0d109360f7bae42d4f1e76) | Client compiled during implementation | Not a new build result or backend integration test |
| Solution compilation with existing OpenAPI warning | [Reference guide](../phase-2-design/ui-references.md), [SEC-05](../security/security-review.md#sec-05--a-high-severity-openapi-dependency-advisory-is-present) | Earlier compilation/package finding | Does not prove current resolved packages are patched |
| Public desktop/mobile visual inspection | [Preview gallery](../screenshots/README.md), [theme/language guide](../phase-3-development/themes-and-languages.md) | Public appearance, language/theme surfaces and standalone unavailable states | Screenshots are captures, not automated assertions |
| Source-code security/data review | [Preserved guide](../security/security-review.md) | Source locations, findings and original workbook/seed observations | Not a penetration test or physical-card certification |

No test project, browser test suite or committed CI workflow is present. External
capture tooling used during the visual review was not added as a repository test
harness. Do not describe `dotnet test` as an established project verification command.

## Review environments

| Mode | Use | Prerequisites |
| --- | --- | --- |
| Standalone frontend | Archive, themes/languages, responsive layout, navigation and public failure states | .NET 10 SDK; [onboarding Mode A](../phase-3-development/onboarding.md) |
| API-hosted development | Login, roles, inventory, dashboard and real listing responses | Hank-provided development SQL/configuration and staff accounts |
| Stripe test integration | Hosted checkout, returned status and backend fulfillment checks | API-hosted client plus approved Stripe test setup from Hank |

Use disposable development records for mutation cases. Record the actual environment;
do not test sale/deletion on the owner's real inventory as a routine visual check.

## Proposed review sequence

1. Compile the client using the command in onboarding, and record its output/revision.
2. Exercise public routes in each language and theme.
3. Review a desktop and a narrow mobile viewport, keyboard navigation and reduced motion.
4. Check failures separately from successful empty results.
5. With backend access, exercise staff-role and live listing cases.
6. With Stripe test setup, exercise payment states and the security guide's backend scenarios.
7. Record outcomes and outstanding items using the [manual case IDs](test-cases.md).

The following cases are **procedures to execute**, not a pass report. If a dependency
is unavailable, record it as pending with a reason.

## Accessibility and performance scope

The source includes labels, focus styles, a skip link, native dialogs, accessible
heading text and reduced-motion behavior. A formal accessibility audit, screen-reader
matrix, browser support certification and measured performance budget are **N/A
as completed evidence**; none was recorded in this delivery. Future reviews can add
measured results without implying existing certification.

## Recording a bug or review outcome

Include case ID, source revision, route, language/theme, browser/viewport, environment,
steps, expected behavior and observed result. Attach a redacted capture or console/
network error when useful. Never include staff passwords, JWTs, checkout references
with customer data or secret keys in the report. Follow [SECURITY.md](../../SECURITY.md)
for sensitive findings.
