# Changelog

## 1.0.2

Patch release containing fixes committed after `1.0.1`.

Includes:

- Exchange conversion now honours explicit currency registries when creating converted money values for custom target currencies.
- System.Text.Json money deserialisation now rejects invalid amount and currency token types with `JsonException`.
- EF Core currency-code conversion now rejects default `CurrencyCode` values instead of storing an empty string.
- Analyzer release tracking now records the shipped diagnostics in `AnalyzerReleases.Shipped.md`.
- `1.0.1` post-publication housekeeping is complete, including package install verification, NuGet README verification, GitHub release notes, and recorded symbol-package evidence.
- Package smoke testing now follows the current `ISOCodex.Addressing` country-code API.

## 1.0.1

Corrects package README and release-status documentation after the stable release.

Includes:

- Package README wording no longer describes the core package as pre-1.0.
- Release notes, release gate, and package status docs now reflect the stable `1.0.x` package line.

## 1.0.0

First stable release of the Currency package family.

Includes:

- Stable package versioning for the core, bridge, integration, exchange-abstraction, and analyzer packages.
- `ISOCodex.Currency.Countries` and `ISOCodex.Currency.Addressing` now depend on stable `ISOCodex.Countries` `1.0.0`.
- Release-gate, package README, and smoke-test defaults updated for `1.0.0`.

## 0.9.0-alpha.15

Adds symbol-parsing analyzer coverage.

Includes:

- `ISOCCUR005`, warning when `MoneyParseOptions` enables symbol parsing without an expected currency.
- Analyzer tests covering symbol-only and code-or-symbol parsing with and without expected currencies.
- Analyzer README updates for the symbol-parsing rule.
- Versioned release-gate, package README, and smoke-test defaults for `0.9.0-alpha.15`.

## 0.9.0-alpha.14

Adds ignored-result analyzer coverage for money boundary APIs.

Includes:

- `ISOCCUR003`, warning when `MoneyValidationResult` or `MoneyParseResult` values are ignored as expression statements or explicit discards.
- Analyzer tests covering ignored validation results, ignored parse results, explicit discards, inspected results, and bool `TryCreate(...)` overloads.
- Analyzer README updates for all three shipped analyzer rules.
- Versioned release-gate, package README, and smoke-test defaults for `0.9.0-alpha.14`.

## 0.9.0-alpha.13

Adds a second compiler-safety analyzer rule.

Includes:

- `ISOCCUR002`, warning on `default(CurrencyCode)` and `default` literals converted to `CurrencyCode`.
- Analyzer tests covering explicit and inferred default `CurrencyCode` values.
- Analyzer README updates for both default-value rules.
- Versioned release-gate, package README, and smoke-test defaults for `0.9.0-alpha.13`.

## 0.9.0-alpha.12

Adds optional Addressing integration.

Includes:

- `ISOCodex.Currency.Addressing` targeting `netstandard2.1`.
- `AddressCurrencyValidator` for combining `ISOCodex.Addressing` address validation with `ISOCodex.Currency.Countries` country/currency policy validation.
- `AddressCurrencyValidationPolicy`, `AddressCurrencyValidationResult`, and stable bridge issue types.
- Tests covering valid address/currency combinations, combined address and country/currency failures, missing addresses, and missing address validators.
- Package smoke-test and publish-workflow coverage for the Addressing bridge package.

## 0.9.0-alpha.11

Adds optional Dapper integration.

Includes:

- `ISOCodex.Currency.Dapper` targeting `netstandard2.1`.
- `CurrencyCodeTypeHandler` for mapping `CurrencyCode` values to uppercase alpha-3 database strings.
- `DapperCurrencyTypeHandlers.Register()` startup helper.
- Tests covering Dapper parameter writing, database value parsing, and invalid database values.
- Package smoke-test and publish-workflow coverage for the Dapper package.

## 0.9.0-alpha.10

Adds optional framework-neutral validation helpers.

Includes:

- `ISOCodex.Currency.Validation` targeting `netstandard2.1`.
- Stable `CurrencyValidationIssue` and `CurrencyValidationResult` types for API/import boundaries.
- `CurrencyBoundaryValidator` for primitive currency, amount/currency, minor-unit, and formatted money inputs.
- Adapters from `MoneyValidationResult` and `MoneyParseResult` to validation results.
- Package smoke-test and publish-workflow coverage for the validation package.

## 0.9.0-alpha.9

Adds optional ASP.NET Core integration.

Includes:

- `ISOCodex.Currency.AspNetCore` targeting `net9.0`.
- MVC model binding for `CurrencyCode` route, query, and form values.
- `AddCurrencyAspNetCore()` registration for currency registry, formatter, parser, and MVC binding.
- Problem-details helpers for failed `MoneyValidationResult` and `MoneyParseResult` values.
- Package smoke-test and publish-workflow coverage for the ASP.NET Core package.

## 0.9.0-alpha.8

Adds optional Entity Framework Core integration.

Includes:

- `ISOCodex.Currency.EntityFrameworkCore` targeting `net10.0`.
- `CurrencyCodeValueConverter` and `HasCurrencyCodeConversion()` for uppercase alpha-3 currency-code columns.
- `ComplexMoney(...)` for mapping `Money` as separate amount and currency-code columns.
- SQLite-backed integration tests covering converter behaviour, schema shape, and round-tripping.
- Package smoke-test and publish-workflow coverage for the new EF Core package.

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
