namespace ISOCodex.Currency;

/// <summary>
/// Identifies a stable reason why a money value could not be created.
/// </summary>
public enum MoneyValidationFailureReason
{
    /// <summary>No validation failure occurred.</summary>
    None = 0,

    /// <summary>The supplied currency code was the uninitialised default value.</summary>
    DefaultCurrency,

    /// <summary>The supplied currency code is not registered.</summary>
    UnknownCurrency,

    /// <summary>The amount has more fractional precision than the currency permits.</summary>
    AmountPrecision,

    /// <summary>The currency does not define applicable minor units for the requested operation.</summary>
    MinorUnitNotApplicable,

    /// <summary>The operation overflowed the target numeric representation.</summary>
    Overflow,

    /// <summary>The amount was not valid for money creation.</summary>
    InvalidAmount,
}
