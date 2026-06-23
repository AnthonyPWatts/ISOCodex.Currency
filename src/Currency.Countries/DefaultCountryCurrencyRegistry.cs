using System;
using System.Collections.Generic;
using System.Linq;
using CountryAlpha2Code = ISOCodex.Countries.CountryAlpha2Code;

namespace ISOCodex.Currency.Countries;

/// <summary>
/// Default country/currency registry backed by a small explicit seed.
/// </summary>
public sealed class DefaultCountryCurrencyRegistry : ICountryCurrencyRegistry
{
    /// <summary>Gets the shared default registry.</summary>
    public static DefaultCountryCurrencyRegistry Instance { get; } = new DefaultCountryCurrencyRegistry();

    private readonly CountryCurrencyInfo[] _all;
    private readonly IReadOnlyDictionary<CountryAlpha2Code, CountryCurrencyInfo[]> _byCountry;

    /// <summary>
    /// Creates a registry backed by the packaged country/currency seed.
    /// </summary>
    public DefaultCountryCurrencyRegistry()
        : this(CreateSeed())
    {
    }

    /// <summary>
    /// Creates a registry backed by supplied country/currency associations.
    /// </summary>
    public DefaultCountryCurrencyRegistry(IEnumerable<CountryCurrencyInfo> countryCurrencies)
    {
        if (countryCurrencies == null)
        {
            throw new ArgumentNullException(nameof(countryCurrencies));
        }

        _all = countryCurrencies.ToArray();
        _byCountry = _all
            .GroupBy(info => info.CountryAlpha2Code)
            .ToDictionary(group => group.Key, group => group.ToArray());
    }

    /// <inheritdoc />
    public IReadOnlyCollection<CountryCurrencyInfo> All => _all;

    /// <inheritdoc />
    public IReadOnlyCollection<CountryCurrencyInfo> GetByCountry(CountryAlpha2Code countryAlpha2Code)
    {
        return _byCountry.TryGetValue(countryAlpha2Code, out var currencies)
            ? currencies
            : Array.Empty<CountryCurrencyInfo>();
    }

    /// <inheritdoc />
    public bool TryGetPrimaryCurrency(CountryAlpha2Code countryAlpha2Code, out CountryCurrencyInfo? countryCurrency)
    {
        countryCurrency = GetByCountry(countryAlpha2Code)
            .FirstOrDefault(info => info.Role == CountryCurrencyRole.PrimaryTender);

        return countryCurrency != null;
    }

    /// <inheritdoc />
    public CountryCurrencyValidationResult Validate(
        CountryAlpha2Code countryAlpha2Code,
        CurrencyCode currencyCode,
        CountryCurrencyValidationPolicy? policy = null)
    {
        if (string.IsNullOrWhiteSpace(countryAlpha2Code.Value))
        {
            return CountryCurrencyValidationResult.Failure(
                CountryCurrencyValidationFailureReason.DefaultCountry,
                "Country alpha-2 code must be initialised.");
        }

        if (currencyCode.IsDefault)
        {
            return CountryCurrencyValidationResult.Failure(
                CountryCurrencyValidationFailureReason.DefaultCurrency,
                "Currency code must be initialised.");
        }

        if (!DefaultCurrencyRegistry.Instance.TryGet(currencyCode, out _))
        {
            return CountryCurrencyValidationResult.Failure(
                CountryCurrencyValidationFailureReason.UnknownCurrency,
                $"Currency code '{currencyCode}' is not registered.");
        }

        var countryCurrencies = GetByCountry(countryAlpha2Code);
        if (countryCurrencies.Count == 0)
        {
            return CountryCurrencyValidationResult.Failure(
                CountryCurrencyValidationFailureReason.UnknownCountry,
                $"Country alpha-2 code '{countryAlpha2Code}' is not known in the current bridge seed.");
        }

        var matches = countryCurrencies
            .Where(info => info.CurrencyCode == currencyCode)
            .ToArray();

        if (matches.Length == 0)
        {
            return CountryCurrencyValidationResult.Failure(
                CountryCurrencyValidationFailureReason.CurrencyNotKnownForCountry,
                $"Currency '{currencyCode}' is not known for country '{countryAlpha2Code}' in the current bridge seed.");
        }

        var validationPolicy = policy ?? CountryCurrencyValidationPolicy.PrimaryTenderOnly;
        var allowed = matches.FirstOrDefault(validationPolicy.Allows);

        if (allowed == null)
        {
            return CountryCurrencyValidationResult.Failure(
                CountryCurrencyValidationFailureReason.CurrencyNotAllowedByPolicy,
                $"Currency '{currencyCode}' is not allowed for country '{countryAlpha2Code}' by the selected validation policy.");
        }

        return CountryCurrencyValidationResult.Success(allowed);
    }

    private static IEnumerable<CountryCurrencyInfo> CreateSeed()
    {
        yield return Primary("GB", CurrencyCode.GBP);
        yield return Primary("US", CurrencyCode.USD);
        yield return Primary("IE", CurrencyCode.EUR);
        yield return Primary("JP", CurrencyCode.JPY);
        yield return Primary("CH", CurrencyCode.CHF);
        yield return Primary("CA", new CurrencyCode("CAD"));
        yield return Primary("AU", new CurrencyCode("AUD"));
        yield return Primary("NZ", new CurrencyCode("NZD"));
    }

    private static CountryCurrencyInfo Primary(string countryAlpha2Code, CurrencyCode currencyCode)
    {
        return new CountryCurrencyInfo(
            CountryAlpha2Code.Parse(countryAlpha2Code),
            currencyCode,
            CountryCurrencyRole.PrimaryTender);
    }
}
