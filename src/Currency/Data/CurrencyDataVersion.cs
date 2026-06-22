using System;

namespace ISOCodex.Currency;

/// <summary>
/// Exposes runtime-visible provenance for the packaged currency data.
/// </summary>
public static class CurrencyDataVersion
{
    /// <summary>Gets the version identifier for the packaged currency data.</summary>
    public static string Identifier { get; } = "seed-0.1.0-alpha.4";

    /// <summary>Gets the date this prerelease seed was checked, expressed as UTC midnight.</summary>
    public static DateTime CheckedOn { get; } = new DateTime(2026, 6, 22, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>Gets a description of the packaged currency data source.</summary>
    public static string Description { get; } = "Small checked-in prerelease seed generated from data/source/currency-data.seed.json; not a full ISO/CLDR snapshot.";

    /// <summary>Gets the kind of source used for the packaged currency data.</summary>
    public static string SourceKind { get; } = "CheckedInSeed";
}
