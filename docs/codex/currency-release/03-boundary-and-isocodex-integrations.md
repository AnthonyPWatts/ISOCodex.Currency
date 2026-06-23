# Boundary and ISOCodex Integration Roadmap

Purpose: define optional packages and bridge packages that make `ISOCodex.Currency` attractive in real .NET applications while keeping the core package small.

The user is not prioritising a Commerce module at this stage. Do not create `ISOCodex.Commerce` for now.

## Package boundary principles

1. `ISOCodex.Currency` must remain usable without `ISOCodex.Countries` or `ISOCodex.Addressing`.
2. Country and address concepts must come from existing ISOCodex packages, not be recreated in Currency.
3. Framework-specific integrations should be optional packages.
4. Serialization and persistence should be explicit, documented, and tested.
5. Boundary packages should prefer structured validation results over exception-driven invalid-input handling.

## Recommended package graph

```text
ISOCodex.Currency
    Core money and currency package.

ISOCodex.Currency.Json.SystemTextJson
    System.Text.Json converters for CurrencyCode and Money.

ISOCodex.Currency.Json.NewtonsoftJson
    Newtonsoft.Json converters for CurrencyCode and Money.

ISOCodex.Currency.EntityFrameworkCore
    EF Core value converters and owned-type mapping helpers.

ISOCodex.Currency.AspNetCore
    Model binding, minimal API binding, problem-details helpers, OpenAPI schema helpers.

ISOCodex.Currency.Dapper
    Dapper type handlers. Initial currency-code handler implemented in `0.9.0-alpha.11`.

ISOCodex.Currency.Validation
    Framework-neutral validation adapters plus optional FluentValidation/DataAnnotations helpers.

ISOCodex.Currency.Countries
    Country/currency relationships using ISOCodex.Countries.

ISOCodex.Currency.Addressing
    Address/currency validation helpers using ISOCodex.Addressing and ISOCodex.Countries.

ISOCodex.Currency.Exchange.Abstractions
    Provider-neutral, deterministic exchange-rate abstractions. Initial direct-rate package implemented in `0.1.0-alpha.8`.
```

## Integration package 1 — System.Text.Json

Status: implemented in `0.1.0-alpha.5`.

### Package

```text
ISOCodex.Currency.Json.SystemTextJson
```

### Dependencies

- `ISOCodex.Currency`
- `System.Text.Json` only if required by target framework/package design

### Scope

Provide converters for:

- `CurrencyCode`
- `Money`

### Supported wire shapes

Default money shape:

```json
{
  "amount": 12.34,
  "currency": "GBP"
}
```

Optional configured shapes:

```json
"GBP 12.34"
```

```json
{
  "minorUnits": 1234,
  "currency": "GBP"
}
```

```json
{
  "amount": "12.34",
  "currency": "GBP"
}
```

### API sketch

```csharp
public sealed class CurrencyCodeJsonConverter : JsonConverter<CurrencyCode>
{
}

public sealed class MoneyJsonConverter : JsonConverter<Money>
{
    public MoneyJsonConverter(MoneyJsonOptions? options = null);
}

public sealed class MoneyJsonOptions
{
    public MoneyJsonShape Shape { get; init; } = MoneyJsonShape.ObjectAmountAndCurrency;
    public bool WriteAmountAsString { get; init; }
    public CurrencyRoundingPolicy? RoundingPolicyForOverPrecision { get; init; }
}
```

### Rules

- Default deserialization rejects invalid currency codes.
- Default deserialization rejects over-precise amounts.
- Do not infer currency from symbols.
- Do not silently round unless a rounding option is explicitly configured.
- Default values should be handled intentionally and documented.

### Acceptance criteria

- converters round-trip valid `CurrencyCode` and `Money`;
- invalid currency fails with useful `JsonException`;
- over-precise amount fails with useful `JsonException`;
- README includes registration example;
- package has tests against `JsonSerializerOptions`.

## Integration package 2 — Newtonsoft.Json

Status: implemented in `0.9.0-alpha.5`.

### Package

```text
ISOCodex.Currency.Json.NewtonsoftJson
```

### Dependencies

- `ISOCodex.Currency`
- `Newtonsoft.Json`

### Scope

Mirror System.Text.Json behaviour as closely as possible for enterprise/legacy systems.

### Acceptance criteria

- same default wire shape as System.Text.Json package;
- same validation semantics;
- equivalent documentation examples.

## Integration package 3 — Entity Framework Core

Status: initial implementation in `0.9.0-alpha.8`.

### Package

```text
ISOCodex.Currency.EntityFrameworkCore
```

### Dependencies

- `ISOCodex.Currency`
- `Microsoft.EntityFrameworkCore.Relational` 10.x

### Scope

Support two relational persistence shapes:

#### Amount + currency code

```sql
Amount decimal(19,4) not null,
CurrencyCode char(3) not null
```

#### Minor units + currency code

```sql
MinorUnits bigint not null,
CurrencyCode char(3) not null
```

The `0.9.0-alpha.8` package provides a `Money` complex-property helper for the amount+currency-code shape. Exact minor-unit storage should be modelled explicitly by the application as `long MinorUnits` plus `CurrencyCode CurrencyCode`, then converted at the boundary with `Money.FromMinorUnits(...)` or `Money.TryFromMinorUnits(...)`.

### API sketch

```csharp
modelBuilder.Entity<Order>(entity =>
{
    entity.ComplexMoney(
        x => x.Total,
        amountColumn: "TotalAmount",
        currencyColumn: "TotalCurrency");
});
```

Alternative converter-only API:

```csharp
builder.Property(x => x.Currency)
    .HasConversion(CurrencyCodeValueConverter.Instance)
    .HasMaxLength(3)
    .IsUnicode(false);
```

### Rules

- Do not recommend storing symbols.
- Do not recommend formatted money strings.
- Do not apply country-specific database constraints.
- Preserve currency code uppercase.

### Acceptance criteria

- sample DbContext compiles;
- relational schema produces expected amount and currency-code columns;
- round-trip tests with SQLite provider;
- docs show amount+currency helper usage and the explicit minorUnits+currency storage choice.

## Integration package 4 — ASP.NET Core

Status: initial implementation in `0.9.0-alpha.9`.

### Package

```text
ISOCodex.Currency.AspNetCore
```

### Dependencies

- `ISOCodex.Currency`
- `Microsoft.AspNetCore.*` as appropriate

### Scope

- MVC model binding for `CurrencyCode`;
- Minimal API use of the core `CurrencyCode.TryParse(...)` binding shape;
- problem-details helpers for `MoneyValidationResult` and `MoneyParseResult` failures;
- DI registration for implemented services, formatters, parsers, and MVC binding.

OpenAPI schema helpers remain future work.

### API sketch

```csharp
builder.Services
    .AddCurrencyAspNetCore();
```

Example endpoint:

```csharp
app.MapPost("/quote", (QuoteRequest request, ICurrencyBoundaryValidator validator) =>
{
    var result = validator.Validate(request);
    return result.Succeeded
        ? Results.Ok(result.Value)
        : Results.ValidationProblem(result.ToProblemDetails());
});
```

### Acceptance criteria

- Minimal API sample compiles using `CurrencyCode`;
- invalid currency produces structured 400 response;
- validation-problem helpers preserve stable failure reasons;
- no hidden rounding.

## Integration package 5 — validation adapters

Status: initial framework-neutral implementation in `0.9.0-alpha.10`.

### Package

```text
ISOCodex.Currency.Validation
```

### Scope

Start framework-neutral, then optionally provide adapters.

Core types:

```csharp
public sealed class CurrencyValidationIssue
{
    public string Code { get; }
    public string Message { get; }
    public string? PropertyName { get; }
}

public sealed class CurrencyValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<CurrencyValidationIssue> Issues { get; }
}
```

Possible adapters:

- DataAnnotations attributes;
- FluentValidation extension methods;
- ASP.NET ModelState helpers.

The `0.9.0-alpha.10` package provides the framework-neutral issue/result types, stable issue codes, primitive boundary validator, and adapters from core money validation/parse results. DataAnnotations, FluentValidation, and ASP.NET-specific adapters remain future work.

### Acceptance criteria

- stable issue codes;
- import/API-friendly errors;
- no dependency on FluentValidation in the core Currency package.

## Bridge package — ISOCodex.Currency.Countries

### Package

```text
ISOCodex.Currency.Countries
```

### Dependencies

- `ISOCodex.Currency`
- `ISOCodex.Countries`

### Purpose

Validate and explain country/currency relationships without duplicating country metadata.

### Do not implement here

- address validation;
- payment-gateway support tables;
- tax rules;
- accounting rules;
- live exchange rates.

### API sketch

```csharp
public sealed class CountryCurrencyInfo
{
    public CountryAlpha2Code Country { get; }
    public CurrencyCode Currency { get; }
    public CountryCurrencyRole Role { get; }
    public bool IsLegalTender { get; }
    public DateOnly? ValidFrom { get; }
    public DateOnly? ValidTo { get; }
    public string? Source { get; }
}

public enum CountryCurrencyRole
{
    Primary,
    Secondary,
    Parallel,
    Historic,
    NonTender,
    SpecialPurpose,
    Unknown
}
```

Registry:

```csharp
public interface ICountryCurrencyRegistry
{
    IReadOnlyList<CountryCurrencyInfo> GetCurrenciesForCountry(CountryAlpha2Code country);
    IReadOnlyList<CountryCurrencyInfo> GetCountriesForCurrency(CurrencyCode currency);
    CountryCurrencyValidationResult Validate(CountryAlpha2Code country, CurrencyCode currency, CountryCurrencyValidationPolicy policy);
}
```

Policies:

```csharp
public sealed class CountryCurrencyValidationPolicy
{
    public static CountryCurrencyValidationPolicy PrimaryTenderOnly { get; }
    public static CountryCurrencyValidationPolicy AnyLegalTender { get; }
    public static CountryCurrencyValidationPolicy ReportingOnly { get; }
    public static CountryCurrencyValidationPolicy HistoricForDate(DateOnly date);
}
```

Result reasons:

```csharp
public enum CountryCurrencyValidationFailureReason
{
    None,
    CountryUnknown,
    CurrencyUnknown,
    CurrencyNotKnownForCountry,
    CurrencyNotLegalTenderForCountry,
    CurrencyHistoricForCountry,
    CurrencyNotPrimaryForCountry,
    PolicyRejected
}
```

### Initial data strategy

Start with a conservative checked-in seed and visible data version. Do not claim official geopolitical or ISO authority.

Future version can generate richer mappings from pinned sources.

### Acceptance criteria

- uses `CountryAlpha2Code` from `ISOCodex.Countries`;
- uses `CurrencyCode` from `ISOCodex.Currency`;
- no duplicate country-code structs;
- no hidden network calls;
- validation result has stable reason codes;
- README includes checkout/import examples.

## Bridge package — ISOCodex.Currency.Addressing

### Package

```text
ISOCodex.Currency.Addressing
```

### Dependencies

- `ISOCodex.Currency`
- `ISOCodex.Countries`
- `ISOCodex.Addressing`
- `ISOCodex.Currency.Countries`

### Purpose

Provide address/currency boundary validation without becoming a Commerce module.

### Scope

Validate combinations such as:

```csharp
Address billingAddress;
Money total;
```

under explicit policy:

```csharp
public sealed class AddressCurrencyValidationPolicy
{
    public bool RequireAddressToBeValid { get; init; }
    public CountryCurrencyValidationPolicy CountryCurrencyPolicy { get; init; }
}
```

API sketch:

```csharp
public sealed class AddressCurrencyValidator
{
    public AddressCurrencyValidationResult Validate(
        Address address,
        Money money,
        AddressCurrencyValidationPolicy policy);
}
```

### Do not implement here

- tax calculation;
- payment processing;
- fraud checks;
- checkout orchestration;
- shipping-rate calculation;
- deliverability checks.

### Acceptance criteria

- address country is read from `Address.CountryCode`;
- address validation delegates to `ISOCodex.Addressing` validators;
- country/currency relationship delegates to `ISOCodex.Currency.Countries`;
- structured result can report address issues and currency-country issues together.

## Integration priority

Recommended after first NuGet deploy:

1. `ISOCodex.Currency.Json.SystemTextJson`
2. `ISOCodex.Currency.Json.NewtonsoftJson` - implemented in `0.9.0-alpha.5`.
3. `ISOCodex.Currency.EntityFrameworkCore` - initial implementation in `0.9.0-alpha.8`.
4. `ISOCodex.Currency.AspNetCore` - initial implementation in `0.9.0-alpha.9`.
5. `ISOCodex.Currency.Validation` - initial framework-neutral implementation in `0.9.0-alpha.10`.
6. `ISOCodex.Currency.Dapper` - initial currency-code handler implemented in `0.9.0-alpha.11`.
7. `ISOCodex.Currency.Countries`
8. `ISOCodex.Currency.Addressing`
9. `ISOCodex.Currency.Exchange.Abstractions` - initial direct-rate package implemented in `0.1.0-alpha.8`.
10. `ISOCodex.Currency.Analyzers` - initial rule implemented in `0.1.0-alpha.7`.

## Documentation examples to add over time

### API boundary example

```csharp
public sealed record PriceInput(decimal Amount, string Currency);

var result = Money.TryCreate(input.Amount, input.Currency);

if (!result.Succeeded)
{
    return Results.ValidationProblem(result.ToProblemDetails());
}
```

### Country/currency example

```csharp
CountryAlpha2Code country = CountryAlpha2Code.Parse("GB");
CurrencyCode currency = CurrencyCode.Parse("GBP");

var result = countryCurrencyRegistry.Validate(
    country,
    currency,
    CountryCurrencyValidationPolicy.PrimaryTenderOnly);
```

### Address/currency example

```csharp
var result = addressCurrencyValidator.Validate(
    billingAddress,
    orderTotal,
    AddressCurrencyValidationPolicy.CheckoutDefault);
```
