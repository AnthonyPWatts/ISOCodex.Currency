using System;

namespace ISOCodex.Currency.Addressing;

/// <summary>
/// Represents a stable issue produced while validating an address and currency combination.
/// </summary>
public sealed class AddressCurrencyValidationIssue
{
    /// <summary>
    /// Creates an address/currency validation issue.
    /// </summary>
    public AddressCurrencyValidationIssue(
        AddressCurrencyValidationIssueSource source,
        string code,
        string message,
        string? propertyName = null)
    {
        if (source == AddressCurrencyValidationIssueSource.None)
        {
            throw new ArgumentOutOfRangeException(nameof(source), source, "Validation issue source must be specified.");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Issue code cannot be null or empty.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Issue message cannot be null or empty.", nameof(message));
        }

        Source = source;
        Code = code;
        Message = message;
        PropertyName = propertyName;
    }

    /// <summary>Gets the delegated validator that produced the issue.</summary>
    public AddressCurrencyValidationIssueSource Source { get; }

    /// <summary>Gets the stable machine-readable issue code.</summary>
    public string Code { get; }

    /// <summary>Gets the human-readable issue message.</summary>
    public string Message { get; }

    /// <summary>Gets the optional property name associated with the issue.</summary>
    public string? PropertyName { get; }
}
