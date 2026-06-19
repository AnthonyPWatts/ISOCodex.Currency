namespace ISOCodex.Currency;

/// <summary>
/// Describes how a formatted money value should identify its currency.
/// </summary>
public enum MoneyCurrencyDisplay
{
    /// <summary>Do not include a currency marker.</summary>
    None = 0,

    /// <summary>Display the alpha-3 currency code, such as GBP.</summary>
    Code = 1,

    /// <summary>Display a culture-derived currency symbol, such as £.</summary>
    Symbol = 2,

    /// <summary>Display both the alpha-3 currency code and a culture-derived symbol.</summary>
    CodeAndSymbol = 3
}
