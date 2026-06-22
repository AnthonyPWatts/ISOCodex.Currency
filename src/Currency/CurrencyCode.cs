using System;
using System.Collections.Generic;
using System.Linq;

namespace ISOCodex.Currency;

/// <summary>
/// Represents a registered ISO 4217-style alpha-3 currency code.
/// </summary>
public readonly struct CurrencyCode : IEquatable<CurrencyCode>
{
    /// <summary>
    /// Creates a currency code from a registered alpha-3 code.
    /// </summary>
    public CurrencyCode(string code)
    {
        if (!TryParse(code, out var currencyCode))
        {
            throw new ArgumentException($"'{code}' is not a registered ISO 4217-style currency code.", nameof(code));
        }

        Code = currencyCode.Code;
    }

    internal CurrencyCode(string code, bool skipValidation)
    {
        Code = skipValidation
            ? code.ToUpperInvariant()
            : new CurrencyCode(code).Code;
    }

    /// <summary>Gets the normalized alpha-3 code.</summary>
    public string Code { get; }

    /// <summary>Gets whether this value is the uninitialised default currency code.</summary>
    public bool IsDefault => string.IsNullOrEmpty(Code);

    /// <summary>Gets the pound sterling currency code.</summary>
    public static CurrencyCode GBP => new CurrencyCode("GBP", true);

    /// <summary>Gets the US dollar currency code.</summary>
    public static CurrencyCode USD => new CurrencyCode("USD", true);

    /// <summary>Gets the euro currency code.</summary>
    public static CurrencyCode EUR => new CurrencyCode("EUR", true);

    /// <summary>Gets the yen currency code.</summary>
    public static CurrencyCode JPY => new CurrencyCode("JPY", true);

    /// <summary>Gets the Swiss franc currency code.</summary>
    public static CurrencyCode CHF => new CurrencyCode("CHF", true);

    /// <summary>Gets the Kuwaiti dinar currency code.</summary>
    public static CurrencyCode KWD => new CurrencyCode("KWD", true);

    /// <summary>Gets the Bahraini dinar currency code.</summary>
    public static CurrencyCode BHD => new CurrencyCode("BHD", true);

    /// <summary>Gets the no-currency code.</summary>
    public static CurrencyCode XXX => new CurrencyCode("XXX", true);

    /// <summary>Gets all registered currency codes.</summary>
    public static IEnumerable<CurrencyCode> All => DefaultCurrencyRegistry.Instance.All.Select(currency => currency.Code);

    /// <summary>
    /// Parses a registered alpha-3 currency code.
    /// </summary>
    public static CurrencyCode Parse(string input)
    {
        if (!TryParse(input, out var currencyCode))
        {
            throw new ArgumentException($"'{input}' is not a registered ISO 4217-style currency code.", nameof(input));
        }

        return currencyCode;
    }

    /// <summary>
    /// Attempts to parse a registered alpha-3 currency code.
    /// </summary>
    public static bool TryParse(string? input, out CurrencyCode currencyCode)
    {
        currencyCode = default;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var normalized = input.Trim().ToUpperInvariant();

        if (!IsAlpha3(normalized))
        {
            return false;
        }

        if (!CurrencyData.ContainsCode(normalized))
        {
            return false;
        }

        currencyCode = new CurrencyCode(normalized, true);
        return true;
    }

    /// <summary>
    /// Returns whether the input is a registered alpha-3 currency code.
    /// </summary>
    public static bool IsValid(string? input)
    {
        return TryParse(input, out _);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Code ?? string.Empty;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is CurrencyCode other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(CurrencyCode other)
    {
        return string.Equals(Code, other.Code, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return StringComparer.Ordinal.GetHashCode(Code ?? string.Empty);
    }

    /// <summary>Returns whether two currency codes are equal.</summary>
    public static bool operator ==(CurrencyCode left, CurrencyCode right)
    {
        return left.Equals(right);
    }

    /// <summary>Returns whether two currency codes are not equal.</summary>
    public static bool operator !=(CurrencyCode left, CurrencyCode right)
    {
        return !left.Equals(right);
    }

    private static bool IsAlpha3(string value)
    {
        return value.Length == 3
            && value[0] >= 'A' && value[0] <= 'Z'
            && value[1] >= 'A' && value[1] <= 'Z'
            && value[2] >= 'A' && value[2] <= 'Z';
    }
}
