# Monitoring and operational ownership

**Status:** observable client behavior; production monitoring pending Hank.
**Updated:** October 3, 2026.

## What exists in the inspected source

The frontend has user-visible loading, unavailable, retry and access states, an
application reload notice, and a layout error boundary. Browser Console and Network
can be used for local diagnosis. A 20-second HTTP timeout is set in Client Program.

The API logs initialization and database retries. Compose captures the web process
output through the existing service logs. None of these constitutes a configured
production alerting or observability platform.

No client telemetry SDK, committed monitoring dashboard, availability probe,
alert routing configuration or measured SLO was found in the inspected application
and deployment files. External infrastructure was not inspected.

## Pending operational documentation

Hank/maintainers need to document actual log/metric providers, retention/redaction,
health signals, alert recipients, production URL, deployment identity, incident
contacts and recovery targets from the deployed configuration. Do not assign an
uptime percentage or on-call schedule without that evidence.

For the frontend contribution, useful future signals include asset load failures,
unexpected API status/timeout rates and checkout-status failures. These are
recommendations, not implemented metrics. Any telemetry change should keep staff
tokens, secrets, seller/customer details and checkout references out of routine logs.

Use the [runbook](runbook.md) for current diagnostics and the
[security review](../security/security-review.md) for release findings that require
backend/deployment verification.
