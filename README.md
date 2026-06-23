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

This package is pre-1.0. The implemented API is useful, optional integrations are split into separate packages, and EF Core persistence helpers live in an optional package.

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
- `MoneyFactory`
- `CurrencyRoundingPolicy`
- `CurrencyRoundingService`
- `MoneyAllocator`
- `InstallmentPlan`
- built-in Money-based installment strategies
- `MoneyFormatter`
- `MoneyParser`
- optional `ISOCodex.Currency.Analyzers` package
- optional `ISOCodex.Currency.Addressing` bridge package
- optional `ISOCodex.Currency.AspNetCore` package
- optional `ISOCodex.Currency.Json.NewtonsoftJson` converters
- optional `ISOCodex.Currency.Json.SystemTextJson` converters
- optional `ISOCodex.Currency.Countries` bridge package
- optional `ISOCodex.Currency.Dapper` package
- optional `ISOCodex.Currency.EntityFrameworkCore` package
- optional `ISOCodex.Currency.Exchange.Abstractions` package
- optional `ISOCodex.Currency.Validation` package

## Projects

- `src/Currency` - core package.
- `src/Currency.Analyzers` - optional Roslyn analyzer package.
- `src/Currency.Addressing` - optional Addressing bridge package.
- `src/Currency.AspNetCore` - optional ASP.NET Core integration package.
- `src/Currency.Countries` - optional Countries bridge package.
- `src/Currency.Dapper` - optional Dapper integration package.
- `src/Currency.EntityFrameworkCore` - optional Entity Framework Core integration package.
- `src/Currency.Exchange.Abstractions` - optional deterministic exchange-rate abstractions package.
- `src/Currency.Json.NewtonsoftJson` - optional Newtonsoft.Json integration package.
- `src/Currency.Json.SystemTextJson` - optional System.Text.Json integration package.
- `src/Currency.Validation` - optional framework-neutral validation helpers package.
- `tests/Currency.Tests` - xUnit test suite.
- `tests/Currency.Analyzers.Tests` - analyzer xUnit test suite.
- `tests/Currency.Addressing.Tests` - Addressing bridge xUnit test suite.
- `tests/Currency.AspNetCore.Tests` - ASP.NET Core integration xUnit test suite.
- `tests/Currency.Countries.Tests` - Countries bridge xUnit test suite.
- `tests/Currency.Dapper.Tests` - Dapper integration xUnit test suite.
- `tests/Currency.EntityFrameworkCore.Tests` - Entity Framework Core integration xUnit test suite.
- `tests/Currency.Exchange.Abstractions.Tests` - exchange abstractions xUnit test suite.
- `tests/Currency.Json.NewtonsoftJson.Tests` - Newtonsoft.Json converter xUnit test suite.
- `tests/Currency.Json.SystemTextJson.Tests` - System.Text.Json converter xUnit test suite.
- `tests/Currency.Validation.Tests` - validation helpers xUnit test suite.
- `ManualTestRig` - small manual console rig for currency metadata.
- `ExtendedTestRigs/BulkMoneyImportTool` - CSV import example for mixed-currency money data.
- `ExtendedTestRigs/CheckoutPricingApi` - Minimal API example for checkout quote calculation.

## Package identity

- Package ID: `ISOCodex.Currency`
- Version: `0.9.0-alpha.13`
- Root namespace: `ISOCodex.Currency`
- Target framework: `netstandard2.1`
- Repository: <https://github.com/AnthonyPWatts/ISOCodex.Currency>

Install the current prerelease with:

```bash
dotnet add package ISOCodex.Currency --version 0.9.0-alpha.13
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

Midpoint policy is a business decision. The same raw tax calculation can produce different valid money values depending on the selected policy:

```csharp
var taxableAmount = Money.Of(10.05m, CurrencyCode.GBP);

var toEvenTax = taxableAmount
    .Multiply(0.10m, CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven)); // GBP 1.00

var awayFromZeroTax = taxableAmount
    .Multiply(0.10m, CurrencyRoundingPolicy.AwayFromZero()); // GBP 1.01

var discount = taxableAmount
    .Multiply(0.10m, CurrencyRoundingPolicy.AwayFromZero()); // GBP 1.01

var discountedPrice = taxableAmount - discount; // GBP 9.04
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

var roundedToQuarter = Money.Of(1.38m, CurrencyCode.GBP)
    .Round(CurrencyRoundingPolicy.CustomIncrement(0.25m, MidpointRounding.AwayFromZero)); // GBP 1.50
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

For example, splitting `GBP 10.05` into six parts produces the same total but places the three extra pennies differently:

```text
First:  GBP 1.68, GBP 1.68, GBP 1.68, GBP 1.67, GBP 1.67, GBP 1.67
Last:   GBP 1.67, GBP 1.67, GBP 1.67, GBP 1.68, GBP 1.68, GBP 1.68
Spread: GBP 1.68, GBP 1.67, GBP 1.68, GBP 1.67, GBP 1.68, GBP 1.67
```

Installment strategies are built on top of `Money` and allocation:

```csharp
var strategy = new FixedFirstInstallmentStrategy(
    Money.Of(4.00m, CurrencyCode.GBP));

var plan = strategy.CalculateInstallments(
    new InstallmentRequest(Money.Of(10.01m, CurrencyCode.GBP), 3));

// GBP 4.00, GBP 3.01, GBP 3.00
```

Built-in strategies include even split, fixed first installment, and whole-major-unit first installment. Remainder placement is explicit for strategies that allocate the remaining total, and they do not hard-code currency symbols.

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

## JSON

JSON support lives in optional packages so the core package remains independent of serializer dependencies.

### System.Text.Json

```bash
dotnet add package ISOCodex.Currency.Json.SystemTextJson --version 0.9.0-alpha.13
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

### Newtonsoft.Json

```bash
dotnet add package ISOCodex.Currency.Json.NewtonsoftJson --version 0.9.0-alpha.13
```

Register the converters explicitly:

```csharp
using ISOCodex.Currency.Json.NewtonsoftJson;
using Newtonsoft.Json;

var settings = new JsonSerializerSettings();
settings.Converters.Add(new CurrencyCodeJsonConverter());
settings.Converters.Add(new MoneyJsonConverter());
```

The Newtonsoft.Json package uses the same default wire shape and validation semantics as the System.Text.Json package.

## Countries Bridge

Country/currency validation lives in the optional `ISOCodex.Currency.Countries` package. The core package does not depend on `ISOCodex.Countries`.

```bash
dotnet add package ISOCodex.Currency.Countries --version 0.9.0-alpha.13
```

The initial bridge seed is deliberately small:

```csharp
using ISOCodex.Currency;
using ISOCodex.Currency.Countries;
using ISOCodex.Countries;

var result = DefaultCountryCurrencyRegistry.Instance.Validate(
    CountryAlpha2Code.Parse("GB"),
    CurrencyCode.GBP,
    CountryCurrencyValidationPolicy.PrimaryTenderOnly);
```

The seed currently covers GB/GBP, US/USD, IE/EUR, JP/JPY, CH/CHF, CA/CAD, AU/AUD, and NZ/NZD. It is not a complete legal-tender dataset or geopolitical authority.

## Addressing Bridge

Address/currency validation lives in the optional `ISOCodex.Currency.Addressing` package. It delegates postal address validation to `ISOCodex.Addressing` and country/currency validation to `ISOCodex.Currency.Countries`.

```bash
dotnet add package ISOCodex.Currency.Addressing --version 0.9.0-alpha.13
```

Use it when a workflow needs one structured result for both the address and the selected currency:

```csharp
using ISOCodex.Currency.Addressing;

var validator = new AddressCurrencyValidator(addressValidatorFactory);
var result = validator.Validate(
    billingAddress,
    CurrencyCode.GBP,
    AddressCurrencyValidationPolicy.CheckoutDefault);
```

The bridge does not calculate tax, shipping, payment acceptance, fraud risk, or deliverability.

## Exchange Abstractions

Provider-neutral exchange contracts live in the optional `ISOCodex.Currency.Exchange.Abstractions` package. The core package does not include live rates and does not make network calls.

```bash
dotnet add package ISOCodex.Currency.Exchange.Abstractions --version 0.9.0-alpha.13
```

The initial converter supports direct rates only and requires an explicit rounding policy:

```csharp
using ISOCodex.Currency;
using ISOCodex.Currency.Exchange.Abstractions;

var effectiveDate = new DateTime(2026, 6, 22, 0, 0, 0, DateTimeKind.Utc);
var converter = new MoneyConverter(provider);

var result = converter.Convert(
    Money.Of(10.00m, CurrencyCode.GBP),
    new ConversionOptions(
        CurrencyCode.USD,
        effectiveDate,
        ExchangeRateKind.MidMarket,
        CurrencyRoundingPolicy.AwayFromZero()));
```

Applications provide their own `IExchangeRateProvider`. `ConversionResult` exposes the input, output, direct rate, raw amount, rounded amount, requested effective date, requested rate kind, rounding policy, and rate source for replay and audit.

## Entity Framework Core

Entity Framework Core integration lives in the optional `ISOCodex.Currency.EntityFrameworkCore` package. The package targets `net10.0` and references `Microsoft.EntityFrameworkCore.Relational`.

```bash
dotnet add package ISOCodex.Currency.EntityFrameworkCore --version 0.9.0-alpha.13
```

Use `HasCurrencyCodeConversion()` when an entity stores a `CurrencyCode` directly:

```csharp
using ISOCodex.Currency.EntityFrameworkCore;

builder.Property(order => order.Currency)
    .HasCurrencyCodeConversion()
    .HasColumnName("CurrencyCode");
```

Use `ComplexMoney(...)` when an entity stores a `Money` value as separate amount and currency-code columns:

```csharp
builder.ComplexMoney(
    order => order.Total,
    amountColumn: "TotalAmount",
    currencyColumn: "TotalCurrency");
```

The default amount column type is `decimal(19,4)`. For exact payment-style minor-unit storage, model the storage shape explicitly as `long MinorUnits` plus `CurrencyCode CurrencyCode`, and convert at the application boundary with `Money.FromMinorUnits(...)` or `Money.TryFromMinorUnits(...)`.

## Dapper

Dapper integration lives in the optional `ISOCodex.Currency.Dapper` package.

```bash
dotnet add package ISOCodex.Currency.Dapper --version 0.9.0-alpha.13
```

Register the type handlers once during application startup:

```csharp
using ISOCodex.Currency.Dapper;

DapperCurrencyTypeHandlers.Register();
```

The `CurrencyCodeTypeHandler` maps `CurrencyCode` values to uppercase alpha-3 database strings. For `Money`, keep amount and currency-code columns separate and construct the value explicitly after query materialisation.

## ASP.NET Core

ASP.NET Core integration lives in the optional `ISOCodex.Currency.AspNetCore` package.

```bash
dotnet add package ISOCodex.Currency.AspNetCore --version 0.9.0-alpha.13
```

Register the default services and MVC model binder:

```csharp
using ISOCodex.Currency.AspNetCore;

builder.Services.AddCurrencyAspNetCore();
```

The package registers `ICurrencyRegistry`, `IMoneyFormatter`, `MoneyParser`, and MVC model binding for `CurrencyCode`. Invalid `CurrencyCode` route, query, or form values add model-state errors instead of throwing.

Minimal APIs can bind `CurrencyCode` directly through the core `TryParse(...)` method:

```csharp
app.MapGet("/currencies/{currency}", (CurrencyCode currency) =>
{
    return Results.Ok(currency.Code);
});
```

For `Money`, keep primitive request DTOs at the HTTP boundary and convert explicitly:

```csharp
var result = Money.TryCreate(request.Amount, request.Currency);

return result.Succeeded
    ? Results.Ok(result.Money)
    : Results.ValidationProblem(result.ToValidationProblemDictionary("amount"));
```

## Validation

Framework-neutral validation helpers live in the optional `ISOCodex.Currency.Validation` package.

```bash
dotnet add package ISOCodex.Currency.Validation --version 0.9.0-alpha.13
```

Use `CurrencyBoundaryValidator` for primitive API, import, and integration inputs when the output should be a stable issue list rather than an exception or framework-specific model state.

```csharp
using ISOCodex.Currency.Validation;

var validator = new CurrencyBoundaryValidator();
var validation = validator.ValidateMoney(12.345m, "GBP", "amount", "currency");

if (!validation.IsValid)
{
    var errors = validation.ToErrorDictionary();
}
```

Each `CurrencyValidationIssue` has a stable `Code`, a human-readable `Message`, and an optional `PropertyName`. Existing `MoneyValidationResult` and `MoneyParseResult` values can be adapted with `ToCurrencyValidationResult(...)`.

## Analyzers

Analyzer support lives in the optional `ISOCodex.Currency.Analyzers` package.

```xml
<PackageReference Include="ISOCodex.Currency.Analyzers" Version="0.9.0-alpha.13" PrivateAssets="all" />
```

The initial rules are `ISOCCUR001`, which warns on `default(Money)` and `default` literals converted to `Money`, and `ISOCCUR002`, which warns on `default(CurrencyCode)` and `default` literals converted to `CurrencyCode`. Use `Money.Zero(currency)`, `Money.Of(amount, currency)`, `CurrencyCode.Parse(...)`, or a known static currency code instead.

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

## Advanced Registries

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

`CurrencyCode.Parse(...)` and `CurrencyCode.TryParse(...)` remain strict and only accept codes from the packaged registry. Use `CurrencyCode.CreateCustom(...)` only with an explicit custom registry.

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

- Currency data is generated from pinned checked-in SIX ISO 4217 and Unicode CLDR source files. It is a derived metadata snapshot, not an official ISO 4217 redistribution.
- Formatting is intended for display, not persistence. Store amount and currency code separately.
- Money parsing is conservative and does not infer a currency from ambiguous symbols without an expected currency.
- JSON converters are available in the optional `ISOCodex.Currency.Json.SystemTextJson` and `ISOCodex.Currency.Json.NewtonsoftJson` packages.
- Addressing bridge helpers are available in the optional `ISOCodex.Currency.Addressing` package.
- ASP.NET Core helpers are available in the optional `ISOCodex.Currency.AspNetCore` package.
- Dapper handlers are available in the optional `ISOCodex.Currency.Dapper` package.
- Framework-neutral validation helpers are available in the optional `ISOCodex.Currency.Validation` package.
- Exchange abstractions are available in the optional `ISOCodex.Currency.Exchange.Abstractions` package, but no live provider is included.
- Entity Framework Core helpers target EF Core 10+ in the optional `ISOCodex.Currency.EntityFrameworkCore` package.

## Non-goals

- This package does not provide live exchange rates.
- This package does not make financial, tax, accounting, or regulatory decisions.
- This package does not infer a user's business currency from locale.
- This package does not provide arbitrary-precision crypto-asset modelling as a first-class goal.
- This package does not guarantee that a currency is currently legal tender unless the metadata says so and the data snapshot is current for that release.

## Verification

From the repository root:

These checks require .NET 9 and .NET 10 SDK/runtime support because the main package/test surface targets `net9.0` consumers and the EF Core integration targets `net10.0`.
If a local machine has newer compatible runtimes but not the .NET 9 runtime, use `pwsh ./eng/smoke-test-package.ps1 -Version 0.9.0-alpha.13 -UseMajorRollForward` for the smoke test. This is a local workaround; CI installs .NET 9 and .NET 10 explicitly.

```bash
dotnet restore ISOCodex.Currency.sln
dotnet build ISOCodex.Currency.sln -c Release --no-restore
dotnet test ISOCodex.Currency.sln -c Release --no-build
pwsh ./eng/pack-packages.ps1 -Configuration Release -OutputPath artifacts -Version 0.9.0-alpha.13
pwsh ./eng/smoke-test-package.ps1 -Version 0.9.0-alpha.13
```

## Currency data workflow

The current pre-1.0 registry is generated from a derived JSON snapshot built from checked-in SIX ISO 4217 and Unicode CLDR source files, pinned by `data/source/currency-data.manifest.json`:

```powershell
pwsh ./scripts/build-currency-data-snapshot.ps1
pwsh ./scripts/update-currency-data.ps1
dotnet test ISOCodex.Currency.sln --filter CurrencyData
```

The manifest records normalized UTF-8/LF SHA-256 values for the derived snapshot and raw upstream source files, the derived entry count, checked date, and runtime provenance values. The snapshot currently contains 178 current SIX List One currency/fund codes and CLDR cash-fraction/territory metadata where available.

Runtime code can log the packaged data provenance:

```csharp
Console.WriteLine(CurrencyDataVersion.Identifier);  // iso4217-cldr-2026-06-22-c1f3aaea
Console.WriteLine(CurrencyDataVersion.SourceKind);  // SIX-ISO4217+Unicode-CLDR
Console.WriteLine(CurrencyDataVersion.CheckedOn);   // 2026-06-22 UTC midnight
Console.WriteLine(CurrencyDataVersion.Description); // pinned SIX/CLDR snapshot; not an official ISO redistribution
```
