# CI/CD status

**Status:** no committed pipeline in the reviewed snapshot.
**Updated:** October 3, 2026.

No `.github/workflows`, other committed pipeline configuration or automated test
project was found at the reviewed frontend revision. The Dockerfile and Compose
file describe packaging/runtime commands, not a continuous delivery pipeline.

## Documented current path

Developers can restore/build the client, perform the documented manual review and
publish the API-hosted artifact. Commands are in
[onboarding](../phase-3-development/onboarding.md) and
[deployment guide](deployment-guide.md). Earlier implementation results are in
[the test plan](../phase-4-testing/test-plan.md).

## Pending Hank / maintainer

If an external pipeline exists, document its provider, trigger branches, build
commands, artifact identity, configuration injection, required checks, deployment
target and rollback procedure from its actual configuration. No pipeline URL,
deployment token, release approval policy or coverage threshold is invented here.

Automated browser/API tests and continuous dependency checks are **N/A as existing
pipeline stages**. They may be added in future work with an implementation and
execution evidence.

The template's `.github` directory was not copied. Only genuinely needed GitHub
files should be added when the project has an approved workflow or template;
Copilot/VS Code histories, chat sessions, transcripts and logs do not belong there.
