using System;

namespace ISOCodex.Currency.Exchange.Abstractions;

/// <summary>
/// Represents a direct exchange rate for a currency pair.
/// </summary>
public sealed class ExchangeRate
{
    /// <summary>
    /// Creates an exchange rate. The rate is the quote-currency amount for one unit of the base currency.
    /// </summary>
    public ExchangeRate(
        CurrencyPair pair,
        decimal rate,
        ExchangeRateKind kind,
        DateTime effectiveDate,
        string source)
    {
        if (pair.BaseCurrency.IsDefault || pair.QuoteCurrency.IsDefault)
        {
            throw new ArgumentException("Exchange-rate pair must contain initialised currencies.", nameof(pair));
        }

        if (rate <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(rate), "Exchange rate must be greater than zero.");
        }

        if (kind == ExchangeRateKind.Unspecified)
        {
            throw new ArgumentException("Exchange rate kind must be specified.", nameof(kind));
        }

        if (effectiveDate == default)
        {
            throw new ArgumentException("Effective date must be specified.", nameof(effectiveDate));
        }

        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentException("Exchange rate source must be specified.", nameof(source));
        }

        Pair = pair;
        Rate = rate;
        Kind = kind;
        EffectiveDate = effectiveDate;
        Source = source.Trim();
    }

    /// <summary>Gets the direct exchange-rate pair.</summary>
    public CurrencyPair Pair { get; }

    /// <summary>Gets the quote-currency amount for one unit of the base currency.</summary>
    public decimal Rate { get; }

    /// <summary>Gets the business meaning of the rate.</summary>
    public ExchangeRateKind Kind { get; }

    /// <summary>Gets the date or instant for which the rate applies.</summary>
    public DateTime EffectiveDate { get; }

    /// <summary>Gets the provider, feed, file, or policy source for audit purposes.</summary>
    public string Source { get; }
}
