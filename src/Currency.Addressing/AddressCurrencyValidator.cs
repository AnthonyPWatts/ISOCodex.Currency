using System;
using System.Collections.Generic;
using ISOCodex.Addressing;
using ISOCodex.Addressing.Validation;
using ISOCodex.Currency.Countries;

namespace ISOCodex.Currency.Addressing;

/// <summary>
/// Validates that an address is structurally valid and that a currency is accepted for the address country.
/// </summary>
public sealed class AddressCurrencyValidator
{
    private readonly IAddressValidatorFactory _addressValidatorFactory;
    private readonly ICountryCurrencyRegistry _countryCurrencyRegistry;

    /// <summary>
    /// Creates a validator using the default country/currency registry.
    /// </summary>
    public AddressCurrencyValidator(IAddressValidatorFactory addressValidatorFactory)
        : this(addressValidatorFactory, DefaultCountryCurrencyRegistry.Instance)
    {
    }

    /// <summary>
    /// Creates a validator using explicit address and country/currency validators.
    /// </summary>
    public AddressCurrencyValidator(
        IAddressValidatorFactory addressValidatorFactory,
        ICountryCurrencyRegistry countryCurrencyRegistry)
    {
        _addressValidatorFactory = addressValidatorFactory ?? throw new ArgumentNullException(nameof(addressValidatorFactory));
        _countryCurrencyRegistry = countryCurrencyRegistry ?? throw new ArgumentNullException(nameof(countryCurrencyRegistry));
    }

    /// <summary>
    /// Validates an address and currency combination.
    /// </summary>
    public AddressCurrencyValidationResult Validate(
        Address? address,
        CurrencyCode currencyCode,
        AddressCurrencyValidationPolicy? policy = null)
    {
        var issues = new List<AddressCurrencyValidationIssue>();
        var addressValidationResult = ValidateAddress(address, issues);
        CountryCurrencyValidationResult? countryCurrencyValidationResult = null;

        if (address != null)
        {
            var validationPolicy = policy ?? AddressCurrencyValidationPolicy.CheckoutDefault;
            countryCurrencyValidationResult = _countryCurrencyRegistry.Validate(
                address.CountryCode,
                currencyCode,
                validationPolicy.CountryCurrencyPolicy);

            if (!countryCurrencyValidationResult.Succeeded)
            {
                issues.Add(new AddressCurrencyValidationIssue(
                    AddressCurrencyValidationIssueSource.CountryCurrency,
                    AddressCurrencyValidationIssueCodes.ForCountryCurrencyFailure(countryCurrencyValidationResult.FailureReason),
                    countryCurrencyValidationResult.ErrorMessage ?? "Currency is not valid for the address country.",
                    nameof(currencyCode)));
            }
        }

        return new AddressCurrencyValidationResult(addressValidationResult, countryCurrencyValidationResult, issues);
    }

    private AddressValidationResult ValidateAddress(
        Address? address,
        ICollection<AddressCurrencyValidationIssue> issues)
    {
        if (address == null)
        {
            var required = new AddressValidationIssue(
                AddressCurrencyValidationIssueCodes.AddressRequired,
                "Address is required.");

            issues.Add(MapAddressIssue(required));
            return new AddressValidationResult(new[] { required });
        }

        IAddressValidator addressValidator;
        try
        {
            addressValidator = _addressValidatorFactory.GetValidator(address.CountryCode);
        }
        catch (InvalidOperationException ex)
        {
            var unavailable = new AddressValidationIssue(
                AddressCurrencyValidationIssueCodes.AddressValidatorUnavailable,
                ex.Message,
                nameof(Address.CountryCode));

            issues.Add(MapAddressIssue(unavailable));
            return new AddressValidationResult(new[] { unavailable });
        }

        var result = addressValidator.Validate(address);
        foreach (var issue in result.Issues)
        {
            issues.Add(MapAddressIssue(issue));
        }

        return result;
    }

    private static AddressCurrencyValidationIssue MapAddressIssue(AddressValidationIssue issue)
    {
        return new AddressCurrencyValidationIssue(
            AddressCurrencyValidationIssueSource.Address,
            issue.Code,
            issue.Message,
            issue.PropertyName);
    }
}
