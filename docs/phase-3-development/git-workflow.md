# Git workflow for the frontend contribution

**Status:** observed history plus contribution guidance.
**Updated:** October 3, 2026.

## Repository and delivered branch

- Repository: [HowTo-Software/Marks-BaseBallCards](https://github.com/HowTo-Software/Marks-BaseBallCards).
- Frontend delivery branch: `feat/hts-frontend-refresh`.
- Frontend commit: `39a8ecc90bcca4a0eb0d109360f7bae42d4f1e76`.
- Application baseline: `b08233bb6f6d38f18b64270e2ba0e0b88579364d`.

The frontend was delivered on a separate branch, following the workspace owner's
request to avoid direct publication to `main`. The documentation reorganization
is delivered in a separate commit on `docs/frontend-documentation`, based on the
approved frontend delivery. Integration into `main` remains a maintainer action.

## Suggested workflow for a follow-up

Inspect your current work before creating a branch:

```powershell
git status --short --branch
git diff --stat
git switch -c docs/frontend-handoff
```

The example branch is concrete to this documentation change. Pick another focused
name when making a different contribution. Do not discard unrelated changes or
rewrite shared history to prepare the branch.

Before committing, inspect the exact files being staged and their diff. Keep
generated `bin`/`obj`, local `.env`, credentials, editor histories and logs out of
the commit. The existing [.gitignore](../../.gitignore) defines the current exclusions.

Use a message describing the behavior and scope, such as
`docs: organize frontend handoff by lifecycle phase`. A frontend behavior change
should describe its visible effect and any API dependency.

## Review description

Include:

- Why the change was made and which journey/files it affects.
- Evidence from the current revision, including relevant captures.
- Build/manual checks performed, with environment and outcome.
- Integration checks still pending and the reason.
- API, asset, translation or documentation compatibility changes.

No committed file establishes required reviewers, branch protection, merge method,
release cadence or signing policy. Those policies remain pending maintainer
confirmation. Hank owns the requested backend documentation handoff; a change to
server contracts should be coordinated before updating the frontend.

No local branch instruction grants permission to merge or deploy. Use the project
owner's agreed publication process for those actions.
