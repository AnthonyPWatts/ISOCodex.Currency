using System;

namespace ISOCodex.Currency.Exchange.Abstractions;

/// <summary>
/// Identifies a direct exchange-rate pair from a base currency to a quote currency.
/// </summary>
public readonly record struct CurrencyPair
{
    /// <summary>
    /// Creates a currency pair.
    /// </summary>
    public CurrencyPair(CurrencyCode baseCurrency, CurrencyCode quoteCurrency)
    {
        if (baseCurrency.IsDefault)
        {
            throw new ArgumentException("Base currency must be initialised.", nameof(baseCurrency));
        }

        if (quoteCurrency.IsDefault)
        {
            throw new ArgumentException("Quote currency must be initialised.", nameof(quoteCurrency));
        }

        BaseCurrency = baseCurrency;
        QuoteCurrency = quoteCurrency;
    }

    /// <summary>Gets the currency being converted from.</summary>
    public CurrencyCode BaseCurrency { get; }

    /// <summary>Gets the currency being converted to.</summary>
    public CurrencyCode QuoteCurrency { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return BaseCurrency + "/" + QuoteCurrency;
    }
}
