# ISOCodex.Currency Killer Features Backlog

Purpose: convert the strategic “Newtonsoft.Json of Currency” ambition into five implementation epics that are attractive across enterprise and SME use cases.

This backlog incorporates the existence of:

- `ISOCodex.Countries`
- `ISOCodex.Addressing`

It deliberately does **not** prioritise a Commerce module at this stage.

## Product positioning

For Currency alone:

> `ISOCodex.Currency` is the safe money and currency foundation for .NET applications: typed, strict, explicit, boundary-friendly, and integration-ready.

For the wider ISOCodex stack:

> ISOCodex provides typed, deterministic foundations for international business data that is usually mishandled as strings: countries, subdivisions, addresses, currencies, and money.

## Killer feature 1 — ISOCodex data graph

### Outcome

A developer can ask typed questions across countries, addresses, and currencies without stringly typed joins or duplicated metadata.

### Why it matters

Most business systems eventually need to relate:

- billing address country;
- shipping address country;
- customer country;
- supplier country;
- invoice currency;
- payment currency;
- settlement currency;
- cash rounding;
- legal tender / primary currency status;
- import/reporting historical context.

Competitor money libraries generally stop at money/currency. ISOCodex can provide an ecosystem-level data graph.

### Package ownership

```text
ISOCodex.Countries
    country/subdivision/display/alias source of truth

ISOCodex.Addressing
    address profile/validator/formatter source of truth

ISOCodex.Currency
    money/currency/minor-unit/rounding source of truth

ISOCodex.Currency.Countries
    country/currency relationship source of truth

ISOCodex.Currency.Addressing
    address/currency boundary validation helpers
```

### MVP

Create `ISOCodex.Currency.Countries` with:

- `CountryCurrencyInfo`
- `CountryCurrencyRole`
- `ICountryCurrencyRegistry`
- `DefaultCountryCurrencyRegistry`
- `CountryCurrencyValidationPolicy`
- `CountryCurrencyValidationResult`
- small checked-in seed for initial country/currency mappings
- data version/provenance class

### Example API

```csharp
CountryAlpha2Code country = CountryAlpha2Code.Parse("GB");

IReadOnlyList<CountryCurrencyInfo> currencies =
    registry.GetCurrenciesForCountry(country);

CountryCurrencyValidationResult result = registry.Validate(
    country,
    CurrencyCode.GBP,
    CountryCurrencyValidationPolicy.PrimaryTenderOnly);
```

### Acceptance criteria

- bridge package depends on Currency and Countries only;
- no country-code duplication;
- no address dependency in `ISOCodex.Currency.Countries`;
- validation result exposes stable reason codes;
- data limitations are documented.

## Killer feature 2 — boundary integration pack

### Outcome

A developer can use Currency naturally at the edges of .NET applications:

- JSON APIs;
- ASP.NET Core endpoints;
- EF Core persistence;
- Dapper data access;
- FluentValidation/DataAnnotations;
- CSV/import pipelines;
- OpenAPI contracts.

### Why it matters

A money library becomes ubiquitous when it is easy to use at boundaries. Newtonsoft.Json won because it solved real serialization edges. Currency should win by solving money/currency edges.

### MVP sequence

1. `ISOCodex.Currency.Json.SystemTextJson`
2. `ISOCodex.Currency.Json.NewtonsoftJson`
3. `ISOCodex.Currency.EntityFrameworkCore`
4. `ISOCodex.Currency.AspNetCore`
5. `ISOCodex.Currency.Validation`
6. `ISOCodex.Currency.Dapper`

### JSON default

Use explicit object shape:

```json
{
  "amount": 12.34,
  "currency": "GBP"
}
```

Do not infer from symbols. Do not silently round.

### EF default

Prefer separate columns:

```sql
Amount decimal(19,4) not null,
CurrencyCode char(3) not null
```

or exact minor-unit storage:

```sql
MinorUnits bigint not null,
CurrencyCode char(3) not null
```

### ASP.NET default

Invalid boundary input should become structured validation/problem-details output, not unhandled exceptions.

### Acceptance criteria

- every integration has a compiling sample;
- JSON and persistence shapes are stable and documented;
- no integration package changes core behaviour implicitly;
- all packages preserve strict precision validation by default.

## Killer feature 3 — named monetary policy engine

### Outcome

Teams can define named, reusable money policies for checkout, imports, reporting, cash handling, and payments without scattering rounding and validation rules throughout application code.

### Why it matters

Enterprise systems need repeatable policy, not ad hoc helper methods.

### Core concepts

```csharp
public sealed class MonetaryPolicy
{
    public string Name { get; }
    public CurrencyRoundingPolicy DefaultRounding { get; }
    public AllocationRemainderStrategy AllocationRemainderStrategy { get; }
    public bool RejectOverPrecision { get; }
    public bool AllowNegativeAmounts { get; }
    public IReadOnlySet<CurrencyCode>? AllowedCurrencies { get; }
}
```

Named registration:

```csharp
services.AddCurrencyPolicy("Checkout", policy => policy
    .RejectOverPrecision()
    .DisallowNegativeAmounts()
    .UseRounding(CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven))
    .AllowCurrencies("GBP", "EUR", "USD"));
```

Country-aware policy belongs in `ISOCodex.Currency.Countries`:

```csharp
services.AddCurrencyPolicy("UK Checkout", policy => policy
    .UseCountry(CountryAlpha2Code.Parse("GB"))
    .AllowPrimaryTenderCurrencyOnly()
    .RejectOverPrecision());
```

Address-aware policy belongs in `ISOCodex.Currency.Addressing`:

```csharp
services.AddAddressCurrencyPolicy("Billing", policy => policy
    .RequireAddressValidation()
    .RequireCurrencyAcceptedForAddressCountry());
```

### MVP

Start framework-neutral:

- `MonetaryPolicy`
- `MonetaryPolicyBuilder`
- `MonetaryPolicyRegistry`
- `MoneyPolicyValidationResult`
- built-in presets:
  - `Strict`
  - `Checkout`
  - `AccountingImport`
  - `CashRegister`
  - `PaymentGateway`

### Acceptance criteria

- policies are explicit and testable;
- no ambient global policy changes static `Money.Of(...)` behaviour;
- policy-based creation returns structured results;
- README explains when to use static `Money` APIs vs policy APIs.

## Killer feature 4 — deterministic exchange-rate abstractions

### Outcome

Support currency conversion in a deterministic, auditable, provider-neutral way without making live exchange rates a core responsibility.

### Why it matters

Many systems need conversion, but enterprise finance systems need known rate sources, effective dates, replayability, and audit trails. Random live-rate calls inside domain logic are not acceptable.

### Package

```text
ISOCodex.Currency.Exchange.Abstractions
```

Optional later:

```text
ISOCodex.Currency.Exchange.Fixed
ISOCodex.Currency.Exchange.Csv
ISOCodex.Currency.Exchange.Json
```

### MVP API

```csharp
public readonly record struct CurrencyPair(CurrencyCode BaseCurrency, CurrencyCode QuoteCurrency);

public sealed class ExchangeRate
{
    public CurrencyPair Pair { get; }
    public decimal Rate { get; }
    public DateOnly EffectiveDate { get; }
    public string Source { get; }
    public ExchangeRateKind Kind { get; }
}

public interface IExchangeRateProvider
{
    ExchangeRateLookupResult TryGetRate(CurrencyPair pair, DateOnly effectiveDate, ExchangeRateKind kind);
}

public sealed class MoneyConverter
{
    public ConversionResult Convert(Money source, CurrencyCode targetCurrency, ConversionOptions options);
}
```

### Conversion result

Should include:

- input money;
- target currency;
- direct/inverse/triangulated path;
- rate source;
- rate effective date;
- raw converted amount;
- rounding policy;
- final money value.

### Acceptance criteria

- no network calls in abstractions package;
- conversion requires explicit rounding;
- direct and inverse rate tests;
- deterministic replay test;
- audit trail is available as structured data.

## Killer feature 5 — compiler safety and high-throughput money types

### Outcome

Make Currency attractive to large codebases and high-volume systems through analyzers and fast exact representations.

### Analyzer package

```text
ISOCodex.Currency.Analyzers
```

### Baseline analyzer rules

```text
ISOCCUR001 Avoid default(Money). Use Money.Zero(currency) or Money.Of(amount, currency).
ISOCCUR002 Avoid default(CurrencyCode). Parse, TryParse, or use a known static code.
ISOCCUR003 Do not ignore MoneyValidationResult, and do not ignore MoneyParseResult.
ISOCCUR004 Do not store formatted money strings as identifiers or persistence values.
ISOCCUR005 Do not infer currency from a symbol without an expected currency.
ISOCCUR006 Do not perform money multiplication/division without explicit rounding policy.
```

Cross-package analyzers later:

```text
ISOCCUR020 Endpoint accepts country and currency but does not validate their relationship.
ISOADDRCUR001 Address.CountryCode and Money.Currency are both present but no address/currency policy is applied.
```

### High-throughput types

Add later, after core API settles:

```csharp
public readonly struct MinorMoney
{
    public long MinorUnits { get; }
    public CurrencyCode Currency { get; }
}
```

Potential later:

```csharp
public readonly struct ScaledMoney
{
    public long Units { get; }
    public int Scale { get; }
    public CurrencyCode Currency { get; }
}
```

### Acceptance criteria

- analyzers ship as a separate package;
- code fixes for default value rules;
- `MinorMoney` round-trips with `Money` where the currency supports minor units;
- conversion failures are explicit and checked;
- benchmarks exist before claiming performance benefits.

## Priority map

### Now: NuGet readiness

- CI/release workflows.
- Package smoke test.
- Accurate README/changelog/release gate.
- First prerelease publish.

### Next: core safety

- documentation polish.

### Then: adoption features

- EF Core helpers;
- ASP.NET Core helpers;
- validation adapters.

### Then: ISOCodex ecosystem features

- `ISOCodex.Currency.Countries`;
- `ISOCodex.Currency.Addressing`.

### Later: advanced enterprise features

- named monetary policy engine;
- exchange abstractions;
- analyzers;
- high-throughput money types.
