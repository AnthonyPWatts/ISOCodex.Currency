# ISOCodex.Currency

ISOCodex.Currency is a small, framework-agnostic .NET library for working with ISO 4217-style currency codes, currency metadata, immutable money values, and explicit currency rounding.

It is aimed at the common places where application code tends to drift into fragile money handling:

- accepting amount/currency input from APIs, forms, files, and integrations
- checking that a currency code is real before storing it
- enforcing currency-specific precision, such as `JPY` having no minor units and `KWD` having three
- keeping amounts and currencies together as one value object
- preventing accidental cross-currency arithmetic
- making rounding decisions explicit and testable
- using currency metadata in checkout, import, billing, and reporting workflows

The package does not try to be an accounting system. It gives application code a safer currency and money foundation.

## Project status

This package is pre-1.0. The implemented API is useful, but broader features such as JSON converters, persistence helpers, and exchange-rate abstractions are still planned.

Current implemented scope:

- `CurrencyCode`
- `CurrencyInfo`
- `CurrencyMinorUnit`
- `CurrencyKind`
- `CurrencyDataVersion`
- `ICurrencyRegistry`
- `DefaultCurrencyRegistry`
- `Money`
- `MoneyValidationFailureReason`
- `MoneyValidationResult`
- `CurrencyRoundingPolicy`
- `CurrencyRoundingService`
- `MoneyAllocator`
- `InstallmentPlan`
- built-in Money-based installment strategies
- `MoneyFormatter`
- `MoneyParser`
- optional `ISOCodex.Currency.Json.SystemTextJson` converters

## Projects

- `src/Currency` - core package.
- `src/Currency.Json.SystemTextJson` - optional System.Text.Json integration package.
- `tests/Currency.Tests` - xUnit test suite.
- `tests/Currency.Json.SystemTextJson.Tests` - JSON converter xUnit test suite.
- `ManualTestRig` - small manual console rig for currency metadata.
- `ExtendedTestRigs/BulkMoneyImportTool` - CSV import example for mixed-currency money data.
- `ExtendedTestRigs/CheckoutPricingApi` - Minimal API example for checkout quote calculation.

## Package identity

- Package ID: `ISOCodex.Currency`
- Version: `0.1.0-alpha.5`
- Root namespace: `ISOCodex.Currency`
- Target framework: `netstandard2.1`
- Repository: <https://github.com/AnthonyPWatts/ISOCodex.Currency>

Install the current prerelease with:

```bash
dotnet add package ISOCodex.Currency --version 0.1.0-alpha.5
```

## Quick start

```csharp
using ISOCodex.Currency;

var code = CurrencyCode.Parse("gbp");
var currency = DefaultCurrencyRegistry.Instance.Get(code);

Console.WriteLine(currency.EnglishName); // Pound Sterling
Console.WriteLine(currency.MinorUnit.DecimalPlaces); // 2
```

## Currency codes and metadata

`CurrencyCode` accepts registered alpha-3 currency codes and normalises them to uppercase.

```csharp
var gbp = CurrencyCode.Parse("gbp");

if (CurrencyCode.TryParse("JPY", out var jpy))
{
    var metadata = DefaultCurrencyRegistry.Instance.Get(jpy);
    Console.WriteLine(metadata.MinorUnit.DecimalPlaces); // 0
}
```

Invalid, unknown, or malformed codes are rejected:

```csharp
CurrencyCode.TryParse("GB", out _);  // false
CurrencyCode.TryParse("ABC", out _); // false unless registered in the packaged data
```

The registry exposes metadata such as numeric code, English name, minor-unit precision, cash rounding increment, tender status, kind, and associated territories.

## Money value object

`Money` always carries a currency. It validates the amount against that currency's standard minor-unit precision.

```csharp
var price = Money.Of(12.34m, CurrencyCode.GBP);
var shipping = Money.Of(3.49m, CurrencyCode.GBP);
var total = price + shipping;
```

Because `Money` and `CurrencyCode` are value types, `default(Money)` and `default(CurrencyCode)` can still exist in arrays, serializers, or incorrectly initialised DTOs. Use `IsDefault` to detect those values at boundaries. Operational `Money` methods fail clearly for default money values; construct valid values with `Money.Zero(currency)` or `Money.Of(amount, currency)`.

Precision is currency-specific:

```csharp
Money.Of(10.99m, CurrencyCode.GBP); // valid
Money.Of(10.999m, CurrencyCode.GBP); // throws

Money.Of(100m, CurrencyCode.JPY); // valid
Money.Of(100.01m, CurrencyCode.JPY); // throws

Money.Of(1.234m, CurrencyCode.KWD); // valid
```

Cross-currency arithmetic is blocked:

```csharp
var gbp = Money.Of(10m, CurrencyCode.GBP);
var usd = Money.Of(10m, CurrencyCode.USD);

var invalid = gbp + usd; // throws InvalidOperationException
```

Use minor-unit conversion when a storage or payment boundary needs integer minor units:

```csharp
var money = Money.Of(12.34m, CurrencyCode.GBP);
var minorUnits = money.ToMinorUnits(); // 1234

var roundTrip = Money.FromMinorUnits(1234, CurrencyCode.GBP);
```

## Rounding

Rounding is explicit. Constructors validate precision; they do not silently fix it. Use `CurrencyRoundingService` when incoming data has extra precision and needs a deliberate rounding decision.

```csharp
var registry = DefaultCurrencyRegistry.Instance;
var rounding = new CurrencyRoundingService();
var currency = registry.Get(CurrencyCode.GBP);

var amount = rounding.RoundAmount(
    12.345m,
    currency,
    CurrencyRoundingPolicy.AwayFromZero());

var money = Money.Of(amount, CurrencyCode.GBP); // GBP 12.35
```

`Money.Multiply`, `Money.Divide`, and `Money.Round` also require a `CurrencyRoundingPolicy`.

```csharp
var tax = Money.Of(19.99m, CurrencyCode.GBP)
    .Multiply(0.2m, CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven));
```

Supported policies:

- `CurrencyRoundingPolicy.Standard(...)` uses the currency's standard minor-unit precision.
- `CurrencyRoundingPolicy.AwayFromZero()` is a standard precision shortcut using away-from-zero midpoint handling.
- `CurrencyRoundingPolicy.Cash(...)` uses cash rounding metadata, such as CHF `0.05`.
- `CurrencyRoundingPolicy.Custom(decimalPlaces, midpoint)` rounds to a caller-supplied number of decimal places.
- `CurrencyRoundingPolicy.CustomIncrement(increment, midpoint)` rounds to an explicit increment.

Cash rounding example:

```csharp
var cashTotal = Money.Of(1.03m, CurrencyCode.CHF)
    .Round(CurrencyRoundingPolicy.Cash()); // CHF 1.05
```

## Allocation and installments

`MoneyAllocator` splits a valid `Money` value into exact minor-unit parts. The allocation always preserves the original total.

```csharp
var allocation = Money.Of(10.00m, CurrencyCode.GBP)
    .Allocate(3, AllocationRemainderStrategy.Last);

// GBP 3.33, GBP 3.33, GBP 3.34
```

Remainder placement is explicit:

- `AllocationRemainderStrategy.First`
- `AllocationRemainderStrategy.Last`
- `AllocationRemainderStrategy.Spread`

Installment strategies are built on top of `Money` and allocation:

```csharp
var strategy = new FixedFirstInstallmentStrategy(
    Money.Of(4.00m, CurrencyCode.GBP));

var plan = strategy.CalculateInstallments(
    new InstallmentRequest(Money.Of(10.01m, CurrencyCode.GBP), 3));

// GBP 4.00, GBP 3.01, GBP 3.00
```

Built-in strategies include even split, fixed first installment, and whole-major-unit first installment. They do not hard-code currency symbols.

## Formatting and parsing

`MoneyFormatter` formats values with explicit currency display choices.

```csharp
var formatter = new MoneyFormatter();
var price = Money.Of(12.34m, CurrencyCode.GBP);

formatter.Format(price); // GBP 12.34

formatter.Format(
    price,
    new MoneyFormatOptions(
        new CultureInfo("en-GB"),
        MoneyCurrencyDisplay.Symbol)); // £12.34
```

Supported display modes are:

- `MoneyCurrencyDisplay.Code`
- `MoneyCurrencyDisplay.Symbol`
- `MoneyCurrencyDisplay.CodeAndSymbol`
- `MoneyCurrencyDisplay.None`

Formatting uses culture-specific separators and currency patterns, while currency decimal places come from the registry metadata by default.

`MoneyParser` is deliberately conservative. By default, it parses alpha-3 currency codes rather than guessing from symbols.

```csharp
var parser = new MoneyParser();
var result = parser.Parse("GBP 12.34", new MoneyParseOptions(CultureInfo.InvariantCulture));

if (result.Succeeded)
{
    var money = result.Money.Value;
}
```

Symbol parsing requires an expected currency:

```csharp
var result = parser.Parse(
    "£12.34",
    new MoneyParseOptions(
        new CultureInfo("en-GB"),
        CurrencyCode.GBP,
        MoneyParseCurrencyStyles.CodeOrSymbol));
```

Failed parses return `MoneyParseResult` with a `MoneyParseFailureReason`; they do not throw for ordinary invalid input.

## System.Text.Json

JSON support lives in the optional `ISOCodex.Currency.Json.SystemTextJson` package so the core package remains independent of `System.Text.Json`.

```bash
dotnet add package ISOCodex.Currency.Json.SystemTextJson --version 0.1.0-alpha.5
```

Register the converters explicitly:

```csharp
using System.Text.Json;
using ISOCodex.Currency.Json.SystemTextJson;

var options = new JsonSerializerOptions();
options.Converters.Add(new CurrencyCodeJsonConverter());
options.Converters.Add(new MoneyJsonConverter());
```

`CurrencyCode` serialises as `"GBP"`. `Money` serialises as `{ "amount": 12.34, "currency": "GBP" }`. Deserialisation rejects invalid currency codes and over-precise money amounts; it does not infer from symbols and does not silently round.

## Imports and API boundaries

At application boundaries, prefer primitive DTOs and convert into domain values after validation.

```csharp
public sealed record PriceInput(decimal Amount, string Currency);

MoneyValidationResult ToMoney(PriceInput input)
{
    return Money.TryCreate(input.Amount, input.Currency);
}
```

`MoneyValidationResult` exposes `Succeeded`, `Money`, `FailureReason`, and `ErrorMessage`. Stable failure reasons include `DefaultCurrency`, `UnknownCurrency`, `AmountPrecision`, `MinorUnitNotApplicable`, `Overflow`, and `InvalidAmount`.

Use the strict `Money.Of(...)` and `Money.FromMinorUnits(...)` factories in trusted domain code where invalid input should throw. Use `Money.TryCreate(...)` and `Money.TryFromMinorUnits(...)` for APIs, forms, CSV imports, and partner integrations where ordinary invalid input should become a validation response.

If imports may contain over-precise amounts, either return the `AmountPrecision` validation reason or round the raw amount first using an explicit policy, then construct `Money`.

## Recommended persistence shape

For relational storage, prefer storing amount and currency code separately:

| Column | Suggested type | Notes |
| --- | --- | --- |
| `Amount` | `decimal(19,4)` or wider | Choose scale for the domain. Four decimals covers common ISO tender currencies and CLF/UYW-style units in the current data. |
| `CurrencyCode` | `char(3)` | Store uppercase ISO 4217-style alpha-3 codes. |

For payment-style exact minor-unit storage, use:

| Column | Suggested type | Notes |
| --- | --- | --- |
| `MinorUnits` | `bigint` | Suitable where the currency has applicable minor units and the scale is stable for the record. |
| `CurrencyCode` | `char(3)` | Required to interpret the integer amount. |

Do not store a currency symbol as the domain currency. Symbols are display concerns and can be ambiguous.

## Extended examples

The extended test rigs are deliberately consumer-shaped examples rather than unit tests.

```bash
dotnet run --project ExtendedTestRigs/BulkMoneyImportTool -- SampleData/money-import.csv
dotnet run --project ExtendedTestRigs/CheckoutPricingApi --urls http://localhost:5000
```

See [ExtendedTestRigs/README.md](ExtendedTestRigs/README.md) for details.

## Current limitations

- Currency data is currently generated from a small checked-in seed, not a full ISO/CLDR snapshot.
- Formatting is intended for display, not persistence. Store amount and currency code separately.
- Money parsing is conservative and does not infer a currency from ambiguous symbols without an expected currency.
- JSON converters are available in the optional `ISOCodex.Currency.Json.SystemTextJson` package.
- There are no Entity Framework Core helpers yet.
- There are no exchange-rate abstractions yet.

## Non-goals

- This package does not provide live exchange rates.
- This package does not make financial, tax, accounting, or regulatory decisions.
- This package does not infer a user's business currency from locale.
- This package does not provide arbitrary-precision crypto-asset modelling as a first-class goal.
- This package does not guarantee that a currency is currently legal tender unless the metadata says so and the data snapshot is current for that release.

## Verification

From the repository root:

These checks require a .NET 9 SDK/runtime because the test project and smoke consumer target `net9.0`.
If a local machine has a newer compatible runtime but not the .NET 9 runtime, use `pwsh ./eng/smoke-test-package.ps1 -Version 0.1.0-alpha.5 -UseMajorRollForward` for the smoke test. This is a local workaround; CI installs .NET 9 explicitly.

```bash
dotnet restore ISOCodex.Currency.sln
dotnet build ISOCodex.Currency.sln -c Release --no-restore
dotnet test ISOCodex.Currency.sln -c Release --no-build
pwsh ./eng/pack-packages.ps1 -Configuration Release -OutputPath artifacts
pwsh ./eng/smoke-test-package.ps1 -Version 0.1.0-alpha.5
```

## Currency data workflow

The current pre-1.0 registry is generated from `data/source/currency-data.seed.json`:

```powershell
pwsh ./scripts/update-currency-data.ps1
dotnet test ISOCodex.Currency.sln --filter CurrencyData
```

The seed is deliberately small. A later data epic should replace it with pinned SIX ISO 4217 and Unicode CLDR source files.

Runtime code can log the packaged data provenance:

```csharp
Console.WriteLine(CurrencyDataVersion.Identifier);  // seed-0.1.0-alpha.4
Console.WriteLine(CurrencyDataVersion.SourceKind);  // CheckedInSeed
Console.WriteLine(CurrencyDataVersion.CheckedOn);   // 2026-06-22 UTC midnight
Console.WriteLine(CurrencyDataVersion.Description); // small checked-in prerelease seed; not a full ISO/CLDR snapshot
```
