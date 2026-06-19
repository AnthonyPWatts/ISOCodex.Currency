# ISOCodex.Currency

ISOCodex.Currency is an early-stage .NET library for ISO 4217-style currency codes, currency metadata, immutable money values, and explicit currency rounding.

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

The current implementation includes:

- `CurrencyCode`
- `CurrencyInfo`
- `CurrencyMinorUnit`
- `CurrencyKind`
- `ICurrencyRegistry`
- `DefaultCurrencyRegistry`
- `Money`
- `CurrencyRoundingPolicy`
- `CurrencyRoundingService`

Allocation, formatting, validation, JSON, persistence, and exchange-rate abstractions are planned follow-up epics.

## Rounding

Rounding is explicit. `Money.Multiply`, `Money.Divide`, and `Money.Round` require a `CurrencyRoundingPolicy` so business code chooses the midpoint behaviour and whether standard, cash, custom decimal-place, or custom-increment rounding applies.

```csharp
var rounded = Money.Of(1m, CurrencyCode.GBP)
    .Multiply(1.005m, CurrencyRoundingPolicy.AwayFromZero());
```

## Non-goals

This package does not provide live exchange rates, financial advice, tax logic, accounting decisions, or runtime network updates for currency data.
