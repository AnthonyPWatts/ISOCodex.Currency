namespace ISOCodex.Currency.Addressing;

/// <summary>
/// Identifies which delegated validator produced an address/currency issue.
/// </summary>
public enum AddressCurrencyValidationIssueSource
{
    /// <summary>No validation issue source.</summary>
    None = 0,

    /// <summary>The issue came from address validation.</summary>
    Address = 1,

    /// <summary>The issue came from country/currency validation.</summary>
    CountryCurrency = 2
}
