# ISOCodex.Currency

Framework-agnostic ISO 4217-style currency metadata, immutable money values, and explicit currency rounding for .NET.

```csharp
var code = CurrencyCode.Parse("usd");
var currency = DefaultCurrencyRegistry.Instance.Get(code);
```

This pre-1.0 package currently includes currency codes, metadata, the default registry, the `Money` value object, and explicit standard/cash/custom rounding policies. Formatting, validation, JSON, and persistence helpers are planned follow-up features.
