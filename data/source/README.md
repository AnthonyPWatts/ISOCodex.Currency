# Currency Data Source Snapshot

This folder contains the checked-in source snapshot used by `scripts/update-currency-data.ps1` to generate `src/Currency/Data/CurrencyData.generated.cs`.

## Files

- `currency-data.seed.json` is a small curated seed for the current pre-1.0 package. It mirrors the generated data in reviewable JSON form.

## Provenance

The seed is not yet a complete ISO 4217 data set. It is a curated subset based on:

- ISO 4217-style alpha code, numeric code, currency name, and minor-unit metadata.
- CLDR supplemental currency fraction metadata where it affects decimal places or cash rounding.
- Manual territory metadata for the currently included entries.

## Update workflow

From the repository root:

```powershell
pwsh ./scripts/update-currency-data.ps1
dotnet test ISOCodex.Currency.sln --filter CurrencyData
```

Review both `data/source/currency-data.seed.json` and `src/Currency/Data/CurrencyData.generated.cs` in the same change.

## Future full-data workflow

A later data epic should replace this seed with pinned source downloads:

- SIX ISO 4217 List One XML for current currencies and funds.
- Unicode CLDR `common/supplemental/supplementalData.xml` for fraction, cash digit, and cash rounding metadata.

The generator should then merge those source files into the public registry and record exact source dates or upstream revisions.
