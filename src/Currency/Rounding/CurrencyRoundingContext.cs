namespace ISOCodex.Currency;

/// <summary>
/// Describes the source of a currency rounding rule.
/// </summary>
public enum CurrencyRoundingContext
{
    /// <summary>Use the currency's standard minor-unit precision.</summary>
    Standard,

    /// <summary>Use the currency's cash precision or cash rounding increment.</summary>
    Cash,

    /// <summary>Use an explicit custom precision or increment.</summary>
    Custom
}
