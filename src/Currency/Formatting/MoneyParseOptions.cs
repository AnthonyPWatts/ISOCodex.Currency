using System.Globalization;

namespace ISOCodex.Currency;

/// <summary>
/// Options for parsing money values.
/// </summary>
public sealed class MoneyParseOptions
{
    /// <summary>Gets default parsing options.</summary>
    public static MoneyParseOptions Default { get; } = new MoneyParseOptions();

    /// <summary>
    /// Creates money parsing options.
    /// </summary>
    public MoneyParseOptions(
        CultureInfo? culture = null,
        CurrencyCode? expectedCurrency = null,
        MoneyParseCurrencyStyles currencyStyles = MoneyParseCurrencyStyles.Code)
    {
        Culture = culture ?? CultureInfo.CurrentCulture;
        ExpectedCurrency = expectedCurrency;
        CurrencyStyles = currencyStyles;
    }

    /// <summary>Gets the culture used to parse separators, signs, and currency symbols.</summary>
    public CultureInfo Culture { get; }

    /// <summary>Gets the expected currency, when the caller already knows it.</summary>
    public CurrencyCode? ExpectedCurrency { get; }

    /// <summary>Gets which currency markers the parser may accept.</summary>
    public MoneyParseCurrencyStyles CurrencyStyles { get; }
}
