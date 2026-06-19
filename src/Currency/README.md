# ISOCodex.Currency

Framework-agnostic ISO 4217-style currency metadata and immutable money values for .NET.

```csharp
var code = CurrencyCode.Parse("usd");
var currency = DefaultCurrencyRegistry.Instance.Get(code);
```

This pre-1.0 package currently includes currency codes, metadata, the default registry, and the `Money` value object. Rounding, formatting, validation, JSON, and persistence helpers are planned follow-up features.
