namespace ISOCodex.Currency.Countries;

/// <summary>
/// Represents the result of validating a country/currency association.
/// </summary>
public sealed class CountryCurrencyValidationResult
{
    private CountryCurrencyValidationResult(
        bool succeeded,
        CountryCurrencyInfo? countryCurrency,
        CountryCurrencyValidationFailureReason failureReason,
        string? errorMessage)
    {
        Succeeded = succeeded;
        CountryCurrency = countryCurrency;
        FailureReason = failureReason;
        ErrorMessage = errorMessage;
    }

    /// <summary>Gets whether validation succeeded.</summary>
    public bool Succeeded { get; }

    /// <summary>Gets the matching country/currency association when validation succeeded.</summary>
    public CountryCurrencyInfo? CountryCurrency { get; }

    /// <summary>Gets the stable machine-readable failure reason.</summary>
    public CountryCurrencyValidationFailureReason FailureReason { get; }

    /// <summary>Gets a human-readable validation message when validation failed.</summary>
    public string? ErrorMessage { get; }

    internal static CountryCurrencyValidationResult Success(CountryCurrencyInfo countryCurrency)
    {
        return new CountryCurrencyValidationResult(true, countryCurrency, CountryCurrencyValidationFailureReason.None, null);
    }

    internal static CountryCurrencyValidationResult Failure(
        CountryCurrencyValidationFailureReason reason,
        string errorMessage)
    {
        return new CountryCurrencyValidationResult(false, null, reason, errorMessage);
    }
}
