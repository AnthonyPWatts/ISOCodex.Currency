using ISOCodex.Addressing;
using ISOCodex.Addressing.Validation;
using ISOCodex.Currency;
using ISOCodex.Currency.Addressing;
using ISOCodex.Currency.Countries;

namespace Currency.Addressing.Tests;

public class AddressCurrencyValidatorTests
{
    [Fact]
    public void Validate_ReturnsSuccessWhenAddressAndCurrencyAreValid()
    {
        var validator = new AddressCurrencyValidator(new StubAddressValidatorFactory(AddressValidationResult.Success));

        var result = validator.Validate(CreateAddress(CountryCode.GB), CurrencyCode.GBP);

        Assert.True(result.Succeeded);
        Assert.Empty(result.Issues);
        Assert.True(result.AddressValidationResult.IsValid);
        Assert.True(result.CountryCurrencyValidationResult?.Succeeded);
    }

    [Fact]
    public void Validate_CombinesAddressIssuesWithCountryCurrencyIssues()
    {
        var addressIssue = new AddressValidationIssue("Address.Line1.Required", "Line1 is required.", "Line1");
        var validator = new AddressCurrencyValidator(new StubAddressValidatorFactory(new AddressValidationResult(new[] { addressIssue })));

        var result = validator.Validate(CreateAddress(CountryCode.GB), CurrencyCode.USD);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Issues, issue =>
            issue.Source == AddressCurrencyValidationIssueSource.Address
            && issue.Code == "Address.Line1.Required"
            && issue.PropertyName == "Line1");
        Assert.Contains(result.Issues, issue =>
            issue.Source == AddressCurrencyValidationIssueSource.CountryCurrency
            && issue.Code == AddressCurrencyValidationIssueCodes.ForCountryCurrencyFailure(CountryCurrencyValidationFailureReason.CurrencyNotKnownForCountry)
            && issue.PropertyName == "currencyCode");
    }

    [Fact]
    public void Validate_ReturnsStructuredIssueWhenAddressIsMissing()
    {
        var validator = new AddressCurrencyValidator(new StubAddressValidatorFactory(AddressValidationResult.Success));

        var result = validator.Validate(null, CurrencyCode.GBP);

        var issue = Assert.Single(result.Issues);
        Assert.False(result.Succeeded);
        Assert.Equal(AddressCurrencyValidationIssueSource.Address, issue.Source);
        Assert.Equal(AddressCurrencyValidationIssueCodes.AddressRequired, issue.Code);
        Assert.Null(result.CountryCurrencyValidationResult);
    }

    [Fact]
    public void Validate_ReturnsStructuredIssueWhenNoAddressValidatorIsRegistered()
    {
        var validator = new AddressCurrencyValidator(new ThrowingAddressValidatorFactory());

        var result = validator.Validate(CreateAddress(CountryCode.GB), CurrencyCode.GBP);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Issues, issue =>
            issue.Source == AddressCurrencyValidationIssueSource.Address
            && issue.Code == AddressCurrencyValidationIssueCodes.AddressValidatorUnavailable
            && issue.PropertyName == nameof(Address.CountryCode));
        Assert.True(result.CountryCurrencyValidationResult?.Succeeded);
    }

    private static Address CreateAddress(CountryCode countryCode)
    {
        return new Address(
            "1 High Street",
            null,
            "London",
            null,
            new PostalCode("SW1A 1AA"),
            countryCode);
    }

    private sealed class StubAddressValidatorFactory : IAddressValidatorFactory
    {
        private readonly IAddressValidator _validator;

        public StubAddressValidatorFactory(AddressValidationResult result)
        {
            _validator = new StubAddressValidator(result);
        }

        public IAddressValidator GetValidator(CountryCode countryCode)
        {
            return _validator;
        }

        public void RegisterValidator(CountryCode countryCode, IAddressValidator validator)
        {
        }
    }

    private sealed class ThrowingAddressValidatorFactory : IAddressValidatorFactory
    {
        public IAddressValidator GetValidator(CountryCode countryCode)
        {
            throw new InvalidOperationException($"No address validator registered for country code '{countryCode.Code}'.");
        }

        public void RegisterValidator(CountryCode countryCode, IAddressValidator validator)
        {
        }
    }

    private sealed class StubAddressValidator : IAddressValidator
    {
        private readonly AddressValidationResult _result;

        public StubAddressValidator(AddressValidationResult result)
        {
            _result = result;
        }

        public AddressValidationResult Validate(Address? address)
        {
            return _result;
        }
    }
}
