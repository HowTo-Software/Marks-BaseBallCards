# Contributing to the frontend

**Scope:** HTS frontend contribution. **Updated:** October 3, 2026.

Start with [onboarding](docs/phase-3-development/onboarding.md), then read the
[technical design](docs/phase-2-design/technical-design.md) and
[client/API contracts](docs/phase-2-design/api-specification.md). Coordinate backend
contract and deployment changes with Hank.

## Before changing a feature

Identify the page, shared component, translation keys and API contract it uses. Keep
the archive separate from live inventory, retain USD labels, and preserve the
distinction between illustrations and physical cards. Existing motion sources and
license notices are documented in [UI references](docs/phase-2-design/ui-references.md).

Never put connection strings, JWT signing keys, staff passwords or Stripe secret
keys in the client or `wwwroot`. Browser assets can be downloaded by every visitor.
Report sensitive findings through the process in [SECURITY.md](SECURITY.md).

## Making a reviewable contribution

1. Create a focused branch following the [Git workflow](docs/phase-3-development/git-workflow.md).
2. Follow the [coding and content conventions](docs/phase-3-development/coding-standards.md).
3. Update both translation tables when UI copy changes. Preserve formatting arguments.
4. Use the [review cases](docs/phase-4-testing/test-cases.md) relevant to the change;
   distinguish checks performed from checks awaiting backend access.
5. Include a clear description of the behavior, changed files, screenshots where
   useful, and validation performed. Do not mark unavailable checks as passing.
6. Update the documentation and [changelog](CHANGELOG.md) when behavior or setup changes.

The source history records the frontend delivery on `feat/hts-frontend-refresh`.
Branch protection, mandatory reviewers and release permissions are not documented
in the inspected repository. Confirm those policies with the maintainers before
merging or releasing; this guide does not establish them.
