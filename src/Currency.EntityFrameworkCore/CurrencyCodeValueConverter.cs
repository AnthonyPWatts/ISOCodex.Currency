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
            currency => currency.IsDefault ? string.Empty : currency.Code,
            value => CurrencyCode.Parse(value),
            new ConverterMappingHints(size: 3, unicode: false))
    {
    }
}
