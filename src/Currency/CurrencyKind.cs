namespace ISOCodex.Currency;

/// <summary>
/// Describes the broad kind of a currency metadata entry.
/// </summary>
public enum CurrencyKind
{
    /// <summary>A currency issued for general national or regional use.</summary>
    NationalCurrency,

    /// <summary>A fund or accounting unit.</summary>
    Fund,

    /// <summary>A precious-metal unit represented in ISO 4217-style data.</summary>
    PreciousMetal,

    /// <summary>A testing currency code.</summary>
    Testing,

    /// <summary>An explicit no-currency entry.</summary>
    NoCurrency,

    /// <summary>A currency kind that is not known by the registry.</summary>
    Unknown
}
