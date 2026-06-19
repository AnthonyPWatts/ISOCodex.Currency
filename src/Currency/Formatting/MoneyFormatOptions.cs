using System;
using System.Globalization;

namespace ISOCodex.Currency;

/// <summary>
/// Options for formatting money values.
/// </summary>
public sealed class MoneyFormatOptions
{
    /// <summary>Gets default formatting options.</summary>
    public static MoneyFormatOptions Default { get; } = new MoneyFormatOptions();

    /// <summary>
    /// Creates money formatting options.
    /// </summary>
    public MoneyFormatOptions(
        CultureInfo? culture = null,
        MoneyCurrencyDisplay currencyDisplay = MoneyCurrencyDisplay.Code,
        bool useCurrencyDecimalPlaces = true)
    {
        if (!Enum.IsDefined(typeof(MoneyCurrencyDisplay), currencyDisplay))
        {
            throw new ArgumentOutOfRangeException(nameof(currencyDisplay), currencyDisplay, "Unknown currency display option.");
        }

        Culture = culture ?? CultureInfo.CurrentCulture;
        CurrencyDisplay = currencyDisplay;
        UseCurrencyDecimalPlaces = useCurrencyDecimalPlaces;
    }

    /// <summary>Gets the culture used to format separators, signs, and currency patterns.</summary>
    public CultureInfo Culture { get; }

    /// <summary>Gets how the formatted value should identify its currency.</summary>
    public MoneyCurrencyDisplay CurrencyDisplay { get; }

    /// <summary>Gets whether formatting should use the currency metadata decimal places.</summary>
    public bool UseCurrencyDecimalPlaces { get; }
}
