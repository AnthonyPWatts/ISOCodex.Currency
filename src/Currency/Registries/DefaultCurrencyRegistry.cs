using System;
using System.Collections.Generic;
using System.Linq;

namespace ISOCodex.Currency;

/// <summary>
/// Thread-safe default registry backed by the packaged currency data snapshot.
/// </summary>
public sealed class DefaultCurrencyRegistry : ICurrencyRegistry
{
    /// <summary>Gets the shared registry backed by packaged currency data.</summary>
    public static DefaultCurrencyRegistry Instance { get; } = new DefaultCurrencyRegistry();

    private readonly CurrencyInfo[] _all;
    private readonly IReadOnlyDictionary<CurrencyCode, CurrencyInfo> _byCode;
    private readonly IReadOnlyDictionary<string, CurrencyInfo> _byNumericCode;

    /// <summary>
    /// Creates a registry backed by the packaged currency data.
    /// </summary>
    public DefaultCurrencyRegistry()
        : this(CurrencyData.CreateCurrencyInfos())
    {
    }

    /// <summary>
    /// Creates a registry backed by the supplied currency metadata.
    /// </summary>
    public DefaultCurrencyRegistry(IEnumerable<CurrencyInfo> currencies)
    {
        if (currencies == null)
        {
            throw new ArgumentNullException(nameof(currencies));
        }

        _all = currencies.ToArray();
        _byCode = _all.ToDictionary(currency => currency.Code);
        _byNumericCode = _all
            .Where(currency => !string.IsNullOrWhiteSpace(currency.NumericCode))
            .ToDictionary(currency => currency.NumericCode, StringComparer.Ordinal);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<CurrencyInfo> All => _all;

    /// <inheritdoc />
    public CurrencyInfo Get(CurrencyCode code)
    {
        if (TryGet(code, out var currency))
        {
            return currency;
        }

        throw new KeyNotFoundException($"Currency code '{code}' is not registered.");
    }

    /// <inheritdoc />
    public bool TryGet(CurrencyCode code, out CurrencyInfo currency)
    {
        if (_byCode.TryGetValue(code, out var found))
        {
            currency = found;
            return true;
        }

        currency = null!;
        return false;
    }

    /// <inheritdoc />
    public CurrencyInfo GetByNumericCode(string numericCode)
    {
        if (TryGetByNumericCode(numericCode, out var currency))
        {
            return currency;
        }

        throw new KeyNotFoundException($"Currency numeric code '{numericCode}' is not registered.");
    }

    /// <inheritdoc />
    public bool TryGetByNumericCode(string? numericCode, out CurrencyInfo currency)
    {
        currency = null!;

        if (string.IsNullOrWhiteSpace(numericCode))
        {
            return false;
        }

        return _byNumericCode.TryGetValue(numericCode.Trim(), out currency!);
    }
}
