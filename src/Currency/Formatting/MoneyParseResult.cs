using System;

namespace ISOCodex.Currency;

/// <summary>
/// Represents the result of parsing a money value.
/// </summary>
public sealed class MoneyParseResult
{
    private MoneyParseResult(Money? money, MoneyParseFailureReason failureReason, string? errorMessage)
    {
        Money = money;
        FailureReason = failureReason;
        ErrorMessage = errorMessage;
    }

    /// <summary>Gets whether parsing succeeded.</summary>
    public bool Succeeded => FailureReason == MoneyParseFailureReason.None;

    /// <summary>Gets the parsed money value when parsing succeeded.</summary>
    public Money? Money { get; }

    /// <summary>Gets the failure reason when parsing failed.</summary>
    public MoneyParseFailureReason FailureReason { get; }

    /// <summary>Gets a human-readable parsing error.</summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Creates a successful parse result.
    /// </summary>
    public static MoneyParseResult Success(Money money)
    {
        return new MoneyParseResult(money, MoneyParseFailureReason.None, null);
    }

    /// <summary>
    /// Creates a failed parse result.
    /// </summary>
    public static MoneyParseResult Failure(MoneyParseFailureReason failureReason, string errorMessage)
    {
        if (failureReason == MoneyParseFailureReason.None)
        {
            throw new ArgumentOutOfRangeException(nameof(failureReason), "Failure results must include a failure reason.");
        }

        return new MoneyParseResult(null, failureReason, errorMessage);
    }
}
