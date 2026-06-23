using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ISOCodex.Currency.EntityFrameworkCore;

/// <summary>
/// Converts <see cref="CurrencyCode"/> values to and from uppercase alpha-3 database strings.
/// </summary>
public sealed class CurrencyCodeValueConverter : ValueConverter<CurrencyCode, string>
{
    /// <summary>Gets a reusable converter instance.</summary>
    public static CurrencyCodeValueConverter Instance { get; } = new CurrencyCodeValueConverter();

    /// <summary>Creates a converter for currency code values.</summary>
    public CurrencyCodeValueConverter()
        : base(
            currency => ConvertToProviderValue(currency),
            value => CurrencyCode.Parse(value),
            new ConverterMappingHints(size: 3, unicode: false))
    {
    }

    private static string ConvertToProviderValue(CurrencyCode currency)
    {
        if (currency.IsDefault)
        {
            throw new InvalidOperationException("Currency code must be initialised before it can be stored.");
        }

        return currency.Code;
    }
}
