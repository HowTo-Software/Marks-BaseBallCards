# Frontend delivery notes — October 3, 2026

**Status:** summary of the existing frontend commit, not a new deployment.
**Source:** `39a8ecc90bcca4a0eb0d109360f7bae42d4f1e76`.

## Included

The delivery refreshes the public archive, marketplace, story, sign-in and staff
workspaces using one editorial visual system. It adds light/dark themes,
English/PT-BR/Spanish controls, persisted display preferences, adapted heading and
scroll effects, local fonts and original artwork.

The archive gains search, marker filters, sorting, grid/list layouts, dialogs and
card-number links. API pages have clearer loading/failure states. Checkout requires
a validated hosted redirect and a server-confirmed paid status. Login return paths
are restricted to the application.

## Compatibility

The contribution retains .NET/Blazor, existing server routes, role names and shared
DTOs. It does not change backend persistence. The public archive is a committed
historical snapshot; inventory edits do not modify it. Currency remains USD in all
languages.

## Recorded validation

The original notes record client compilation with zero warnings/errors and public
Chromium visual inspection, including mobile, themes and translated pages. The
solution compilation recorded the existing API OpenAPI warning. No automated test
suite, authenticated staff mutations or real Stripe payments were exercised.

See [the evidence table](../phase-4-testing/test-plan.md) and
[preview gallery](../screenshots/README.md).

## Remaining work and approval boundary

The commit records Hank's approval of the frontend as reported by the workspace
owner. That does not establish a production deployment or approval of this later
documentation reorganization.

Live SQL, staff access, Stripe fulfillment, shipping, production TLS, CI/CD and
remaining security findings need Hank's verification/documentation. See
[handoff](../phase-1-inception/repository-assessment.md). No release number or
production URL is assigned because no such release evidence was provided.
