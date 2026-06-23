using System;
using System.Collections.Generic;
using System.Linq;

namespace ISOCodex.Currency.Validation;

/// <summary>
/// Represents framework-neutral validation output for currency and money boundary input.
/// </summary>
public sealed class CurrencyValidationResult
{
    private static readonly IReadOnlyList<CurrencyValidationIssue> NoIssues = Array.Empty<CurrencyValidationIssue>();

    private CurrencyValidationResult(IReadOnlyList<CurrencyValidationIssue> issues)
    {
        Issues = issues;
    }

    /// <summary>Gets a successful validation result.</summary>
    public static CurrencyValidationResult Valid { get; } = new CurrencyValidationResult(NoIssues);

    /// <summary>Gets whether validation succeeded.</summary>
    public bool IsValid => Issues.Count == 0;

    /// <summary>Gets validation issues.</summary>
    public IReadOnlyList<CurrencyValidationIssue> Issues { get; }

    /// <summary>
    /// Creates a failed validation result from one issue.
    /// </summary>
    public static CurrencyValidationResult Failure(CurrencyValidationIssue issue)
    {
        if (issue == null)
        {
            throw new ArgumentNullException(nameof(issue));
        }

        return new CurrencyValidationResult(new[] { issue });
    }

    /// <summary>
    /// Creates a validation result from a collection of issues.
    /// </summary>
    public static CurrencyValidationResult FromIssues(IEnumerable<CurrencyValidationIssue> issues)
    {
        if (issues == null)
        {
            throw new ArgumentNullException(nameof(issues));
        }

        var issueList = issues.ToArray();
        return issueList.Length == 0
            ? Valid
            : new CurrencyValidationResult(issueList);
    }

    /// <summary>
    /// Converts issues to a property-keyed dictionary suitable for APIs, imports, and UI boundaries.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> ToErrorDictionary(string defaultPropertyName = "")
    {
        return Issues
            .GroupBy(issue => issue.PropertyName ?? defaultPropertyName, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.Select(issue => issue.Message).ToArray(),
                StringComparer.Ordinal);
    }
}
