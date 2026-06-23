# Extended test rigs

These projects are deliberately consumer-shaped examples for `ISOCodex.Currency`. They are not unit tests. They exist to exercise the package from application edges where API roughness, metadata gaps, documentation gaps, and awkward money-handling patterns are easier to spot.

## Projects

- `BulkMoneyImportTool` is a console import tool that reads mixed-currency CSV rows, validates currency/precision, applies optional rounding, and writes review outputs.
- `CheckoutPricingApi` is an ASP.NET Core Minimal API for a small checkout-pricing workflow that calculates subtotal, tax, and total with explicit rounding.

Both projects use local project references rather than NuGet package references.

## Smoke test

From the repository root:

```bash
dotnet build ISOCodex.Currency.sln
dotnet test
dotnet run --project ExtendedTestRigs/BulkMoneyImportTool -- SampleData/money-import.csv
dotnet run --project ExtendedTestRigs/CheckoutPricingApi --urls http://localhost:5000
```

For the API rig, use `CheckoutPricingApi/CheckoutPricingApi.http` or call the endpoints directly.

