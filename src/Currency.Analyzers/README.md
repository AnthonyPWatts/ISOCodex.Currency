# ISOCodex.Currency.Analyzers

Roslyn analyzers for `ISOCodex.Currency`.

## Install

```xml
<PackageReference Include="ISOCodex.Currency.Analyzers" Version="0.9.0-alpha.15" PrivateAssets="all" />
```

## Diagnostics

### ISOCCUR001

Avoid `default(Money)`. Use `Money.Zero(currency)` or `Money.Of(amount, currency)`.

```csharp
Money value = default(Money); // ISOCCUR001
Money other = default;        // ISOCCUR001 when converted to Money
```

### ISOCCUR002

Avoid `default(CurrencyCode)`. Use `CurrencyCode.Parse(...)`, `CurrencyCode.TryParse(...)`, or a known static code such as `CurrencyCode.GBP`.

```csharp
CurrencyCode value = default(CurrencyCode); // ISOCCUR002
CurrencyCode other = default;               // ISOCCUR002 when converted to CurrencyCode
```

### ISOCCUR003

Do not ignore `MoneyValidationResult` or `MoneyParseResult`. Store the result and check `Succeeded` plus the stable failure reason before continuing.

```csharp
Money.TryCreate(12.345m, CurrencyCode.GBP);        // ISOCCUR003
new MoneyParser().Parse("GBP 12.345");             // ISOCCUR003

var result = Money.TryCreate(12.345m, CurrencyCode.GBP);
if (!result.Succeeded)
{
    Console.WriteLine(result.FailureReason);
}
```

### ISOCCUR005

Do not enable symbol parsing without an expected currency. Currency symbols are ambiguous across cultures and territories, so symbol parsing should be anchored to the currency the caller already expects.

```csharp
new MoneyParseOptions(
    new CultureInfo("en-GB"),
    currencyStyles: MoneyParseCurrencyStyles.CodeOrSymbol); // ISOCCUR005

new MoneyParseOptions(
    new CultureInfo("en-GB"),
    CurrencyCode.GBP,
    MoneyParseCurrencyStyles.CodeOrSymbol);                 // ok
```

This analyzer package does not include code fixes.
