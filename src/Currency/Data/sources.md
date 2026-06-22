# Currency Data Sources

The current data set is generated from checked-in SIX ISO 4217 and Unicode CLDR source files.

It is pinned by `data/source/currency-data.manifest.json`, which records normalized UTF-8/LF SHA-256 values for:

- `data/source/currency-data.snapshot.json`
- `data/source/upstream/six/list-one.xml`
- `data/source/upstream/cldr/supplementalData.xml`

The generated registry contains 178 current SIX List One currency/fund codes. CLDR supplies cash fraction metadata and current territory-to-currency relationships where available.

## Current Generation Workflow

From the repository root:

```powershell
pwsh ./scripts/build-currency-data-snapshot.ps1
pwsh ./scripts/update-currency-data.ps1
dotnet test ISOCodex.Currency.sln --filter CurrencyData
```

Review `data/source/currency-data.snapshot.json`, `data/source/currency-data.manifest.json`, `data/source/upstream/`, `src/Currency/Data/CurrencyData.generated.cs`, and `src/Currency/Data/CurrencyDataVersion.cs` together.

## Source Provenance

The current manifest was checked on 2026-06-22.

- SIX identifies itself as the official ISO 4217 Maintenance Agency and publishes current and historical currency code lists online free of charge.
- The checked-in SIX List One XML reports `Pblshd="2026-01-01"`.
- Unicode CLDR documents supplemental currency data, including `digits`, `rounding`, `cashDigits`, and `cashRounding` attributes.
- The CLDR repository stores this data in `common/supplemental/supplementalData.xml`.

This package data is a derived metadata snapshot and is not an official ISO 4217 redistribution.
