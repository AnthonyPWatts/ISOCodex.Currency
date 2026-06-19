# ISOCodex.Currency

ISOCodex.Currency is an early-stage .NET library for ISO 4217-style currency codes and metadata. The current implementation covers the first slice of the public API: currency code parsing, currency metadata, and a default registry.

## Project status

This package is pre-1.0. Public APIs may change while the money, rounding, formatting, validation, and persistence features are implemented.

## Projects

- `src/Currency` - core package.
- `tests/Currency.Tests` - xUnit test suite.
- `ManualTestRig` - manual console rig retained for local experiments.

## Package identity

- Package ID: `ISOCodex.Currency`
- Root namespace: `ISOCodex.Currency`
- Target framework: `netstandard2.1`

## Quick start

```csharp
var code = CurrencyCode.Parse("gbp");
var registry = DefaultCurrencyRegistry.Instance;
var currency = registry.Get(code);

Console.WriteLine(currency.EnglishName); // Pound Sterling
```

## Current scope

The first implementation slice includes:

- `CurrencyCode`
- `CurrencyInfo`
- `CurrencyMinorUnit`
- `CurrencyKind`
- `ICurrencyRegistry`
- `DefaultCurrencyRegistry`

Money values, rounding, allocation, formatting, validation, JSON, persistence, and exchange-rate abstractions are planned follow-up epics.

## Non-goals

This package does not provide live exchange rates, financial advice, tax logic, accounting decisions, or runtime network updates for currency data.
