# User troubleshooting

**Status:** guidance for current UI states. **Updated:** October 3, 2026.

| Problem | What to try |
| --- | --- |
| No archive result matches | Clear search/marker filters, or search a shorter player name/card number |
| Marketplace says it cannot be reached | Check your connection and use Try again; browse the archive while live data is unavailable |
| Marketplace has no listings | An empty successful response can be correct; the archive does not imply sale availability |
| Buy is disabled | The listing may be sold, have no positive price, or another checkout request may still be running |
| Login is unavailable | Retry after checking connectivity; a standalone frontend preview has no configured API |
| Sign-in credentials fail | Check username/password and contact the owner through the established private channel; repeated attempts may be throttled or locked |
| A staff page is restricted | Confirm the account's role; Admin and SystemAdmin have different access |
| Display choice is forgotten after reload | Browser storage may be disabled, cleared or unavailable in a private session; in-page choices can still work |
| Seller text remains English/original | Seller descriptions, names and stored audit details are intentionally not automatically translated |
| Payment is not confirmed yet | Use Check payment; if uncertainty continues, ask the owner to inspect the payment/server record privately |
| Interface looks old after an update | Reload the page; if needed, ask the developer to check served assets/cache against the expected release |
| Error/reload notice appears | Reload once; if the problem returns, provide route, browser, time, language/theme and a redacted capture |

Do not share passwords, bearer tokens, full payment details or secret keys in a
public report. For developer diagnosis, use the
[frontend runbook](../phase-6-operations/runbook.md). Production service, account
recovery and payment reconciliation procedures remain pending Hank/the site owner.
