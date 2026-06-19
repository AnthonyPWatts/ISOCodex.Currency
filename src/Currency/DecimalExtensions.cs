using System;

namespace AnthonyPWatts.Currency;

/// <summary>
/// Legacy decimal helpers retained as obsolete compatibility shims.
/// </summary>
public static class DecimalExtensions
{
    /// <summary>
    /// Truncates a decimal to two decimal places, effectively rounding down.
    /// </summary>
    /// <param name="value">The value to truncate.</param>
    /// <returns>The value truncated to two decimal places.</returns>
    [Obsolete("Use ISOCodex.Currency rounding services in the new API.")]
    public static decimal TruncateToCurrency(this decimal value)
    {
        return Math.Floor(value * 100) / 100;
    }

    /// <summary>
    /// Rounds a decimal to two decimal places using away-from-zero midpoint handling.
    /// </summary>
    /// <param name="value">The value to round.</param>
    /// <returns>The value rounded to two decimal places.</returns>
    [Obsolete("Use ISOCodex.Currency rounding services in the new API.")]
    public static decimal RoundToCurrency(this decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
