namespace ISOCodex.Currency.Countries;

/// <summary>
/// Describes the role a currency has for a country.
/// </summary>
public enum CountryCurrencyRole
{
    /// <summary>The currency is the primary tender in the current seed.</summary>
    PrimaryTender = 0,

    /// <summary>The currency is legal tender but not marked as the primary tender.</summary>
    LegalTender = 1,
}
