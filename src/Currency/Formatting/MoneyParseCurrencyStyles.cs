using System;

namespace ISOCodex.Currency;

/// <summary>
/// Describes which currency markers a money parser may accept.
/// </summary>
[Flags]
public enum MoneyParseCurrencyStyles
{
    /// <summary>Do not accept currency markers.</summary>
    None = 0,

    /// <summary>Accept alpha-3 currency codes, such as GBP.</summary>
    Code = 1,

    /// <summary>Accept the expected currency's culture-derived symbol.</summary>
    Symbol = 2,

    /// <summary>Accept either alpha-3 currency codes or the expected currency's culture-derived symbol.</summary>
    CodeOrSymbol = Code | Symbol
}
