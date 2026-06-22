using System;

namespace ISOCodex.Currency.Exchange.Abstractions;

/// <summary>
/// Represents the result of an exchange-rate lookup.
/// </summary>
public sealed class ExchangeRateLookupResult
{
    private ExchangeRateLookupResult(
        bool succeeded,
        ExchangeRate? rate,
        ExchangeRateLookupFailureReason failureReason,
        string? errorMessage)
    {
        Succeeded = succeeded;
        Rate = rate;
        FailureReason = failureReason;
        ErrorMessage = errorMessage;
    }

    /// <summary>Gets whether lookup succeeded.</summary>
    public bool Succeeded { get; }

    /// <summary>Gets the exchange rate when lookup succeeded.</summary>
    public ExchangeRate? Rate { get; }

    /// <summary>Gets the stable failure reason when lookup failed.</summary>
    public ExchangeRateLookupFailureReason FailureReason { get; }

    /// <summary>Gets a human-readable error message when lookup failed.</summary>
    public string? ErrorMessage { get; }

    /// <summary>Creates a successful lookup result.</summary>
    public static ExchangeRateLookupResult Success(ExchangeRate rate)
    {
        if (rate == null)
        {
            throw new ArgumentNullException(nameof(rate));
        }

        return new ExchangeRateLookupResult(true, rate, ExchangeRateLookupFailureReason.None, null);
    }

    /// <summary>Creates a failed lookup result.</summary>
    public static ExchangeRateLookupResult Failure(
        ExchangeRateLookupFailureReason reason,
        string errorMessage)
    {
        if (reason == ExchangeRateLookupFailureReason.None)
        {
            throw new ArgumentException("A failure result must use a specific failure reason.", nameof(reason));
        }

        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException("Failure message must be specified.", nameof(errorMessage));
        }

        return new ExchangeRateLookupResult(false, null, reason, errorMessage);
    }
}
