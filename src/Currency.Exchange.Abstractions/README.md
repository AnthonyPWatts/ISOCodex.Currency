# ISOCodex.Currency.Exchange.Abstractions

Provider-neutral deterministic exchange-rate abstractions for `ISOCodex.Currency`.

## Install

```bash
dotnet add package ISOCodex.Currency.Exchange.Abstractions --version 1.0.0
```

## Scope

This package defines the exchange-rate contracts and a direct-rate `MoneyConverter`. It deliberately does not include a live exchange-rate provider and does not make network calls. Applications supply rates from their own approved source, such as an audited database, file import, internal policy table, or a separately reviewed provider package.

The initial converter supports direct rates only. Inverse, triangulated, cached, and live-provider behaviours are future package work.

## Example

```csharp
using ISOCodex.Currency;
using ISOCodex.Currency.Exchange.Abstractions;

var effectiveDate = new DateTime(2026, 6, 22, 0, 0, 0, DateTimeKind.Utc);
var rate = new ExchangeRate(
    new CurrencyPair(CurrencyCode.GBP, CurrencyCode.USD),
    1.2345m,
    ExchangeRateKind.MidMarket,
    effectiveDate,
    "finance-approved-rates.csv");

IExchangeRateProvider provider = new InMemoryExchangeRateProvider(rate);
var converter = new MoneyConverter(provider);

var result = converter.Convert(
    Money.Of(10.00m, CurrencyCode.GBP),
    new ConversionOptions(
        CurrencyCode.USD,
        effectiveDate,
        ExchangeRateKind.MidMarket,
        CurrencyRoundingPolicy.AwayFromZero()));

Console.WriteLine(result.RawAmount);     // 12.345000
Console.WriteLine(result.RoundedAmount); // 12.35
Console.WriteLine(result.Output);        // USD 12.35
Console.WriteLine(result.RateSource);    // finance-approved-rates.csv
```

A minimal provider can be implemented in application code:

```csharp
public sealed class InMemoryExchangeRateProvider : IExchangeRateProvider
{
    private readonly ExchangeRate _rate;

    public InMemoryExchangeRateProvider(ExchangeRate rate)
    {
        _rate = rate;
    }

    public ExchangeRateLookupResult GetRate(
        CurrencyPair pair,
        DateTime effectiveDate,
        ExchangeRateKind rateKind)
    {
        if (_rate.Pair == pair && _rate.EffectiveDate == effectiveDate && _rate.Kind == rateKind)
        {
            return ExchangeRateLookupResult.Success(_rate);
        }

        return ExchangeRateLookupResult.Failure(
            ExchangeRateLookupFailureReason.RateUnavailable,
            $"No rate for {pair} on {effectiveDate:O}.");
    }
}
```

## Audit behaviour

`ConversionResult` includes:

- input money;
- output money;
- direct rate used;
- raw unrounded target amount;
- rounded target amount;
- requested effective date;
- requested rate kind;
- explicit rounding policy;
- rate source.

This is enough for deterministic replay when the application preserves the same input, rate, effective date, rate kind, and rounding policy.
