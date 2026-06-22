using System.Collections.Generic;
using CountryAlpha2Code = ISOCodex.Countries.CountryAlpha2Code;

namespace ISOCodex.Currency.Countries;

/// <summary>
/// Provides lookup and validation for country/currency associations.
/// </summary>
public interface ICountryCurrencyRegistry
{
    /// <summary>Gets all known country/currency associations.</summary>
    IReadOnlyCollection<CountryCurrencyInfo> All { get; }

    /// <summary>Gets all known currency associations for a country.</summary>
    IReadOnlyCollection<CountryCurrencyInfo> GetByCountry(CountryAlpha2Code countryAlpha2Code);

    /// <summary>Attempts to get the primary tender association for a country.</summary>
    bool TryGetPrimaryCurrency(CountryAlpha2Code countryAlpha2Code, out CountryCurrencyInfo? countryCurrency);

    /// <summary>Validates a country/currency association using the supplied policy.</summary>
    CountryCurrencyValidationResult Validate(
        CountryAlpha2Code countryAlpha2Code,
        CurrencyCode currencyCode,
        CountryCurrencyValidationPolicy? policy = null);
}
