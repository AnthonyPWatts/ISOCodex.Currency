# ISOCodex.Currency

Small, framework-agnostic ISO 4217-style currency metadata, immutable money values, and explicit currency rounding for .NET.

## Install

```bash
dotnet add package ISOCodex.Currency --version 1.0.1
```

## What it is useful for

Use this package when application code needs to:

- validate and normalise alpha-3 currency codes
- inspect currency metadata such as numeric code and decimal precision
- keep money amounts tied to their currency
- prevent accidental cross-currency arithmetic
- enforce currency-specific amount precision
- make rounding rules explicit and testable
- format and conservatively parse money values

## Quick start

```csharp
using ISOCodex.Currency;

var code = CurrencyCode.Parse("gbp");
var currency = DefaultCurrencyRegistry.Instance.Get(code);

Console.WriteLine(currency.EnglishName); // Pound Sterling
```

## Money

`Money` validates amount precision against the currency metadata.

```csharp
var item = Money.Of(12.99m, CurrencyCode.GBP);
var shipping = Money.Of(3.49m, CurrencyCode.GBP);
var total = item + shipping;
```

`Money.IsDefault` and `CurrencyCode.IsDefault` detect uninitialised value-type defaults. Prefer `Money.Zero(currency)` or `Money.Of(amount, currency)` when creating real money values.

For API and import boundaries, use `Money.TryCreate(...)` or `Money.TryFromMinorUnits(...)` to receive a `MoneyValidationResult` with a stable `MoneyValidationFailureReason` instead of using exceptions for ordinary invalid input.

`CurrencyDataVersion` exposes the current pinned SIX ISO 4217 and Unicode CLDR snapshot provenance at runtime. The data is derived metadata, not an official ISO 4217 redistribution.

Different currencies cannot be added, subtracted, or compared.

```csharp
var gbp = Money.Of(10m, CurrencyCode.GBP);
var usd = Money.Of(10m, CurrencyCode.USD);

var invalid = gbp + usd; // throws
```

## Rounding

Rounding is explicit and separate from construction.

```csharp
var rounding = new CurrencyRoundingService();
var currency = DefaultCurrencyRegistry.Instance.Get(CurrencyCode.GBP);
var roundedAmount = rounding.RoundAmount(
    12.345m,
    currency,
    CurrencyRoundingPolicy.AwayFromZero());

var money = Money.Of(roundedAmount, CurrencyCode.GBP);
```

Use `Money.Multiply`, `Money.Divide`, and `Money.Round` when the value is already valid money:

```csharp
var tax = Money.Of(19.99m, CurrencyCode.GBP)
    .Multiply(0.2m, CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven));
```

The rounding policy matters. For example, `GBP 10.05 * 10%` produces `GBP 1.00` with midpoint-to-even rounding and `GBP 1.01` with away-from-zero rounding.

Cash rounding is available where metadata provides an increment:

```csharp
var cashTotal = Money.Of(1.03m, CurrencyCode.CHF)
    .Round(CurrencyRoundingPolicy.Cash()); // CHF 1.05

var roundedToQuarter = Money.Of(1.38m, CurrencyCode.GBP)
    .Round(CurrencyRoundingPolicy.CustomIncrement(0.25m, MidpointRounding.AwayFromZero)); // GBP 1.50
```

## Allocation and installments

Split money into exact minor-unit parts:

```csharp
var allocation = Money.Of(10.00m, CurrencyCode.GBP)
    .Allocate(3, AllocationRemainderStrategy.First);
```

Remainder strategy controls which parts receive the extra minor units. Splitting `GBP 10.05` into six parts can place the extra pennies at the start, at the end, or spread through the allocation while preserving the total.

Create installment plans with Money-based strategies:

```csharp
var strategy = new EvenSplitInstallmentStrategy(AllocationRemainderStrategy.Last);
var plan = strategy.CalculateInstallments(
    new InstallmentRequest(Money.Of(10.00m, CurrencyCode.GBP), 3));
```

## Formatting and parsing

Format with explicit currency display choices:

```csharp
var formatter = new MoneyFormatter();
var price = Money.Of(12.34m, CurrencyCode.GBP);

formatter.Format(price); // GBP 12.34
formatter.Format(price, new MoneyFormatOptions(new CultureInfo("en-GB"), MoneyCurrencyDisplay.Symbol)); // £12.34
```

Parse conservatively with result objects:

```csharp
var parser = new MoneyParser();
var result = parser.Parse("GBP 12.34", new MoneyParseOptions(CultureInfo.InvariantCulture));
```

Symbol parsing requires an expected currency, because symbols can be ambiguous.

## Boundaries and persistence

At API, import, and database boundaries, keep amount and currency code separate:

| Value | Suggested shape |
| --- | --- |
| Amount | `decimal` / `decimal(19,4)` or wider |
| Currency | uppercase `char(3)` alpha-3 code |

For exact payment boundaries, `Money.ToMinorUnits()` and `Money.FromMinorUnits(...)` support integer minor-unit conversion where the currency defines applicable minor units.

## Advanced registries

Static `Money` factories use `DefaultCurrencyRegistry.Instance`. Use `MoneyFactory` when an application needs an explicit registry, such as a controlled test currency, an internal accounting unit, or an alternate metadata snapshot.

```csharp
var customCode = CurrencyCode.CreateCustom("ZZA");
var registry = new DefaultCurrencyRegistry(new[]
{
    new CurrencyInfo(
        customCode,
        "999",
        "Internal test unit",
        new CurrencyMinorUnit(4),
        CurrencyKind.Testing,
        false)
});

var factory = new MoneyFactory(registry);
var money = factory.Of(12.3456m, customCode);
```

`CurrencyCode.Parse(...)` and `CurrencyCode.TryParse(...)` remain strict and only accept codes from the packaged registry. Use `CurrencyCode.CreateCustom(...)` only with an explicit registry.

## Current limitations

The core package does not include EF Core helpers or live exchange-rate providers.
JSON converters are available in the optional `ISOCodex.Currency.Json.SystemTextJson` and `ISOCodex.Currency.Json.NewtonsoftJson` packages.
Country/currency validation helpers are available in the optional `ISOCodex.Currency.Countries` bridge package.
Analyzer diagnostics are available in the optional `ISOCodex.Currency.Analyzers` package.
Provider-neutral exchange abstractions are available in the optional `ISOCodex.Currency.Exchange.Abstractions` package.
