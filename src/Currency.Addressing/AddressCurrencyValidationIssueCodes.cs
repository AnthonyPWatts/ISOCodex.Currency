using ISOCodex.Currency.Countries;

namespace ISOCodex.Currency.Addressing;

/// <summary>
/// Stable issue codes emitted by the address/currency bridge.
/// </summary>
public static class AddressCurrencyValidationIssueCodes
{
    /// <summary>Address input was required but not supplied.</summary>
    public const string AddressRequired = "Address.Required";

    /// <summary>No address validator was registered for the address country.</summary>
    public const string AddressValidatorUnavailable = "Address.ValidatorUnavailable";

    /// <summary>The address country could not be converted to an ISOCodex.Countries alpha-2 code.</summary>
    public const string AddressCountryInvalid = "Address.Country.Invalid";

    /// <summary>
    /// Creates the bridge issue code for a country/currency validation failure reason.
    /// </summary>
    public static string ForCountryCurrencyFailure(CountryCurrencyValidationFailureReason reason)
    {
        return "CountryCurrency." + reason;
    }
}
