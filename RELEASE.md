# Release Notes

## 0.1.0-alpha.3

Adds non-throwing `Money` validation APIs for boundary input.

This release adds `MoneyValidationResult`, `MoneyValidationFailureReason`, `Money.TryCreate(...)`, and `Money.TryFromMinorUnits(...)` so APIs, forms, CSV imports, and partner integrations can validate amount/currency input without exception control flow. Existing strict `Money.Of(...)` and `Money.FromMinorUnits(...)` behaviour is unchanged.

## 0.1.0-alpha.2

Adds default-value safety for `CurrencyCode` and `Money`.

This release adds `IsDefault` detection for value-type defaults and clearer operational errors when uninitialised `Money` values leak into arithmetic, rounding, allocation, comparison, or minor-unit conversion.

## 0.1.0-alpha.1

Initial public prerelease of `ISOCodex.Currency`.

This release includes currency code parsing, default currency metadata, immutable money values, explicit rounding, exact minor-unit conversion, allocation helpers, installment helpers, culture-aware formatting, conservative parsing, and consumer-shaped example projects.

See `CHANGELOG.md` for the full scope and `docs/release-gate.md` for the publication checklist.
