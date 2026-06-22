# Changelog

## 0.9.0-alpha.7

Replaces the curated currency seed with a pinned SIX ISO 4217 and Unicode CLDR data snapshot.

Includes:

- Checked-in SIX ISO 4217 List One XML and Unicode CLDR supplemental currency data source files.
- A generated 178-entry `currency-data.snapshot.json` source for packaged registry generation.
- Manifest pinning for the derived snapshot and raw upstream sources by normalized UTF-8/LF SHA-256.
- Generated `CurrencyDataVersion` provenance for the full source snapshot.
- Tests proving every SIX List One code is represented and CLDR cash metadata is applied.

## 0.9.0-alpha.6

Adds README example compile coverage for the package surface.

Includes:

- `Currency.ReadmeExamples.Tests` covering the README quick start, rounding, allocation, JSON, country, exchange, advanced registry, and data-provenance examples.
- Solution-level test coverage for optional package examples that consume the public APIs together.
- Versioned release-gate, package README, and smoke-test defaults for `0.9.0-alpha.6`.

## 0.9.0-alpha.5

Adds optional Newtonsoft.Json integration.

Includes:

- `ISOCodex.Currency.Json.NewtonsoftJson`.
- `CurrencyCodeJsonConverter`, serialising currency codes as alpha-3 strings.
- `MoneyJsonConverter`, serialising money as `{ "amount": 12.34, "currency": "GBP" }`.
- Strict deserialisation for invalid currency codes, over-precise money amounts, missing properties, and default values.
- Package smoke-test coverage for both JSON integration packages.

## 0.9.0-alpha.4

Adds auditable currency data source provenance.

Includes:

- Checked-in source manifest with SHA-256 and entry-count pinning for the generated currency seed.
- `scripts/update-currency-data.ps1` verification of the manifest before generated data is written.
- Generated `CurrencyDataVersion` values from the source manifest.
- Tests proving the manifest, source seed, generated registry data, and runtime provenance stay aligned.

## 0.9.0-alpha.2

Polishes rounding, allocation, and installment documentation and tests.

Includes:

- Tax and percentage-discount examples that show explicit midpoint rounding decisions.
- Custom-increment `Money.Round(...)` test coverage.
- Side-by-side allocation remainder strategy tests.
- Fixed-first installment edge-case coverage for configured remainder placement.
- README examples for explicit rounding and remainder strategies.

## 0.9.0-alpha.1

Adds explicit registry-backed money creation.

Includes:

- `CurrencyCode.CreateCustom(...)` for custom alpha-3 codes used with explicit registries.
- `MoneyFactory` for `Of(...)`, `TryCreate(...)`, `FromMinorUnits(...)`, and `TryFromMinorUnits(...)` using a supplied `ICurrencyRegistry`.
- Tests proving custom registries do not mutate the default registry and static `Money` APIs continue to use packaged metadata.
- Documentation for advanced registry scenarios.

## 0.1.0-alpha.8

Adds optional `ISOCodex.Currency.Exchange.Abstractions` package.

Includes:

- `CurrencyPair` direct base/quote rate pair.
- `ExchangeRate`, `ExchangeRateKind`, `IExchangeRateProvider`, and `ExchangeRateLookupResult`.
- `ConversionOptions` requiring target currency, effective date, rate kind, and explicit rounding policy.
- `MoneyConverter` direct-rate conversion only for the MVP.
- `ConversionResult` with input, output, direct rate, raw amount, rounded amount, requested effective date, requested rate kind, rounding policy, and rate source.
- Tests proving deterministic direct conversion, explicit rounding, no inverse conversion, and no core dependency on the exchange abstractions package.

## 0.1.0-alpha.7

Adds optional `ISOCodex.Currency.Analyzers` package.

Includes:

- `ISOCCUR001`, warning on `default(Money)` and `default` literals converted to `Money`.
- Analyzer package layout under `analyzers/dotnet/cs`.
- Analyzer tests using in-memory Roslyn compilation.

## 0.1.0-alpha.6

Adds optional `ISOCodex.Currency.Countries` bridge package.

Includes:

- `CountryCurrencyInfo`.
- `CountryCurrencyRole`.
- `ICountryCurrencyRegistry`.
- `DefaultCountryCurrencyRegistry` with a small prerelease seed: GB/GBP, US/USD, IE/EUR, JP/JPY, CH/CHF, CA/CAD, AU/AUD, and NZ/NZD.
- `CountryCurrencyValidationPolicy` with `PrimaryTenderOnly` and `AnyLegalTender` presets.
- `CountryCurrencyValidationResult` and `CountryCurrencyValidationFailureReason`.
- Tests that the core package does not depend on `ISOCodex.Countries`.

## 0.1.0-alpha.5

Adds optional System.Text.Json integration package.

Includes:

- `ISOCodex.Currency.Json.SystemTextJson`.
- `CurrencyCodeJsonConverter`, serialising currency codes as alpha-3 strings.
- `MoneyJsonConverter`, serialising money as `{ "amount": 12.34, "currency": "GBP" }`.
- Strict deserialisation for invalid currency codes, over-precise money amounts, missing properties, and default values.
- CI, publish workflow, and smoke-test updates for multi-package releases.

## 0.1.0-alpha.4

Adds runtime-visible currency data provenance.

Includes:

- `CurrencyDataVersion.Identifier`.
- `CurrencyDataVersion.CheckedOn`.
- `CurrencyDataVersion.Description`.
- `CurrencyDataVersion.SourceKind`.
- Tests that the data provenance describes the checked-in seed and preserves known registry entries.

## 0.1.0-alpha.3

Adds non-throwing money validation APIs for API and import boundaries.

Includes:

- `MoneyValidationFailureReason`.
- `MoneyValidationResult`.
- `Money.TryCreate(decimal, CurrencyCode)`.
- `Money.TryCreate(decimal, string)`.
- bool `Money.TryCreate(...)` convenience overloads.
- `Money.TryFromMinorUnits(long, CurrencyCode)`.
- Tests for valid input, over-precision, unknown currencies, default currencies, JPY precision, and minor-unit-not-applicable failures.

## 0.1.0-alpha.2

Adds default-value safety APIs for `CurrencyCode` and `Money`.

Includes:

- `CurrencyCode.IsDefault`.
- `Money.IsDefault`.
- Clear default-value guard errors for operational `Money` methods.
- Tests that equality and hash code remain safe for default `Money` values.

## 0.1.0-alpha.1

Initial public prerelease of `ISOCodex.Currency`.

Includes:

- `CurrencyCode` value object for registered ISO 4217-style alpha-3 codes.
- `CurrencyInfo` metadata and default registry.
- Immutable `Money` value object with currency-specific precision validation.
- Explicit standard, cash, custom decimal-place, and custom-increment rounding policies.
- Exact minor-unit conversion where the currency defines applicable minor units.
- Same-currency arithmetic guardrails.
- Money allocation and installment helpers.
- Culture-aware money formatting with explicit currency display choices.
- Conservative money parsing with structured parse failure reasons.
- Extended examples for CSV import and checkout pricing.
- Repeatable currency data seed generation workflow.

Known limitations:

- Currency data is generated from a small checked-in seed rather than a full ISO/CLDR snapshot.
- No JSON converters yet.
- No Entity Framework Core helpers yet.
- No exchange-rate abstractions yet.
