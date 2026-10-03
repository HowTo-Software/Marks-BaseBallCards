# Frontend diagnostic runbook

**Status:** source-based diagnostic procedures. **Updated:** October 3, 2026.

Use this guide to separate an asset/rendering problem from an API, account or payment
problem. It does not establish a production incident response team or SLA.

## First collect context

Record the route, source/artifact revision if known, browser, viewport, language/theme,
time and whether this is standalone client or API-hosted mode. Inspect browser
Console and Network with secrets redacted. Do not copy JWTs, staff passwords,
connection strings or payment/customer details into a report.

## Symptom guide

| Symptom | First checks | Responsible area |
| --- | --- | --- |
| Entire page fails to start | Check HTML, `_framework`, local CSS/font/JS requests and console exceptions; confirm files belong to one artifact | Frontend assets/host |
| Archive cannot load | Inspect `data/collection.json`; use the archive retry action; check route/base path and JSON response | Frontend/static host |
| Archive works; marketplace/login unavailable | Confirm origin: 5248 standalone does not proxy to 5238; inspect actual `/api` response | Integration/host, then backend |
| API-hosted site never starts | Inspect API startup logs for configuration/SQL/migration failure; database initialization precedes serving the client | Hank/backend |
| Old styles or language copy after deployment | Reload with browser cache disabled; compare HTML, scripts and compiled client to the intended artifact | Assets/cache policy |
| Theme/language choice is not retained | Check storage permissions and `hts.theme`/`hts.language`; storage-denied fallback is session-only | Browser preferences |
| Sign-in appears to work but staff route fails | Inspect response status and account role; confirm token storage is allowed and token has not expired | Account/backend/auth integration |
| Dashboard totals load but history does not | Inspect the independent `api/history?take=200` request; the UI preserves valid statistics | Backend/history |
| Purchase status remains unconfirmed | Keep the reference private; inspect status endpoint outcome; ask Hank to check Stripe/server fulfillment | Backend/payment operations |
| Unexpected route shows not found | Check route spelling and host SPA fallback; current app expects base `/` | Frontend/static host |

A 401 indicates authentication is needed/expired; a 403 indicates insufficient API
access; 429 indicates throttling. A client loading/error message by itself does not
prove which downstream dependency failed. Capture the actual response before changing code.

## Existing local/container commands

For standalone preview, use [onboarding](../phase-3-development/onboarding.md) and
stop the terminal process with Ctrl+C when finished.

For the configured Compose host:

```powershell
docker compose ps
docker compose logs --tail 100 web
```

These are diagnostics of the existing web service. SQL is external to Compose and
requires Hank's runbook. Do not restart a production service or rerun seeding as a
substitute for investigating a payment/inventory discrepancy.

## Browser storage recovery

The display keys are separate from `mbc_token`. Removing a display key resets a
preference to browser defaults on a later load; it does not reset the collection.
Use the visible logout action for a staff session where possible. Clearing a token
only removes this browser's copy; it does not revoke an already-copied credential.

Storage restrictions can prevent login even while public browsing and display
controls work. Do not resolve that by embedding credentials in client assets.

## Asset recovery and backend handoff

The authoritative frontend source and local artwork/translations are committed.
A rebuild or restoring a known coherent artifact can recover those assets, subject
to compatible backend contracts and the maintainer's release procedure.

A standalone frontend disaster-recovery plan is **N/A** because there is no
authoritative browser database. SQL backups, account recovery, migration rollback,
paid-order reconciliation, RPO/RTO, incident ownership and production service
management are **pending Hank**. Browser preferences are not a backup of inventory
or sales. See [monitoring](monitoring.md) and [handoff](../phase-1-inception/repository-assessment.md).
