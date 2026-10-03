# Frequently asked questions

**Status:** answers based on the current frontend. **Updated:** October 3, 2026.

## Is a card in the collection archive available to buy?

The archive is the original checklist, not live stock. Check the marketplace listing
for current availability, condition, notes and price.

## Are the covers photographs of actual cards?

No. They are original illustrated record covers. The current client data has no
physical-card scan field or grading-certificate workflow.

## Do Rookie or Royals markers certify a classification?

They preserve the original collector annotations. The frontend delivery does not
independently certify every checklist name or marker.

## Why are there 64 archive records?

That is the supplied original seed/checklist. It is not presented as the full
1991 Topps base set. The [data guide](../phase-2-design/data-model.md) explains the
difference and links to the preserved source review.

## Does Portuguese or Spanish change the currency?

No. Prices and checkout amounts remain USD. The language affects UI text, numeric
separators and date formatting. Seller descriptions remain original.

## Why did the site open in dark mode or another language?

The first visit follows system appearance and supported browser language. A saved
header choice takes priority afterward. Unsupported languages fall back to English.

## Do buyers need a staff account?

No. The current browsing and hosted checkout flow is public. Staff sign-in is for
the inventory and reporting workspaces.

## Can I register or reset a staff password here?

There is no registration/reset page in this contribution. Accounts and recovery
are handled by the site owner; Hank needs to document the backend procedure.

## Does an unconfirmed return page mean payment failed?

No. It means the page could not confirm a paid status. Check again and use the
owner's private support channel if needed. The return URL alone is not proof of payment.

## Does payment confirmation guarantee shipping or delivery?

The page confirms the service's paid status. Shipping, delivery, refunds and order
reconciliation policies are outside this frontend contribution and need owner documentation.

## What do filing percentages measure?

The proportion of current database records flagged as filed in each of six sets.
They do not measure completion against a full canonical Topps checklist.

## Why do Admin and SystemAdmin see different workspaces?

They are distinct server roles. Admin manages inventory; SystemAdmin views statistics
and history. An access restriction can be correct for the signed-in account.
