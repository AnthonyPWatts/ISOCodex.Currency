namespace ISOCodex.Currency.Countries;

/// <summary>
/// Identifies a stable reason why country/currency validation failed.
/// </summary>
public enum CountryCurrencyValidationFailureReason
{
    /// <summary>No validation failure occurred.</summary>
    None = 0,

    /// <summary>The country code was the uninitialised default value.</summary>
    DefaultCountry,

    /// <summary>The country code is not known in the current bridge seed.</summary>
    UnknownCountry,

    /// <summary>The currency code was the uninitialised default value.</summary>
    DefaultCurrency,

    /// <summary>The currency code is not registered in ISOCodex.Currency.</summary>
    UnknownCurrency,

    /// <summary>The currency is not known for the country in the current bridge seed.</summary>
    CurrencyNotKnownForCountry,

    /// <summary>The currency is known for the country but is not allowed by the selected policy.</summary>
    CurrencyNotAllowedByPolicy,
}
