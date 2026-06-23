# ISOCodex.Currency.EntityFrameworkCore

Entity Framework Core integration helpers for `ISOCodex.Currency`.

## Install

```bash
dotnet add package ISOCodex.Currency.EntityFrameworkCore --version 0.9.0-alpha.12
```

This package targets `net10.0` and references `Microsoft.EntityFrameworkCore.Relational`.

## Currency Codes

Use `HasCurrencyCodeConversion()` to store `CurrencyCode` as an uppercase, non-Unicode, fixed-length alpha-3 code.

```csharp
builder.Property(order => order.Currency)
    .HasCurrencyCodeConversion()
    .HasColumnName("CurrencyCode");
```

## Money

Use `ComplexMoney(...)` to store `Money` as separate amount and currency-code columns.

```csharp
builder.ComplexMoney(
    order => order.Total,
    amountColumn: "TotalAmount",
    currencyColumn: "TotalCurrency");
```

The default amount column type is `decimal(19,4)`. Override it when the domain needs a different precision or scale.

This package deliberately does not store formatted money strings or currency symbols. Store amount and currency code separately so records remain queryable and auditable.

## Minor Units

Exact minor-unit persistence should be modelled explicitly in the application, for example as `long MinorUnits` plus `CurrencyCode CurrencyCode`. EF Core value converters cannot map one `Money` value into two primitive provider columns by themselves.
