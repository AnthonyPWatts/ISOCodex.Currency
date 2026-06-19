namespace ISOCodex.Currency;

/// <summary>
/// Describes why parsing a money value failed.
/// </summary>
public enum MoneyParseFailureReason
{
    /// <summary>Parsing succeeded.</summary>
    None = 0,

    /// <summary>The input was null, empty, or whitespace.</summary>
    EmptyInput,

    /// <summary>The input did not include a currency and no expected currency was supplied.</summary>
    MissingCurrency,

    /// <summary>The input contained an unknown currency code.</summary>
    UnknownCurrency,

    /// <summary>The input contained more than one possible currency code.</summary>
    AmbiguousCurrency,

    /// <summary>The input currency did not match the expected currency.</summary>
    CurrencyMismatch,

    /// <summary>The amount could not be parsed using the requested culture.</summary>
    InvalidAmount,

    /// <summary>The amount has invalid precision for the parsed currency.</summary>
    AmountPrecision
}
