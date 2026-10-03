# Manual frontend review cases

**Status:** executable review guidance; results not filled in.
**Updated:** October 3, 2026.

Cases marked **integration** depend on Hank's development environment. The
[test plan](test-plan.md) identifies existing evidence and execution limits.
Use source-backed expectations below; record the actual result in the change review.

## Public preview cases

| ID | Steps | Expected behavior |
| --- | --- | --- |
| UI-01: public navigation | Start the standalone client; open home, collection and about; follow header/footer links | Local pages load, titles/content match their routes; no API credentials needed |
| UI-02: archive data/search | Open `/collection`; clear filters, search a player from the archive, then card number `1`; try a nonmatching term | Original 64-record archive; correct filtered records or explicit no-results state |
| UI-03: filters/views | Use Rookie/Royals filters, number/name sort and grid/list; clear filters | Only matching annotations shown; view/sort changes preserve coherent results |
| UI-04: record dialog | Open `/collection?card=1`; close with Escape; open by keyboard and close again | Record number 1 opens; dialog closes and focus returns to its opener when it remains present |
| UI-05: theme persistence | Toggle light/dark, visit another public route, reload | Shared frame/forms/dialogs use the chosen theme; saved choice persists when storage is available |
| UI-06: languages and page state | Enter an archive search, open a record, switch EN/PT-BR/ES | Visible UI changes without resetting search/dialog; player names and source data remain original |
| UI-07: responsive/keyboard | Use a narrow viewport; operate mobile navigation, skip link, fields and dialog with keyboard | Controls remain reachable, focus visible, mobile menu closes after navigation |
| UI-08: reduced motion | Enable reduced motion in browser/OS; load home; change the preference while open | Heading/reveal/tilt/stack animations stop or remain disabled; content stays readable |
| UI-09: missing services | In standalone mode open marketplace and login; try reloading/retrying | Clear recoverable unavailable state, without fabricated stock or successful login |
| UI-10: missing payment reference | Open `/buy/success` without `session_id` | Missing-reference guidance, no confirmed purchase |
| UI-11: route recovery | Open an unknown route, `/not-found` and `/access-denied` | Matching recovery UI; working navigation back to public pages |

## Browser state cases

Use a disposable browser profile so existing staff state is not affected.

- **STATE-01:** on a fresh profile with no saved display keys, compare initial
  appearance/language with supported browser settings. Unsupported languages use English.
- **STATE-02:** deny browser storage and change language/theme. Public browsing and
  in-page display changes should remain usable. This does not promise successful staff login
  when token storage is blocked.
- **STATE-03:** place a malformed or expired token under `mbc_token` in the disposable
  profile and reload. The auth provider should produce anonymous UI state rather
  than breaking the public page. This is not server signature verification.
- **STATE-04:** after a language change, inspect number/date formatting where data is
  available. Amounts retain USD labels and values; stored condition/notes stay original.

## API-hosted integration cases

| ID | Setup and steps | Expected behavior |
| --- | --- | --- |
| INT-01: roles | Sign in with an approved Admin account, then separately a SystemAdmin account; open each staff route | Role-appropriate navigation; other protected role route restricted; API independently enforces access |
| INT-02: inventory validation | On development records, submit missing number/name or an invalid doubles value; try listing with no positive price | Local/shared validation explains errors; no false saved state |
| INT-03: inventory mutation | Create/edit a disposable record; cancel sale/delete confirmation; then perform an approved development mutation | Cancel leaves record untouched; confirmed success follows API response; failed response is recoverable |
| INT-04: listing lifecycle | Change a development record's price, condition and sale status; refresh marketplace | Marketplace reflects API state, not historical archive; USD and seller text preserved |
| INT-05: dashboard | Open system workspace; compare displayed totals/filing ratios to returned DTOs; simulate history failure independently | Filing denominator is current records; history error does not replace valid statistics |
| INT-06: login return path | Sign in with a local ReturnUrl, then external/protocol-relative/backslash variants using a development account | Accepted local path works; invalid destination falls back to the role-specific page |
| INT-07: API failure | Use a development failure/expired-token response during a mutation | No success message from a failed request; correct recoverable status/error guidance |

## Stripe test cases

- **PAY-01:** start checkout for a listed development record with a positive price.
  Confirm the browser goes only to an HTTPS `checkout.stripe.com` URL and the
  amount matches the backend record. Repeated clicks should not start concurrent
  UI submissions while the request is pending.
- **PAY-02:** complete checkout in Stripe test mode and return. Confirmed UI requires
  the server's `paid` response; the presence of `session_id` alone is insufficient.
- **PAY-03:** leave checkout or return while confirmation is unavailable. Verify
  pending/retry copy without claiming that payment failed.
- **PAY-04:** leave the return page during polling. Subsequent loop iterations
  should stop; do not assume an already-running status HTTP request was aborted.

Backend concurrency, reservation, delayed events, webhook retries and refunds need
the security guide's own verification and Hank's integration procedures. UI guards
alone cannot establish those outcomes.
