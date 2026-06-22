namespace ISOCodex.Currency.Countries;

/// <summary>
/// Controls which country/currency associations are accepted by validation.
/// </summary>
public sealed class CountryCurrencyValidationPolicy
{
    private CountryCurrencyValidationPolicy(bool primaryTenderOnly)
    {
        IsPrimaryTenderOnly = primaryTenderOnly;
    }

    /// <summary>Gets a policy that accepts only primary tender associations.</summary>
    public static CountryCurrencyValidationPolicy PrimaryTenderOnly { get; } = new CountryCurrencyValidationPolicy(true);

    /// <summary>Gets a policy that accepts any legal tender association in the registry.</summary>
    public static CountryCurrencyValidationPolicy AnyLegalTender { get; } = new CountryCurrencyValidationPolicy(false);

    internal bool IsPrimaryTenderOnly { get; }

    internal bool Allows(CountryCurrencyInfo info)
    {
        return !IsPrimaryTenderOnly || info.Role == CountryCurrencyRole.PrimaryTender;
    }
}
