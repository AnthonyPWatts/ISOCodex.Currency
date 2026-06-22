using System;

namespace ISOCodex.Currency.Exchange.Abstractions;

/// <summary>
/// Provides deterministic exchange-rate lookup. Implementations decide where rates come from.
/// </summary>
public interface IExchangeRateProvider
{
    /// <summary>
    /// Gets a direct exchange rate for the requested pair, effective date, and rate kind.
    /// </summary>
    ExchangeRateLookupResult GetRate(
        CurrencyPair pair,
        DateTime effectiveDate,
        ExchangeRateKind rateKind);
}
