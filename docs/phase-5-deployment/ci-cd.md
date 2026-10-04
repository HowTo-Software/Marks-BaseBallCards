# CI/CD status

**Status:** one committed API build and deployment workflow; no automated test project.
**Updated:** October 3, 2026.

[`deploy.yml`](../../.github/workflows/deploy.yml) builds the API in Release
configuration on pull requests to `main`. On pushes to `main` and manual dispatch,
it also runs a deployment job on the configured self-hosted runner. That job writes
the `ENV_FILE` secret to a temporary `.env`, updates the database name, copies the
checkout and `.env` (mode 600) to `/opt/Marks-BaseBallCards` on the deployment host
(192.168.1.206), rebuilds the Compose service there, checks the HTTP endpoint and
removes the runner's copy of `.env`. Production stacks on that host live in
`/opt/<repository name>`, never in a user's home directory; the directory is created
once by root (`sudo install -d -o htsadmin -g htsadmin -m 750 /opt/Marks-BaseBallCards`).
The workflow does not run automated tests or verify browser behavior.

## Evidence and boundaries

The workflow confirms the configured build/deploy steps, not their production
readiness or a successful release. No live runner, deployment, database migration,
TLS termination, rollback or payment operation was inspected for this frontend
documentation work. The documented manual review process remains in the
[test plan](../phase-4-testing/test-plan.md).

## Pending Hank / maintainer

Document the runner and host ownership, secret provisioning/rotation, database
backup and migration recovery, TLS/proxy configuration, deployment verification and
rollback process. No deployment token, release approval policy or coverage threshold
is inferred from the workflow.

Automated browser/API tests and continuous dependency checks are not configured
pipeline stages. They may be added in future work with an implementation and
execution evidence.

The template's `.github` directory was not copied. The existing workflow is
documented from its checked-in configuration; Copilot/VS Code histories, chat
sessions, transcripts and logs do not belong there.
