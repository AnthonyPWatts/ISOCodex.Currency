# ISOCodex.Currency.Analyzers

Roslyn analyzers for `ISOCodex.Currency`.

## Install

```xml
<PackageReference Include="ISOCodex.Currency.Analyzers" Version="0.9.0-alpha.10" PrivateAssets="all" />
```

## Diagnostics

### ISOCCUR001

Avoid `default(Money)`. Use `Money.Zero(currency)` or `Money.Of(amount, currency)`.

```csharp
Money value = default(Money); // ISOCCUR001
Money other = default;        // ISOCCUR001 when converted to Money
```

This initial analyzer package does not include code fixes. Future analyzer rules may cover `default(CurrencyCode)` and ignored validation results.
