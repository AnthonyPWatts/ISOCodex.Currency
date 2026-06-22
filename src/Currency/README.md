# ISOCodex.Currency

Small, framework-agnostic ISO 4217-style currency metadata, immutable money values, and explicit currency rounding for .NET.

## Install

```bash
dotnet add package ISOCodex.Currency --version 0.1.0-alpha.1
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

Cash rounding is available where metadata provides an increment:

```csharp
var cashTotal = Money.Of(1.03m, CurrencyCode.CHF)
    .Round(CurrencyRoundingPolicy.Cash()); // CHF 1.05
```

## Allocation and installments

Split money into exact minor-unit parts:

```csharp
var allocation = Money.Of(10.00m, CurrencyCode.GBP)
    .Allocate(3, AllocationRemainderStrategy.First);
```

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

## Current limitations

This pre-1.0 package does not yet include JSON converters, EF Core helpers, or exchange-rate abstractions.
