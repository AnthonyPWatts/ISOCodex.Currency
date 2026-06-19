using System;

namespace ISOCodex.Currency;

/// <summary>
/// Describes the fractional precision used by a currency.
/// </summary>
public sealed class CurrencyMinorUnit
{
    /// <summary>
    /// Represents a currency entry where a minor unit is not applicable.
    /// </summary>
    public static CurrencyMinorUnit NotApplicable { get; } = new CurrencyMinorUnit();

    private CurrencyMinorUnit()
    {
        IsApplicable = false;
        DecimalPlaces = 0;
        Increment = 0m;
    }

    /// <summary>
    /// Creates an applicable minor-unit definition.
    /// </summary>
    /// <param name="decimalPlaces">The number of decimal places used by the currency.</param>
    public CurrencyMinorUnit(int decimalPlaces)
    {
        if (decimalPlaces < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places must be zero or greater.");
        }

        IsApplicable = true;
        DecimalPlaces = decimalPlaces;
        Increment = CalculateIncrement(decimalPlaces);
    }

    /// <summary>Gets whether the currency has an applicable minor unit.</summary>
    public bool IsApplicable { get; }

    /// <summary>Gets the number of decimal places used by the minor unit.</summary>
    public int DecimalPlaces { get; }

    /// <summary>Gets the decimal increment represented by the minor unit.</summary>
    public decimal Increment { get; }

    private static decimal CalculateIncrement(int decimalPlaces)
    {
        decimal increment = 1m;

        for (var i = 0; i < decimalPlaces; i++)
        {
            increment /= 10m;
        }

        return increment;
    }
}
