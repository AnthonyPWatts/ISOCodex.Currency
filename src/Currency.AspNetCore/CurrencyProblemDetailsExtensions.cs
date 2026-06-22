using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ISOCodex.Currency.AspNetCore;

/// <summary>
/// Converts currency validation and parse failures into ASP.NET Core validation problem details.
/// </summary>
public static class CurrencyProblemDetailsExtensions
{
    /// <summary>
    /// Converts a failed money validation result into a dictionary suitable for <c>Results.ValidationProblem(...)</c>.
    /// </summary>
    public static IDictionary<string, string[]> ToValidationProblemDictionary(
        this MoneyValidationResult result,
        string fieldName = "money")
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (result.Succeeded)
        {
            throw new InvalidOperationException("Cannot create validation errors for a successful money validation result.");
        }

        return CreateValidationProblemDictionary(fieldName, result.ErrorMessage, result.FailureReason.ToString());
    }

    /// <summary>
    /// Converts a failed money parse result into a dictionary suitable for <c>Results.ValidationProblem(...)</c>.
    /// </summary>
    public static IDictionary<string, string[]> ToValidationProblemDictionary(
        this MoneyParseResult result,
        string fieldName = "money")
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (result.Succeeded)
        {
            throw new InvalidOperationException("Cannot create validation errors for a successful money parse result.");
        }

        return CreateValidationProblemDictionary(fieldName, result.ErrorMessage, result.FailureReason.ToString());
    }

    /// <summary>
    /// Converts a failed money validation result into ASP.NET Core <see cref="ValidationProblemDetails"/>.
    /// </summary>
    public static ValidationProblemDetails ToValidationProblemDetails(
        this MoneyValidationResult result,
        string fieldName = "money")
    {
        var errors = result.ToValidationProblemDictionary(fieldName);
        var details = new ValidationProblemDetails(errors)
        {
            Title = "Currency validation failed.",
            Status = StatusCodes.Status400BadRequest
        };
        details.Extensions["reason"] = result.FailureReason.ToString();
        return details;
    }

    /// <summary>
    /// Converts a failed money parse result into ASP.NET Core <see cref="ValidationProblemDetails"/>.
    /// </summary>
    public static ValidationProblemDetails ToValidationProblemDetails(
        this MoneyParseResult result,
        string fieldName = "money")
    {
        var errors = result.ToValidationProblemDictionary(fieldName);
        var details = new ValidationProblemDetails(errors)
        {
            Title = "Money parse failed.",
            Status = StatusCodes.Status400BadRequest
        };
        details.Extensions["reason"] = result.FailureReason.ToString();
        return details;
    }

    private static IDictionary<string, string[]> CreateValidationProblemDictionary(
        string fieldName,
        string? errorMessage,
        string reason)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException("Field name must be supplied.", nameof(fieldName));
        }

        var message = string.IsNullOrWhiteSpace(errorMessage)
            ? reason
            : errorMessage;

        return new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            [fieldName] = new[] { message }
        };
    }
}
