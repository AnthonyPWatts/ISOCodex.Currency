# Currency Data Source Snapshot

This folder contains the checked-in source snapshot used to generate `src/Currency/Data/CurrencyData.generated.cs`.

## Files

- `currency-data.snapshot.json` is the derived reviewable snapshot consumed by `scripts/update-currency-data.ps1`.
- `currency-data.manifest.json` pins the derived snapshot and raw source files by normalized UTF-8/LF SHA-256.
- `upstream/six/list-one.xml` is the checked-in SIX ISO 4217 List One XML source.
- `upstream/cldr/supplementalData.xml` is the checked-in Unicode CLDR supplemental data source.

## Provenance

The derived snapshot is generated from:

- SIX ISO 4217 List One XML for current currency and funds code, numeric code, English name, and standard minor-unit metadata.
- Unicode CLDR supplemental currency data for cash fraction metadata and current territory-to-currency relationships.

This package data is derived metadata and is not an official ISO 4217 redistribution.

## Update workflow

From the repository root:

```powershell
pwsh ./scripts/build-currency-data-snapshot.ps1
pwsh ./scripts/update-currency-data.ps1
dotnet test ISOCodex.Currency.sln --filter CurrencyData
```

`build-currency-data-snapshot.ps1` merges the checked-in upstream XML files into `currency-data.snapshot.json`.
`update-currency-data.ps1` verifies the manifest hashes and entry count before writing generated source.

Review `currency-data.snapshot.json`, `currency-data.manifest.json`, `upstream/`, `src/Currency/Data/CurrencyData.generated.cs`, and `src/Currency/Data/CurrencyDataVersion.cs` in the same change.
