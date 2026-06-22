# Currency Data Sources

The current data set is a curated pre-1.0 seed, generated from `data/source/currency-data.seed.json` by `scripts/update-currency-data.ps1`.

It is intentionally small and reviewable. It is pinned by `data/source/currency-data.manifest.json`, which records the normalized UTF-8/LF source SHA-256, entry count, checked date, and generated runtime provenance values. It is not yet a complete ISO 4217 list.

## Current generation workflow

From the repository root:

```powershell
pwsh ./scripts/update-currency-data.ps1
dotnet test ISOCodex.Currency.sln --filter CurrencyData
```

Review `data/source/currency-data.seed.json`, `data/source/currency-data.manifest.json`, `src/Currency/Data/CurrencyData.generated.cs`, and `src/Currency/Data/CurrencyDataVersion.cs` together.

## Source provenance

The current seed manifest was checked on 2026-06-22. Source locations were last checked on 2026-06-19:

- ISO describes ISO 4217 as the international standard for alphabetic and numeric currency codes and links to the SIX-hosted XLS/XML code lists.
- SIX identifies itself as the official ISO 4217 Maintenance Agency and publishes current and historical currency code lists online free of charge.
- Unicode CLDR documents supplemental currency data, including `digits`, `rounding`, `cashDigits`, and `cashRounding` attributes.
- The CLDR repository stores this data in `common/supplemental/supplementalData.xml`.

## Future full-data workflow

A later data epic should replace this seed with pinned source downloads:

- SIX ISO 4217 List One XML for current currencies and funds.
- Unicode CLDR `common/supplemental/supplementalData.xml` for fraction, cash digit, and cash rounding metadata.

The generator should merge those source files into the public registry and record exact source dates or upstream revisions.
