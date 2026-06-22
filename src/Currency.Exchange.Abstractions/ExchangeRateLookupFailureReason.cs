namespace ISOCodex.Currency.Exchange.Abstractions;

/// <summary>
/// Stable failure reasons for exchange-rate lookup attempts.
/// </summary>
public enum ExchangeRateLookupFailureReason
{
    /// <summary>No failure occurred.</summary>
    None = 0,

    /// <summary>The provider has no matching rate.</summary>
    RateUnavailable = 1,

    /// <summary>The requested pair is not supported.</summary>
    UnsupportedPair = 2,

    /// <summary>The requested effective date is not supported.</summary>
    UnsupportedEffectiveDate = 3,

    /// <summary>The requested rate kind is not supported.</summary>
    UnsupportedRateKind = 4
}
