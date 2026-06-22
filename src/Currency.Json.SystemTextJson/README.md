# ISOCodex.Currency.Json.SystemTextJson

System.Text.Json converters for `ISOCodex.Currency`.

## Install

```bash
dotnet add package ISOCodex.Currency.Json.SystemTextJson --version 0.9.0-alpha.5
```

## Register converters

```csharp
using System.Text.Json;
using ISOCodex.Currency.Json.SystemTextJson;

var options = new JsonSerializerOptions();
options.Converters.Add(new CurrencyCodeJsonConverter());
options.Converters.Add(new MoneyJsonConverter());
```

`CurrencyCode` serialises as an alpha-3 string:

```json
"GBP"
```

`Money` serialises as an amount/currency object:

```json
{ "amount": 12.34, "currency": "GBP" }
```

Deserialisation validates currency codes and money precision. Invalid currency codes, over-precise amounts, missing properties, and default values fail with `JsonException`; the converters do not infer from symbols and do not silently round.
