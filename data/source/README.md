# Currency Data Source Snapshot

This folder contains the checked-in source snapshot used by `scripts/update-currency-data.ps1` to generate `src/Currency/Data/CurrencyData.generated.cs`.

## Files

- `currency-data.seed.json` is a small curated seed for the current pre-1.0 package. It mirrors the generated data in reviewable JSON form.
- `currency-data.manifest.json` pins the seed by SHA-256, entry count, checked date, and generated runtime provenance values.

## Provenance

The seed is not yet a complete ISO 4217 data set. It is a pinned curated subset based on:

- ISO 4217-style alpha code, numeric code, currency name, and minor-unit metadata.
- CLDR supplemental currency fraction metadata where it affects decimal places or cash rounding.
- Manual territory metadata for the currently included entries.

## Update workflow

From the repository root:

```powershell
pwsh ./scripts/update-currency-data.ps1
dotnet test ISOCodex.Currency.sln --filter CurrencyData
```

The generator verifies the manifest hash and entry count before writing generated source.

Review `data/source/currency-data.seed.json`, `data/source/currency-data.manifest.json`, `src/Currency/Data/CurrencyData.generated.cs`, and `src/Currency/Data/CurrencyDataVersion.cs` in the same change.

## Future full-data workflow

A later data epic should replace this seed with pinned source downloads:

- SIX ISO 4217 List One XML for current currencies and funds.
- Unicode CLDR `common/supplemental/supplementalData.xml` for fraction, cash digit, and cash rounding metadata.

The generator should then merge those source files into the public registry and record exact source dates or upstream revisions.
