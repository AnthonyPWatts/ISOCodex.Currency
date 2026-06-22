using System;
using CountryAlpha2Code = ISOCodex.Countries.CountryAlpha2Code;

namespace ISOCodex.Currency.Countries;

/// <summary>
/// Describes a currency associated with a country code.
/// </summary>
public sealed class CountryCurrencyInfo
{
    /// <summary>
    /// Creates a country/currency association.
    /// </summary>
    public CountryCurrencyInfo(
        CountryAlpha2Code countryAlpha2Code,
        CurrencyCode currencyCode,
        CountryCurrencyRole role = CountryCurrencyRole.PrimaryTender)
    {
        if (string.IsNullOrWhiteSpace(countryAlpha2Code.Value))
        {
            throw new ArgumentException("Country alpha-2 code must be initialised.", nameof(countryAlpha2Code));
        }

        if (currencyCode.IsDefault)
        {
            throw new ArgumentException("Currency code must be initialised.", nameof(currencyCode));
        }

        if (!DefaultCurrencyRegistry.Instance.TryGet(currencyCode, out _))
        {
            throw new ArgumentException($"Currency code '{currencyCode}' is not registered.", nameof(currencyCode));
        }

        CountryAlpha2Code = countryAlpha2Code;
        CurrencyCode = currencyCode;
        Role = role;
    }

    /// <summary>Gets the country alpha-2 code.</summary>
    public CountryAlpha2Code CountryAlpha2Code { get; }

    /// <summary>Gets the associated currency code.</summary>
    public CurrencyCode CurrencyCode { get; }

    /// <summary>Gets the role this currency has for the country.</summary>
    public CountryCurrencyRole Role { get; }
}
