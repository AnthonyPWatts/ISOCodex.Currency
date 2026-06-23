using System;

namespace ISOCodex.Currency.Validation;

/// <summary>
/// Stable issue codes emitted by the validation package.
/// </summary>
public static class CurrencyValidationIssueCodes
{
    /// <summary>The currency code was missing, malformed, or unknown.</summary>
    public const string CurrencyUnknown = "Currency.Unknown";

    /// <summary>The currency code was the uninitialised default value.</summary>
    public const string CurrencyDefault = "Currency.Default";

    /// <summary>The money amount has more precision than the currency permits.</summary>
    public const string MoneyAmountPrecision = "Money.AmountPrecision";

    /// <summary>The currency does not support applicable minor units.</summary>
    public const string MoneyMinorUnitNotApplicable = "Money.MinorUnitNotApplicable";

    /// <summary>The money conversion overflowed the target representation.</summary>
    public const string MoneyOverflow = "Money.Overflow";

    /// <summary>The money amount was invalid.</summary>
    public const string MoneyInvalidAmount = "Money.InvalidAmount";

    /// <summary>The money text was empty.</summary>
    public const string MoneyParseEmptyInput = "MoneyParse.EmptyInput";

    /// <summary>The money text did not include a required currency.</summary>
    public const string MoneyParseMissingCurrency = "MoneyParse.MissingCurrency";

    /// <summary>The money text included an unknown currency.</summary>
    public const string MoneyParseUnknownCurrency = "MoneyParse.UnknownCurrency";

    /// <summary>The money text included multiple currency candidates.</summary>
    public const string MoneyParseAmbiguousCurrency = "MoneyParse.AmbiguousCurrency";

    /// <summary>The money text currency did not match the expected currency.</summary>
    public const string MoneyParseCurrencyMismatch = "MoneyParse.CurrencyMismatch";

    /// <summary>The money text amount could not be parsed.</summary>
    public const string MoneyParseInvalidAmount = "MoneyParse.InvalidAmount";

    /// <summary>The money text amount has invalid currency precision.</summary>
    public const string MoneyParseAmountPrecision = "MoneyParse.AmountPrecision";

    /// <summary>
    /// Maps a core money validation failure reason to a stable issue code.
    /// </summary>
    public static string FromMoneyValidationFailure(MoneyValidationFailureReason reason)
    {
        switch (reason)
        {
            case MoneyValidationFailureReason.DefaultCurrency:
                return CurrencyDefault;
            case MoneyValidationFailureReason.UnknownCurrency:
                return CurrencyUnknown;
            case MoneyValidationFailureReason.AmountPrecision:
                return MoneyAmountPrecision;
            case MoneyValidationFailureReason.MinorUnitNotApplicable:
                return MoneyMinorUnitNotApplicable;
            case MoneyValidationFailureReason.Overflow:
                return MoneyOverflow;
            case MoneyValidationFailureReason.InvalidAmount:
                return MoneyInvalidAmount;
            case MoneyValidationFailureReason.None:
                throw new ArgumentOutOfRangeException(nameof(reason), "Successful validation results do not have an issue code.");
            default:
                throw new ArgumentOutOfRangeException(nameof(reason), reason, "Unknown money validation failure reason.");
        }
    }

    /// <summary>
    /// Maps a core money parse failure reason to a stable issue code.
    /// </summary>
    public static string FromMoneyParseFailure(MoneyParseFailureReason reason)
    {
        switch (reason)
        {
            case MoneyParseFailureReason.EmptyInput:
                return MoneyParseEmptyInput;
            case MoneyParseFailureReason.MissingCurrency:
                return MoneyParseMissingCurrency;
            case MoneyParseFailureReason.UnknownCurrency:
                return MoneyParseUnknownCurrency;
            case MoneyParseFailureReason.AmbiguousCurrency:
                return MoneyParseAmbiguousCurrency;
            case MoneyParseFailureReason.CurrencyMismatch:
                return MoneyParseCurrencyMismatch;
            case MoneyParseFailureReason.InvalidAmount:
                return MoneyParseInvalidAmount;
            case MoneyParseFailureReason.AmountPrecision:
                return MoneyParseAmountPrecision;
            case MoneyParseFailureReason.None:
                throw new ArgumentOutOfRangeException(nameof(reason), "Successful parse results do not have an issue code.");
            default:
                throw new ArgumentOutOfRangeException(nameof(reason), reason, "Unknown money parse failure reason.");
        }
    }
}
