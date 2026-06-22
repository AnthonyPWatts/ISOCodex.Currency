namespace ISOCodex.Currency;

/// <summary>
/// Represents the result of non-throwing money validation and creation.
/// </summary>
public sealed class MoneyValidationResult
{
    private MoneyValidationResult(
        bool succeeded,
        Money? money,
        MoneyValidationFailureReason failureReason,
        string? errorMessage)
    {
        Succeeded = succeeded;
        Money = money;
        FailureReason = failureReason;
        ErrorMessage = errorMessage;
    }

    /// <summary>Gets whether validation succeeded.</summary>
    public bool Succeeded { get; }

    /// <summary>Gets the created money value when validation succeeded.</summary>
    public Money? Money { get; }

    /// <summary>Gets the stable machine-readable failure reason.</summary>
    public MoneyValidationFailureReason FailureReason { get; }

    /// <summary>Gets a human-readable validation message when validation failed.</summary>
    public string? ErrorMessage { get; }

    internal static MoneyValidationResult Success(Money money)
    {
        return new MoneyValidationResult(true, money, MoneyValidationFailureReason.None, null);
    }

    internal static MoneyValidationResult Failure(MoneyValidationFailureReason reason, string errorMessage)
    {
        return new MoneyValidationResult(false, null, reason, errorMessage);
    }
}
