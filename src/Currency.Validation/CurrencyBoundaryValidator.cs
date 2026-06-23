using System;

namespace ISOCodex.Currency.Validation;

/// <summary>
/// Validates primitive currency and money inputs at API, import, and integration boundaries.
/// </summary>
public sealed class CurrencyBoundaryValidator
{
    private readonly MoneyParser _moneyParser;

    /// <summary>
    /// Creates a boundary validator using the default money parser.
    /// </summary>
    public CurrencyBoundaryValidator()
        : this(new MoneyParser())
    {
    }

    /// <summary>
    /// Creates a boundary validator using an explicit money parser.
    /// </summary>
    public CurrencyBoundaryValidator(MoneyParser moneyParser)
    {
        _moneyParser = moneyParser ?? throw new ArgumentNullException(nameof(moneyParser));
    }

    /// <summary>
    /// Validates that a primitive string contains a registered currency code.
    /// </summary>
    public CurrencyValidationResult ValidateCurrencyCode(string? currencyCode, string propertyName = "currency")
    {
        return CurrencyCode.TryParse(currencyCode, out _)
            ? CurrencyValidationResult.Valid
            : CurrencyValidationResult.Failure(new CurrencyValidationIssue(
                CurrencyValidationIssueCodes.CurrencyUnknown,
                $"Currency code '{currencyCode}' is not a registered ISO 4217-style currency code.",
                propertyName));
    }

    /// <summary>
    /// Validates a decimal amount and primitive currency string as a money value.
    /// </summary>
    public CurrencyValidationResult ValidateMoney(
        decimal amount,
        string? currencyCode,
        string amountPropertyName = "amount",
        string currencyPropertyName = "currency")
    {
        var result = Money.TryCreate(amount, currencyCode ?? string.Empty);
        return result.ToCurrencyValidationResult(GetMoneyPropertyName(result, amountPropertyName, currencyPropertyName));
    }

    /// <summary>
    /// Validates integer minor units and a primitive currency string as a money value.
    /// </summary>
    public CurrencyValidationResult ValidateMinorUnits(
        long minorUnits,
        string? currencyCode,
        string minorUnitsPropertyName = "minorUnits",
        string currencyPropertyName = "currency")
    {
        if (!CurrencyCode.TryParse(currencyCode, out var parsedCurrency))
        {
            return CurrencyValidationResult.Failure(new CurrencyValidationIssue(
                CurrencyValidationIssueCodes.CurrencyUnknown,
                $"Currency code '{currencyCode}' is not a registered ISO 4217-style currency code.",
                currencyPropertyName));
        }

        var result = Money.TryFromMinorUnits(minorUnits, parsedCurrency);
        return result.ToCurrencyValidationResult(GetMinorUnitsPropertyName(result, minorUnitsPropertyName, currencyPropertyName));
    }

    /// <summary>
    /// Validates a formatted money string using the configured parser.
    /// </summary>
    public CurrencyValidationResult ValidateMoneyText(
        string? input,
        string propertyName = "money",
        MoneyParseOptions? options = null)
    {
        return _moneyParser.Parse(input, options).ToCurrencyValidationResult(propertyName);
    }

    private static string GetMoneyPropertyName(
        MoneyValidationResult result,
        string amountPropertyName,
        string currencyPropertyName)
    {
        return result.FailureReason == MoneyValidationFailureReason.UnknownCurrency
            || result.FailureReason == MoneyValidationFailureReason.DefaultCurrency
            ? currencyPropertyName
            : amountPropertyName;
    }

    private static string GetMinorUnitsPropertyName(
        MoneyValidationResult result,
        string minorUnitsPropertyName,
        string currencyPropertyName)
    {
        return result.FailureReason == MoneyValidationFailureReason.UnknownCurrency
            || result.FailureReason == MoneyValidationFailureReason.DefaultCurrency
            ? currencyPropertyName
            : minorUnitsPropertyName;
    }
}
