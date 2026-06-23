using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ISOCodex.Addressing.Validation;
using ISOCodex.Currency.Countries;

namespace ISOCodex.Currency.Addressing;

/// <summary>
/// Represents the combined result of address validation and country/currency validation.
/// </summary>
public sealed class AddressCurrencyValidationResult
{
    internal AddressCurrencyValidationResult(
        AddressValidationResult addressValidationResult,
        CountryCurrencyValidationResult? countryCurrencyValidationResult,
        IEnumerable<AddressCurrencyValidationIssue> issues)
    {
        AddressValidationResult = addressValidationResult;
        CountryCurrencyValidationResult = countryCurrencyValidationResult;
        Issues = new ReadOnlyCollection<AddressCurrencyValidationIssue>(issues.ToArray());
        Succeeded = AddressValidationResult.IsValid
            && CountryCurrencyValidationResult?.Succeeded == true
            && Issues.Count == 0;
    }

    /// <summary>Gets whether both delegated validations succeeded.</summary>
    public bool Succeeded { get; }

    /// <summary>Gets the delegated address validation result.</summary>
    public AddressValidationResult AddressValidationResult { get; }

    /// <summary>Gets the delegated country/currency validation result when one could be evaluated.</summary>
    public CountryCurrencyValidationResult? CountryCurrencyValidationResult { get; }

    /// <summary>Gets all address and country/currency issues in one stable list.</summary>
    public IReadOnlyList<AddressCurrencyValidationIssue> Issues { get; }
}
