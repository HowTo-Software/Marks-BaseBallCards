# Security guidance and reporting

**Scope:** source review and frontend boundaries. **Updated:** October 3, 2026.

The [Security Validation and Remediation Guide](docs/security/security-review.md)
contains the existing findings, exact code locations, recommended fixes and
verification steps. SEC-10 records a client source remediation; the remaining
backend and deployment findings have not been cleared by this documentation work.
The [frontend threat model](docs/security/threat-model.md) explains the browser/API
trust boundary.

## Reporting a suspected vulnerability

Use the private project channel already agreed with the repository maintainers or
Hank. Include the affected revision, relevant route/file, reproduction steps, impact
and a redacted example. Do not include live passwords, bearer tokens, database
credentials, Stripe keys or customer payment information in an issue or screenshot.

**Pending maintainer setup:** no dedicated security address, private advisory
configuration, response SLA or supported-version policy is established in the
reviewed files. If you do not already have a private contact, ask the maintainer for
one before sharing exploit details. This document does not invent a reporting address.

## Release review

The source review is not a production certification. TLS/proxy settings, secrets,
SQL privileges, package resolution, concurrent purchases and Stripe fulfillment
require backend/deployment verification. Use the guide's individual verification
steps and record the evidence with the release. These areas are assigned to Hank
for documentation handoff, as requested by the workspace owner.
