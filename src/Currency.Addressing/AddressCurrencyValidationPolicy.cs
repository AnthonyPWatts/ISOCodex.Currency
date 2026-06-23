using ISOCodex.Currency.Countries;

namespace ISOCodex.Currency.Addressing;

/// <summary>
/// Controls how an address/currency combination is validated.
/// </summary>
public sealed class AddressCurrencyValidationPolicy
{
    /// <summary>
    /// Gets the default checkout policy: address validation plus primary tender currency only.
    /// </summary>
    public static AddressCurrencyValidationPolicy CheckoutDefault { get; } =
        new AddressCurrencyValidationPolicy(CountryCurrencyValidationPolicy.PrimaryTenderOnly);

    /// <summary>
    /// Gets a policy that allows any legal tender known for the address country.
    /// </summary>
    public static AddressCurrencyValidationPolicy AnyLegalTender { get; } =
        new AddressCurrencyValidationPolicy(CountryCurrencyValidationPolicy.AnyLegalTender);

    /// <summary>
    /// Creates an address/currency validation policy.
    /// </summary>
    public AddressCurrencyValidationPolicy(CountryCurrencyValidationPolicy? countryCurrencyPolicy = null)
    {
        CountryCurrencyPolicy = countryCurrencyPolicy ?? CountryCurrencyValidationPolicy.PrimaryTenderOnly;
    }

    /// <summary>Gets the country/currency policy delegated to ISOCodex.Currency.Countries.</summary>
    public CountryCurrencyValidationPolicy CountryCurrencyPolicy { get; }
}
