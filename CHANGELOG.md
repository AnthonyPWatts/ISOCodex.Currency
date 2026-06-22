# Changelog

## 0.1.0-alpha.4

Adds runtime-visible currency data provenance.

Includes:

- `CurrencyDataVersion.Identifier`.
- `CurrencyDataVersion.CheckedOn`.
- `CurrencyDataVersion.Description`.
- `CurrencyDataVersion.SourceKind`.
- Tests that the data provenance describes the checked-in seed and preserves known registry entries.

## 0.1.0-alpha.3

Adds non-throwing money validation APIs for API and import boundaries.

Includes:

- `MoneyValidationFailureReason`.
- `MoneyValidationResult`.
- `Money.TryCreate(decimal, CurrencyCode)`.
- `Money.TryCreate(decimal, string)`.
- bool `Money.TryCreate(...)` convenience overloads.
- `Money.TryFromMinorUnits(long, CurrencyCode)`.
- Tests for valid input, over-precision, unknown currencies, default currencies, JPY precision, and minor-unit-not-applicable failures.

## 0.1.0-alpha.2

Adds default-value safety APIs for `CurrencyCode` and `Money`.

Includes:

- `CurrencyCode.IsDefault`.
- `Money.IsDefault`.
- Clear default-value guard errors for operational `Money` methods.
- Tests that equality and hash code remain safe for default `Money` values.

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
- No JSON converters yet.
- No Entity Framework Core helpers yet.
- No exchange-rate abstractions yet.
