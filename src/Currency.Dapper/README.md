# ISOCodex.Currency.Dapper

Dapper type handlers for `ISOCodex.Currency`.

## Install

```bash
dotnet add package ISOCodex.Currency.Dapper --version 0.9.0-alpha.12
```

This package targets `netstandard2.1` and depends on Dapper.

## Currency Codes

Register the handlers once during application startup.

```csharp
using ISOCodex.Currency.Dapper;

DapperCurrencyTypeHandlers.Register();
```

The `CurrencyCodeTypeHandler` maps registered `CurrencyCode` values to uppercase alpha-3 database strings.

```csharp
using Dapper;
using ISOCodex.Currency;

var price = await connection.QuerySingleAsync<PriceRow>(
    "select amount, currency_code as Currency from prices where id = @id",
    new { id });

var money = Money.Of(price.Amount, price.Currency);

public sealed record PriceRow(decimal Amount, CurrencyCode Currency);
```

Persist `Money` values as separate amount and currency-code columns. A single formatted text column is intentionally not provided by this package because it is harder to query, index, and validate.
