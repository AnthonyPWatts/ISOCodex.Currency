# Release Notes

## 0.1.0-alpha.5

Adds the optional `ISOCodex.Currency.Json.SystemTextJson` package.

This release adds explicit System.Text.Json converters for `CurrencyCode` and `Money`. The core package remains independent of `System.Text.Json`; consumers opt in by referencing the integration package and registering the converters.

## 0.1.0-alpha.4

Adds runtime-visible currency data provenance.

This release adds `CurrencyDataVersion` so applications can log that the current prerelease registry is backed by a small checked-in seed rather than a full ISO/CLDR snapshot. No runtime network calls or data pipeline changes were introduced.

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
