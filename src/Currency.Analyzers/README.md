# ISOCodex.Currency.Analyzers

Roslyn analyzers for `ISOCodex.Currency`.

## Install

```xml
<PackageReference Include="ISOCodex.Currency.Analyzers" Version="0.9.0-alpha.13" PrivateAssets="all" />
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

This analyzer package does not include code fixes. Future analyzer rules may cover ignored validation results.
