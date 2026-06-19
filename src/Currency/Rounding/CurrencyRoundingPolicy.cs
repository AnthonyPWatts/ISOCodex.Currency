using System;

namespace ISOCodex.Currency;

/// <summary>
/// Describes how an amount should be rounded for a currency.
/// </summary>
public sealed class CurrencyRoundingPolicy
{
    private CurrencyRoundingPolicy(
        CurrencyRoundingContext context,
        int? decimalPlaces,
        decimal? increment,
        MidpointRounding midpointRounding)
    {
        Context = context;
        DecimalPlaces = decimalPlaces;
        Increment = increment;
        MidpointRounding = midpointRounding;
    }

    /// <summary>Gets the rounding context.</summary>
    public CurrencyRoundingContext Context { get; }

    /// <summary>Gets the explicit decimal places, when configured.</summary>
    public int? DecimalPlaces { get; }

    /// <summary>Gets the explicit rounding increment, when configured.</summary>
    public decimal? Increment { get; }

    /// <summary>Gets the midpoint strategy.</summary>
    public MidpointRounding MidpointRounding { get; }

    /// <summary>
    /// Creates a policy that uses the currency's standard minor-unit precision.
    /// </summary>
    public static CurrencyRoundingPolicy Standard(MidpointRounding midpointRounding = MidpointRounding.ToEven)
    {
        return new CurrencyRoundingPolicy(CurrencyRoundingContext.Standard, null, null, midpointRounding);
    }

    /// <summary>
    /// Creates a standard precision policy using away-from-zero midpoint handling.
    /// </summary>
    public static CurrencyRoundingPolicy AwayFromZero()
    {
        return Standard(MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Creates a policy that uses the currency's cash rounding metadata.
    /// </summary>
    public static CurrencyRoundingPolicy Cash(MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
    {
        return new CurrencyRoundingPolicy(CurrencyRoundingContext.Cash, null, null, midpointRounding);
    }

    /// <summary>
    /// Creates a policy with explicit decimal-place precision.
    /// </summary>
    public static CurrencyRoundingPolicy Custom(int decimalPlaces, MidpointRounding midpointRounding)
    {
        if (decimalPlaces < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places must be zero or greater.");
        }

        return new CurrencyRoundingPolicy(CurrencyRoundingContext.Custom, decimalPlaces, null, midpointRounding);
    }

    /// <summary>
    /// Creates a policy with an explicit rounding increment.
    /// </summary>
    public static CurrencyRoundingPolicy CustomIncrement(decimal increment, MidpointRounding midpointRounding)
    {
        if (increment <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(increment), "Rounding increment must be greater than zero.");
        }

        return new CurrencyRoundingPolicy(CurrencyRoundingContext.Custom, null, increment, midpointRounding);
    }
}
