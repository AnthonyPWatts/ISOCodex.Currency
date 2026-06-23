using System;

namespace ISOCodex.Currency.Validation;

/// <summary>
/// Converts core currency result types into framework-neutral validation results.
/// </summary>
public static class CurrencyValidationResultExtensions
{
    /// <summary>
    /// Converts a core <see cref="MoneyValidationResult"/> into a framework-neutral validation result.
    /// </summary>
    public static CurrencyValidationResult ToCurrencyValidationResult(
        this MoneyValidationResult result,
        string propertyName = "money")
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (result.Succeeded)
        {
            return CurrencyValidationResult.Valid;
        }

        return CurrencyValidationResult.Failure(new CurrencyValidationIssue(
            CurrencyValidationIssueCodes.FromMoneyValidationFailure(result.FailureReason),
            result.ErrorMessage ?? result.FailureReason.ToString(),
            propertyName));
    }

    /// <summary>
    /// Converts a core <see cref="MoneyParseResult"/> into a framework-neutral validation result.
    /// </summary>
    public static CurrencyValidationResult ToCurrencyValidationResult(
        this MoneyParseResult result,
        string propertyName = "money")
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (result.Succeeded)
        {
            return CurrencyValidationResult.Valid;
        }

        return CurrencyValidationResult.Failure(new CurrencyValidationIssue(
            CurrencyValidationIssueCodes.FromMoneyParseFailure(result.FailureReason),
            result.ErrorMessage ?? result.FailureReason.ToString(),
            propertyName));
    }
}
