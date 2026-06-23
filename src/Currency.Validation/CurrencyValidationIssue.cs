using System;

namespace ISOCodex.Currency.Validation;

/// <summary>
/// Represents a stable validation issue for currency and money boundary input.
/// </summary>
public sealed class CurrencyValidationIssue
{
    /// <summary>
    /// Creates a validation issue.
    /// </summary>
    public CurrencyValidationIssue(string code, string message, string? propertyName = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Issue code must be supplied.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Issue message must be supplied.", nameof(message));
        }

        Code = code;
        Message = message;
        PropertyName = string.IsNullOrWhiteSpace(propertyName) ? null : propertyName;
    }

    /// <summary>Gets the stable machine-readable issue code.</summary>
    public string Code { get; }

    /// <summary>Gets the human-readable validation message.</summary>
    public string Message { get; }

    /// <summary>Gets the associated property name when one is known.</summary>
    public string? PropertyName { get; }
}
