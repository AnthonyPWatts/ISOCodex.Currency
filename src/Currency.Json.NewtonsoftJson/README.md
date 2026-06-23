# ISOCodex.Currency.Json.NewtonsoftJson

Newtonsoft.Json converters for `ISOCodex.Currency` value objects.

## Install

```bash
dotnet add package ISOCodex.Currency.Json.NewtonsoftJson --version 0.9.0-alpha.12
```

## Usage

```csharp
using ISOCodex.Currency.Json.NewtonsoftJson;
using Newtonsoft.Json;

var settings = new JsonSerializerSettings();
settings.Converters.Add(new CurrencyCodeJsonConverter());
settings.Converters.Add(new MoneyJsonConverter());
```

`CurrencyCode` serialises as an alpha-3 string:

```json
"GBP"
```

`Money` serialises as an amount/currency object:

```json
{
  "amount": 12.34,
  "currency": "GBP"
}
```

Deserialisation rejects invalid currency codes and over-precise money amounts. It does not infer from symbols and does not silently round.
