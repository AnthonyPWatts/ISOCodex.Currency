using System.Collections.Generic;

namespace ISOCodex.Currency;

/// <summary>
/// Provides lookup access to currency metadata.
/// </summary>
public interface ICurrencyRegistry
{
    /// <summary>Gets metadata for a registered currency code.</summary>
    CurrencyInfo Get(CurrencyCode code);

    /// <summary>Attempts to get metadata for a registered currency code.</summary>
    bool TryGet(CurrencyCode code, out CurrencyInfo currency);

    /// <summary>Gets metadata for a registered numeric currency code.</summary>
    CurrencyInfo GetByNumericCode(string numericCode);

    /// <summary>Attempts to get metadata for a registered numeric currency code.</summary>
    bool TryGetByNumericCode(string? numericCode, out CurrencyInfo currency);

    /// <summary>Gets all registered currencies.</summary>
    IReadOnlyCollection<CurrencyInfo> All { get; }
}
