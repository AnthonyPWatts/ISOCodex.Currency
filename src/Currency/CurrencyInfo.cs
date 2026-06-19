using System;
using System.Collections.Generic;
using System.Linq;

namespace ISOCodex.Currency;

/// <summary>
/// Immutable metadata for an ISO 4217-style currency entry.
/// </summary>
public sealed class CurrencyInfo
{
    /// <summary>
    /// Creates a currency metadata entry.
    /// </summary>
    public CurrencyInfo(
        CurrencyCode code,
        string numericCode,
        string englishName,
        CurrencyMinorUnit minorUnit,
        CurrencyKind kind,
        bool isTender,
        IEnumerable<string>? territories = null,
        CurrencyMinorUnit? cashMinorUnit = null,
        decimal? cashRoundingIncrement = null)
    {
        Code = code;
        NumericCode = numericCode ?? throw new ArgumentNullException(nameof(numericCode));
        EnglishName = englishName ?? throw new ArgumentNullException(nameof(englishName));
        MinorUnit = minorUnit ?? throw new ArgumentNullException(nameof(minorUnit));
        CashMinorUnit = cashMinorUnit ?? minorUnit;
        CashRoundingIncrement = cashRoundingIncrement;
        Kind = kind;
        IsTender = isTender;
        Territories = (territories ?? Enumerable.Empty<string>()).ToArray();
    }

    /// <summary>Gets the alpha-3 currency code.</summary>
    public CurrencyCode Code { get; }

    /// <summary>Gets the ISO 4217-style numeric currency code.</summary>
    public string NumericCode { get; }

    /// <summary>Gets the English display name.</summary>
    public string EnglishName { get; }

    /// <summary>Gets the standard minor-unit metadata.</summary>
    public CurrencyMinorUnit MinorUnit { get; }

    /// <summary>Gets the cash minor-unit metadata.</summary>
    public CurrencyMinorUnit CashMinorUnit { get; }

    /// <summary>Gets the cash rounding increment when one is known.</summary>
    public decimal? CashRoundingIncrement { get; }

    /// <summary>Gets the broad metadata kind.</summary>
    public CurrencyKind Kind { get; }

    /// <summary>Gets whether the currency is tender in the metadata snapshot.</summary>
    public bool IsTender { get; }

    /// <summary>Gets the territories associated with the currency entry.</summary>
    public IReadOnlyList<string> Territories { get; }
}
