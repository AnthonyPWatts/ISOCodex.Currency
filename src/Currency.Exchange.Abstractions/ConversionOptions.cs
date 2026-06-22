using System;

namespace ISOCodex.Currency.Exchange.Abstractions;

/// <summary>
/// Options required to convert money using an explicit exchange rate and rounding policy.
/// </summary>
public sealed class ConversionOptions
{
    /// <summary>
    /// Creates conversion options.
    /// </summary>
    public ConversionOptions(
        CurrencyCode targetCurrency,
        DateTime effectiveDate,
        ExchangeRateKind rateKind,
        CurrencyRoundingPolicy roundingPolicy)
    {
        if (targetCurrency.IsDefault)
        {
            throw new ArgumentException("Target currency must be initialised.", nameof(targetCurrency));
        }

        if (effectiveDate == default)
        {
            throw new ArgumentException("Effective date must be specified.", nameof(effectiveDate));
        }

        if (rateKind == ExchangeRateKind.Unspecified)
        {
            throw new ArgumentException("Rate kind must be specified.", nameof(rateKind));
        }

        TargetCurrency = targetCurrency;
        EffectiveDate = effectiveDate;
        RateKind = rateKind;
        RoundingPolicy = roundingPolicy ?? throw new ArgumentNullException(nameof(roundingPolicy));
    }

    /// <summary>Gets the currency to convert to.</summary>
    public CurrencyCode TargetCurrency { get; }

    /// <summary>Gets the effective date or instant requested from the rate provider.</summary>
    public DateTime EffectiveDate { get; }

    /// <summary>Gets the requested rate kind.</summary>
    public ExchangeRateKind RateKind { get; }

    /// <summary>Gets the explicit rounding policy for the converted target amount.</summary>
    public CurrencyRoundingPolicy RoundingPolicy { get; }
}
