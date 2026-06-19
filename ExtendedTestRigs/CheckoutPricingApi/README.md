# CheckoutPricingApi

Minimal API POC for a checkout-pricing service that accepts line items, applies explicit currency rounding, and returns subtotal, tax, and total.

## Run

```bash
dotnet run --project ExtendedTestRigs/CheckoutPricingApi --urls http://localhost:5000
```

Then use `CheckoutPricingApi.http` or call the endpoints directly.

## Features exercised

- API boundary DTOs with primitive amount/currency fields.
- Currency code parsing and metadata lookup.
- Construction of valid `Money` values before arithmetic.
- Same-currency subtotal calculation.
- Tax calculation with explicit midpoint rounding.
- Cash rounding preview for currencies such as CHF.

## Known limitations

- This is not a real checkout workflow.
- There is no JSON converter package yet, so responses use simple amount/currency DTOs.
- Validation is intentionally API-local until the library has structured validation results.
