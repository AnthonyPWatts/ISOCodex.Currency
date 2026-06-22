namespace ISOCodex.Currency.Exchange.Abstractions;

/// <summary>
/// Describes the business meaning of an exchange rate.
/// </summary>
public enum ExchangeRateKind
{
    /// <summary>No rate kind was specified.</summary>
    Unspecified = 0,

    /// <summary>A spot rate.</summary>
    Spot = 1,

    /// <summary>A mid-market rate.</summary>
    MidMarket = 2,

    /// <summary>A buy-side rate.</summary>
    Buy = 3,

    /// <summary>A sell-side rate.</summary>
    Sell = 4,

    /// <summary>An accounting or internally approved rate.</summary>
    Accounting = 5
}
