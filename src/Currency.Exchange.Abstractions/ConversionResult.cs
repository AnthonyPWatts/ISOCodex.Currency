using System;

namespace ISOCodex.Currency.Exchange.Abstractions;

/// <summary>
/// Represents an auditable money conversion result.
/// </summary>
public sealed class ConversionResult
{
    internal ConversionResult(
        Money input,
        Money output,
        ExchangeRate rate,
        decimal rawAmount,
        decimal roundedAmount,
        CurrencyRoundingPolicy roundingPolicy,
        DateTime requestedEffectiveDate,
        ExchangeRateKind requestedRateKind)
    {
        Input = input;
        Output = output;
        Rate = rate;
        RawAmount = rawAmount;
        RoundedAmount = roundedAmount;
        RoundingPolicy = roundingPolicy;
        RequestedEffectiveDate = requestedEffectiveDate;
        RequestedRateKind = requestedRateKind;
    }

    /// <summary>Gets the source money value.</summary>
    public Money Input { get; }

    /// <summary>Gets the rounded converted money value.</summary>
    public Money Output { get; }

    /// <summary>Gets the direct rate used for conversion.</summary>
    public ExchangeRate Rate { get; }

    /// <summary>Gets the unrounded target-currency amount.</summary>
    public decimal RawAmount { get; }

    /// <summary>Gets the rounded target-currency amount.</summary>
    public decimal RoundedAmount { get; }

    /// <summary>Gets the explicit rounding policy used for conversion.</summary>
    public CurrencyRoundingPolicy RoundingPolicy { get; }

    /// <summary>Gets the effective date requested from the provider.</summary>
    public DateTime RequestedEffectiveDate { get; }

    /// <summary>Gets the rate kind requested from the provider.</summary>
    public ExchangeRateKind RequestedRateKind { get; }

    /// <summary>Gets the source currency.</summary>
    public CurrencyCode SourceCurrency => Input.Currency;

    /// <summary>Gets the target currency.</summary>
    public CurrencyCode TargetCurrency => Output.Currency;

    /// <summary>Gets the provider, feed, file, or policy source used for audit purposes.</summary>
    public string RateSource => Rate.Source;
}
