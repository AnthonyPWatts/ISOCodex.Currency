# Changelog

## 0.1.0-alpha.1

Initial public prerelease of `ISOCodex.Currency`.

Includes:

- `CurrencyCode` value object for registered ISO 4217-style alpha-3 codes.
- `CurrencyInfo` metadata and default registry.
- Immutable `Money` value object with currency-specific precision validation.
- Explicit standard, cash, custom decimal-place, and custom-increment rounding policies.
- Exact minor-unit conversion where the currency defines applicable minor units.
- Same-currency arithmetic guardrails.
- Money allocation and installment helpers.
- Culture-aware money formatting with explicit currency display choices.
- Conservative money parsing with structured parse failure reasons.
- Extended examples for CSV import and checkout pricing.
- Repeatable currency data seed generation workflow.

Known limitations:

- Currency data is generated from a small checked-in seed rather than a full ISO/CLDR snapshot.
- No structured validation result API yet.
- No JSON converters yet.
- No Entity Framework Core helpers yet.
- No exchange-rate abstractions yet.
