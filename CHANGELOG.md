# Changelog

Changes are identified by the revisions available in this repository. No version
tags or published release numbers were found during the October 3 documentation review.

## Unreleased — documentation organization

- Organize frontend documentation using the `best-document` lifecycle structure.
- Add setup, architecture, client contracts, retrospective ADRs, manual review cases,
  deployment boundaries, operations guidance and user instructions.
- Preserve the security review, visual provenance, theme/language guide and screenshots.
- Record backend and infrastructure handoff items for Hank and the pending project license.

## October 3, 2026 — frontend contribution (`39a8ecc`)

[Commit and complete delivery description](https://github.com/HowTo-Software/Marks-BaseBallCards/commit/39a8ecc90bcca4a0eb0d109360f7bae42d4f1e76).
The commit records Hank's approval as reported by the workspace owner.

- Editorial HTS identity, refreshed public pages and staff workspaces.
- OriginKit heading and Skiper scrolling behavior adapted to native Blazor/CSS/JavaScript.
- Light/dark themes, English/PT-BR/Spanish UI and persisted preferences.
- Public 64-record archive, search, filters, sorting, views and detail links.
- Recoverable API states, restricted checkout redirects, payment-status checks and
  restricted login return paths.
- Source security/data review, provenance and public preview captures.

The implementation notes record a client build and public browser inspection.
No automated suite, real payments or SQL-backed staff mutations were run.

## June 28, 2026 — application baseline (`b08233b`)

[Baseline commit](https://github.com/HowTo-Software/Marks-BaseBallCards/commit/b08233bb6f6d38f18b64270e2ba0e0b88579364d).

The recorded baseline provides the .NET 10 Blazor client, ASP.NET Core API, shared
DTOs, SQL Server persistence, JWT roles, Stripe integration and Docker web host.
This entry summarizes that commit; it does not document any subsequent backend
restructuring outside the inspected tree.
