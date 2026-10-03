# Using Mark's Baseball Cards

**Status:** guide to the delivered frontend. **Updated:** October 3, 2026.

## Browse the collection

The home page introduces the collection and links to the archive, marketplace and
story. **Collection** is the original 64-record checklist. It is useful for exploring
names, numbers and the collector's rookie/Royals markers; it does not show current stock.

On the collection page:

1. Search by player name or card number.
2. Use the marker filters and number/name sorting.
3. Switch between grid and list views.
4. Select a record to open its details. Close with the close button or Escape.
5. Follow its marketplace link to check whether a live listing exists.

Covers are illustrated record artwork, not photographs of the physical cards.
A marker is a collector annotation, not a grading or authenticity certificate.

## Choose appearance and language

Use the header's sun/moon control to change light/dark appearance. Choose **EN**,
**PT-BR** or **ES** for English, Brazilian Portuguese or Spanish.

The first visit uses supported browser language and system appearance. Saved choices
take priority on later visits when browser storage is available. Language changes
update the current interface without clearing an archive search or open record.

Names, card numbers, seller-entered condition/notes and stored activity details keep
their original content. Prices remain **USD** in every language; only formatting
changes. Dates use the selected locale and the browser's local timezone.

## View listings and buy

**Marketplace** loads current listings from the server. Search, category filters,
price/name/number sorting and **Include sold** help inspect the results. Open a
listing to review its price, condition and notes before choosing Buy.

Purchasing leaves the site for Stripe's hosted Checkout. Browsing and buying do not
require a staff login. Sold listings or listings without a positive price cannot
be bought through the button.

On return, the site checks the checkout reference with the server. **Payment
confirmed** appears only when that service reports a paid session. An unconfirmed
message can mean confirmation is still unavailable; it does not automatically mean
the payment failed. Use **Check payment** to try again. A return link with no session
reference cannot confirm a purchase.

Shipment, refunds and delivery timing are not defined by this frontend guide.
Those policies must come from the site owner/Hank. If confirmation remains unclear,
use the owner's established support channel and keep payment details private.

## Staff sign-in

Staff accounts are supplied by the site owner. There is no public account creation
or password-reset page. Open **Staff access**, enter the supplied username/password
and sign in. The password-visibility button changes whether the entered text is shown.

Your role determines which workspace is available. An Admin account and a
SystemAdmin account have different permissions; one is not automatically the other.

### Inventory workspace — Admin

The workspace shows live records and listing status. Use the search and status
filter to find a record, then create or edit its name/number, plastic storage,
six filing flags, doubles, marker flags, price, condition and listing notes.

To list a record, provide a price above zero. Validation messages explain missing
or out-of-range values. A save is completed only after the server accepts it.

**Mark sold** and **Delete** open a confirmation. Review the record and action before
confirming. Deletion removes the inventory record; the historical public archive
does not change. A canceled confirmation leaves the record as it was.

### System workspace — SystemAdmin

The dashboard shows current inventory totals, markers, recorded doubles, listing/
sold counts and recorded sales in USD. The six progress bars show filing membership
as a share of **current inventory records**, not completion of the full Topps set.

Activity shows the latest returned entries, up to 200. **Refresh data** updates the
server data. History can fail independently while valid statistics remain visible.

## When a page cannot load

An unavailable marketplace is different from an empty marketplace. Use the page's
retry action, check the connection, or continue browsing the historical archive.
For more guidance, see [troubleshooting](troubleshooting.md) and [FAQ](faq.md).
