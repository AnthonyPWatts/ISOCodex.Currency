# Codex Task Prompts for ISOCodex.Currency

Use these as copy-paste implementation prompts. Run one task at a time.

General constraints for every task:

- Keep `ISOCodex.Currency` core independent of `ISOCodex.Countries` and `ISOCodex.Addressing` unless the task explicitly creates a bridge package.
- Preserve existing public APIs unless the task explicitly says otherwise.
- Prefer additive changes while the package remains pre-1.0.
- Keep rounding explicit.
- Do not infer currency from symbols, country, locale, or address without an explicit policy.
- Do not create a Commerce module.
- Run relevant tests and report exact commands/results.

---

Tasks 1-6 from the original implementation pack were completed during the NuGet-first release preparation and `0.1.0-alpha.2` default-value safety work, then removed from this remaining-task prompt list.

---

## Task 7 — add non-throwing Money validation API

### Prompt

```text
You are working in AnthonyPWatts/ISOCodex.Currency.

Add non-throwing Money creation APIs for API/import boundaries.

Context:
- Money.Of currently throws for invalid amount precision or currency.
- Currency currently needs a structured, non-throwing result pattern for API/import boundaries.
- Follow the wider ISOCodex style: stable machine-readable failure reasons, human-readable messages, and no exceptions for ordinary invalid boundary input.

Implement:
1. Add MoneyValidationFailureReason enum.
2. Add MoneyValidationResult type.
3. Add Money.TryCreate(decimal amount, CurrencyCode currency) returning MoneyValidationResult.
4. Add Money.TryCreate(decimal amount, string currencyCode) returning MoneyValidationResult.
5. Add bool TryCreate overloads if they fit existing style.
6. Add Money.TryFromMinorUnits(long minorUnits, CurrencyCode currency) returning MoneyValidationResult.
7. Keep Money.Of and Money.FromMinorUnits behaviour unchanged.
8. Add tests for valid input, over-precision, unknown currency, JPY precision, minor-unit-not-applicable cases.
9. Update README import/API boundary section.

Run:
- dotnet test ISOCodex.Currency.sln -c Release

Report:
- API surface;
- result reason codes;
- tests added.
```

### Acceptance criteria

- boundary code can validate without exception control flow.
- existing APIs are preserved.
- result reason codes are stable and documented.

---

## Task 8 — add CurrencyDataVersion

### Prompt

```text
You are working in AnthonyPWatts/ISOCodex.Currency.

Add runtime-visible currency data provenance.

Context:
- README says current data is generated from a small checked-in seed, not a full ISO/CLDR snapshot.
- This limitation should be visible at runtime and in docs.

Implement:
1. Add public static CurrencyDataVersion class.
2. Include Identifier, CheckedOn, Description, and SourceKind properties.
3. Values should truthfully describe the current checked-in seed.
4. Add tests that these properties are populated and mention the seed/source limitation.
5. Update README data workflow section with example usage.

Do not:
- fetch data at runtime;
- claim official ISO authority;
- replace the data generation pipeline in this task.

Run:
- dotnet test ISOCodex.Currency.sln -c Release

Report:
- API;
- exact values;
- tests.
```

### Acceptance criteria

- users can log data version/provenance.
- docs remain truthful.
- no runtime network calls.

---

## Task 9 — create System.Text.Json integration package

### Prompt

```text
You are working in AnthonyPWatts/ISOCodex.Currency.

Create an optional System.Text.Json integration package.

Package:
- ISOCodex.Currency.Json.SystemTextJson

Constraints:
- Do not add System.Text.Json dependency to the core package unless unavoidable.
- Do not change Money core semantics.
- Default JSON shape for Money should be { "amount": 12.34, "currency": "GBP" }.
- CurrencyCode should serialize as "GBP".
- Deserialization should reject invalid currency and over-precise amount by default.
- Do not silently round.
- Do not infer from symbols.

Implement:
1. Add new project under src/Currency.Json.SystemTextJson or equivalent repo convention.
2. Add tests under tests/Currency.Json.SystemTextJson.Tests or equivalent.
3. Add converters for CurrencyCode and Money.
4. Add options only if needed; keep MVP simple.
5. Add README section for registration.
6. Add project to solution.

Run:
- dotnet restore ISOCodex.Currency.sln
- dotnet build ISOCodex.Currency.sln -c Release --no-restore
- dotnet test ISOCodex.Currency.sln -c Release --no-build

Report:
- package/project names;
- JSON shapes;
- tests.
```

### Acceptance criteria

- converters round-trip valid values.
- invalid values fail predictably.
- core package remains independent.

---

## Task 10 — create Currency.Countries bridge package skeleton

### Prompt

```text
You are working in AnthonyPWatts/ISOCodex.Currency.

Create the initial ISOCodex.Currency.Countries bridge package skeleton.

Context:
- ISOCodex.Countries owns country codes, country metadata, aliases, display names, subdivisions, and special code elements.
- Currency must not recreate country-code types.
- This is a bridge package, not a core dependency.

Implement:
1. Add new project: ISOCodex.Currency.Countries.
2. Reference ISOCodex.Currency and ISOCodex.Countries.
3. Add CountryCurrencyInfo with CountryAlpha2Code and CurrencyCode.
4. Add CountryCurrencyRole enum.
5. Add ICountryCurrencyRegistry.
6. Add DefaultCountryCurrencyRegistry with a small explicit seed for common countries/currencies sufficient for tests: GB/GBP, US/USD, IE/EUR, JP/JPY, CH/CHF, CA/CAD, AU/AUD, NZ/NZD.
7. Add CountryCurrencyValidationPolicy with PrimaryTenderOnly and AnyLegalTender presets.
8. Add CountryCurrencyValidationResult and reason enum.
9. Add tests.
10. Add README documentation that this is an initial seed and not official geopolitical authority.
11. Do not add Addressing dependency.
12. Add project to solution.

Run:
- dotnet restore ISOCodex.Currency.sln
- dotnet build ISOCodex.Currency.sln -c Release --no-restore
- dotnet test ISOCodex.Currency.sln -c Release --no-build

Report:
- package API;
- dependencies;
- validation examples;
- tests.
```

### Acceptance criteria

- bridge package compiles and tests.
- no country-code duplication.
- no dependency from core Currency to Countries.

---

## Task 11 — create initial analyzer project spike

### Prompt

```text
You are working in AnthonyPWatts/ISOCodex.Currency.

Create an initial analyzer package spike for default Money/CurrencyCode misuse.

Package:
- ISOCodex.Currency.Analyzers

Implement only the first rule unless the repo already has analyzer infrastructure.

Rule:
- ISOCCUR001: Avoid default(Money). Use Money.Zero(currency) or Money.Of(amount, currency).

Optional second rule:
- ISOCCUR002: Avoid default(CurrencyCode). Parse, TryParse, or use a known static code.

Constraints:
- Analyzer package must be separate from core.
- Do not add analyzer dependencies to runtime package.
- Include analyzer tests.
- If analyzer infrastructure is too large for one PR, create a minimal compiling project and document next steps.

Run:
- dotnet build
- analyzer tests if added

Report:
- analyzer IDs;
- diagnostics implemented;
- code fixes if any;
- known limitations.
```

### Acceptance criteria

- analyzer project compiles.
- at least one diagnostic is covered by tests.
- runtime package is unaffected.

---

## Task 12 — create exchange abstractions spike

### Prompt

```text
You are working in AnthonyPWatts/ISOCodex.Currency.

Create a provider-neutral deterministic exchange-rate abstractions package.

Package:
- ISOCodex.Currency.Exchange.Abstractions

Constraints:
- No live exchange-rate provider.
- No network calls.
- Conversion must require explicit rounding.
- Conversion result must include audit information.
- Core Currency package must not depend on exchange abstractions unless intentionally reviewed.

Implement:
1. CurrencyPair record struct.
2. ExchangeRate type.
3. ExchangeRateKind enum.
4. IExchangeRateProvider.
5. ExchangeRateLookupResult.
6. ConversionOptions requiring target currency, effective date, rate kind, and rounding policy.
7. MoneyConverter that supports direct rate only for MVP.
8. ConversionResult with input, output, rate, raw amount, rounded amount, and audit fields.
9. Tests for deterministic direct conversion and explicit rounding.
10. README examples.

Run:
- dotnet restore ISOCodex.Currency.sln
- dotnet build ISOCodex.Currency.sln -c Release --no-restore
- dotnet test ISOCodex.Currency.sln -c Release --no-build

Report:
- API;
- deterministic behaviour;
- tests.
```

### Acceptance criteria

- no network dependency.
- conversion is auditable.
- rounding is explicit.


