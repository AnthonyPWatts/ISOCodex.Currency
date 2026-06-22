# BulkMoneyImportTool

Console POC for importing mixed-currency money data from CSV, validating rows, applying explicit rounding rules where requested, and producing review outputs.

## Run

```bash
dotnet run --project ExtendedTestRigs/BulkMoneyImportTool -- SampleData/money-import.csv
```

Outputs are written under the built app's `Output` directory:

- `import-results.csv`
- `import-summary.txt`

## Input columns

| Column | Notes |
| --- | --- |
| `Reference` | Free-text row identifier. |
| `Amount` | Decimal amount. Use invariant decimal notation in the sample. |
| `Currency` | ISO 4217-style alpha-3 code. |
| `Rounding` | `None`, `Standard`, `AwayFromZero`, or `Cash`. |

## Features exercised

- Currency code parsing.
- Registry lookup for metadata.
- Currency-specific precision validation.
- Explicit standard, away-from-zero, and cash rounding.
- Exact minor-unit conversion for accepted rows.
- Import review output that separates accepted, rounded, and rejected rows.

## Known limitations

- The CSV reader is intentionally small and only supports simple single-line records.
- Some import paths still map domain exceptions into review messages instead of modelling every case through structured validation results.
- Output files are written beside the compiled application, not back into source.
