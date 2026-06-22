using ISOCodex.Currency;
using ISOCodex.Currency.Countries;
using CountryAlpha2Code = ISOCodex.Countries.CountryAlpha2Code;

namespace Currency.Countries.Tests;

public class DefaultCountryCurrencyRegistryTests
{
    [Theory]
    [InlineData("GB", "GBP")]
    [InlineData("US", "USD")]
    [InlineData("IE", "EUR")]
    [InlineData("JP", "JPY")]
    [InlineData("CH", "CHF")]
    [InlineData("CA", "CAD")]
    [InlineData("AU", "AUD")]
    [InlineData("NZ", "NZD")]
    public void DefaultSeed_ContainsExpectedPrimaryCurrencies(string countryCode, string currencyCode)
    {
        var country = CountryAlpha2Code.Parse(countryCode);

        var found = DefaultCountryCurrencyRegistry.Instance.TryGetPrimaryCurrency(country, out var countryCurrency);

        Assert.True(found);
        Assert.NotNull(countryCurrency);
        Assert.Equal(country, countryCurrency.CountryAlpha2Code);
        Assert.Equal(CurrencyCode.Parse(currencyCode), countryCurrency.CurrencyCode);
        Assert.Equal(CountryCurrencyRole.PrimaryTender, countryCurrency.Role);
    }

    [Fact]
    public void Validate_AcceptsPrimaryTenderByDefault()
    {
        var result = DefaultCountryCurrencyRegistry.Instance.Validate(
            CountryAlpha2Code.Parse("GB"),
            CurrencyCode.GBP);

        Assert.True(result.Succeeded);
        Assert.Equal(CountryCurrencyValidationFailureReason.None, result.FailureReason);
        Assert.Equal(CurrencyCode.GBP, result.CountryCurrency?.CurrencyCode);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Validate_RejectsCurrencyThatIsNotKnownForCountry()
    {
        var result = DefaultCountryCurrencyRegistry.Instance.Validate(
            CountryAlpha2Code.Parse("GB"),
            CurrencyCode.USD);

        Assert.False(result.Succeeded);
        Assert.Equal(CountryCurrencyValidationFailureReason.CurrencyNotKnownForCountry, result.FailureReason);
        Assert.Contains("GB", result.ErrorMessage);
        Assert.Contains("USD", result.ErrorMessage);
    }

    [Fact]
    public void Validate_RejectsUnknownCountry()
    {
        var result = DefaultCountryCurrencyRegistry.Instance.Validate(
            CountryAlpha2Code.Parse("ZZ"),
            CurrencyCode.GBP);

        Assert.False(result.Succeeded);
        Assert.Equal(CountryCurrencyValidationFailureReason.UnknownCountry, result.FailureReason);
    }

    [Fact]
    public void Validate_RejectsDefaultCountry()
    {
        var result = DefaultCountryCurrencyRegistry.Instance.Validate(
            default,
            CurrencyCode.GBP);

        Assert.False(result.Succeeded);
        Assert.Equal(CountryCurrencyValidationFailureReason.DefaultCountry, result.FailureReason);
    }

    [Fact]
    public void Validate_RejectsDefaultCurrency()
    {
        var result = DefaultCountryCurrencyRegistry.Instance.Validate(
            CountryAlpha2Code.Parse("GB"),
            default);

        Assert.False(result.Succeeded);
        Assert.Equal(CountryCurrencyValidationFailureReason.DefaultCurrency, result.FailureReason);
    }

    [Fact]
    public void Validate_AppliesPolicyToKnownAssociation()
    {
        var registry = new DefaultCountryCurrencyRegistry(new[]
        {
            new CountryCurrencyInfo(CountryAlpha2Code.Parse("GB"), CurrencyCode.USD, CountryCurrencyRole.LegalTender)
        });

        var primaryResult = registry.Validate(
            CountryAlpha2Code.Parse("GB"),
            CurrencyCode.USD,
            CountryCurrencyValidationPolicy.PrimaryTenderOnly);

        var anyTenderResult = registry.Validate(
            CountryAlpha2Code.Parse("GB"),
            CurrencyCode.USD,
            CountryCurrencyValidationPolicy.AnyLegalTender);

        Assert.False(primaryResult.Succeeded);
        Assert.Equal(CountryCurrencyValidationFailureReason.CurrencyNotAllowedByPolicy, primaryResult.FailureReason);
        Assert.True(anyTenderResult.Succeeded);
        Assert.Equal(CountryCurrencyRole.LegalTender, anyTenderResult.CountryCurrency?.Role);
    }
}
