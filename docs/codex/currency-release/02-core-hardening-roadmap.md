# ISOCodex.Currency Core Hardening Roadmap

Purpose: identify issue-sized improvements to make the core package safer and more attractive after, or just before, the first NuGet prerelease.

This roadmap deliberately avoids optional Countries, Addressing, EF Core, ASP.NET Core, or exchange provider dependencies in the core package.

## Guiding principles

1. Keep `ISOCodex.Currency` independently useful.
2. Keep construction strict and rounding explicit.
3. Add non-throwing APIs for boundaries without removing throwing APIs for trusted code.
4. Make data provenance visible.
5. Avoid hidden global behaviour.
6. Do not infer business rules from locale, country, or symbol without an explicit policy.

## Hardening epic 1 — default-value safety

### Problem

`CurrencyCode` and `Money` are value types. In .NET, value types can be default-initialised even if public construction paths validate input.

Examples:

```csharp
Money money = default;
CurrencyCode currency = default;
```

These values are not meaningful domain values, but they can leak into arrays, model binders, serializers, deserializers, or incorrectly initialised DTOs.

### MVP changes

Add non-breaking APIs:

```csharp
public readonly struct CurrencyCode
{
    public bool IsDefault { get; }
}

public readonly struct Money
{
    public bool IsDefault { get; }
}
```

Recommended semantics:

```csharp
CurrencyCode.IsDefault == string.IsNullOrEmpty(Code)
Money.IsDefault == Currency.IsDefault
```

Add guard helpers internally:

```csharp
private void ThrowIfDefault()
```

Use guards in operations where a default value would otherwise produce confusing errors:

- `Money.ToMinorUnits()`
- `Money.RequireSameCurrency(...)`
- `Money.Add(...)`
- `Money.Subtract(...)`
- `Money.Multiply(...)`
- `Money.Divide(...)`
- `Money.Round(...)`
- `Money.Allocate(...)`
- `Money.CompareTo(...)`

Do not make `default(Money).Equals(default(Money))` throw. Equality and hash code should remain safe.

### Tests

- `default(CurrencyCode).IsDefault` is true.
- `CurrencyCode.GBP.IsDefault` is false.
- `default(Money).IsDefault` is true.
- `Money.Zero(CurrencyCode.GBP).IsDefault` is false.
- operational methods throw a clear `InvalidOperationException` when called on default money.
- equality and hash code remain non-throwing.

### Acceptance criteria

- API is additive and non-breaking.
- Error messages clearly say the value is uninitialised/default.
- README gets a small warning under Money value object.

## Hardening epic 2 — non-throwing money creation and validation

### Problem

The current API is strict and throws on invalid construction. That is suitable for trusted domain code but awkward for boundary workflows such as APIs, imports, CSV files, and partner integrations.

Add a structured non-throwing result pattern for direct amount/currency creation. This should be consistent with the wider ISOCodex style used by `ISOCodex.Countries` and `ISOCodex.Addressing`, where boundary validation returns stable failure reasons rather than forcing ordinary invalid input through exceptions.

### MVP API

Add:

```csharp
public enum MoneyValidationFailureReason
{
    None = 0,
    DefaultCurrency,
    UnknownCurrency,
    AmountPrecision,
    MinorUnitNotApplicable,
    Overflow,
    InvalidAmount,
}

public sealed class MoneyValidationResult
{
    public bool Succeeded { get; }
    public Money? Money { get; }
    public MoneyValidationFailureReason FailureReason { get; }
    public string? ErrorMessage { get; }
}
```

Add static methods:

```csharp
public static MoneyValidationResult TryCreate(decimal amount, CurrencyCode currency);
public static MoneyValidationResult TryCreate(decimal amount, string currencyCode);
public static MoneyValidationResult TryFromMinorUnits(long minorUnits, CurrencyCode currency);
```

Add optional convenience pattern:

```csharp
public static bool TryCreate(decimal amount, CurrencyCode currency, out Money money);
public static bool TryCreate(decimal amount, string currencyCode, out Money money);
```

### Behaviour

- Existing `Money.Of(...)` and `Money.FromMinorUnits(...)` remain strict and throwing.
- New validation APIs catch expected validation failures and return stable reasons.
- Unexpected programming errors should not be swallowed unless intentionally mapped.

### Tests

- valid GBP amount succeeds;
- over-precise GBP amount fails with `AmountPrecision`;
- JPY fractional amount fails with `AmountPrecision`;
- unknown currency string fails with `UnknownCurrency`;
- blank currency string fails with `UnknownCurrency` or `InvalidCurrencySyntax`, if a more granular enum is introduced;
- `XXX` minor-unit conversion fails with a stable reason if minor units are not applicable;
- success result includes exact `Money` value.

### Acceptance criteria

- Boundary code can validate without exception control flow.
- Result type has stable machine-readable reason codes.
- README import/API boundary section is updated.

## Hardening epic 3 — currency data version and provenance

### Problem

The README states that current data is generated from a small checked-in seed rather than a full ISO/CLDR snapshot. That is acceptable for prerelease but must be transparent and machine-readable.

### MVP API

Add:

```csharp
public static class CurrencyDataVersion
{
    public static string Identifier { get; }
    public static DateOnly CheckedOn { get; }
    public static string Description { get; }
    public static string SourceKind { get; }
}
```

For the current seed, values can be explicit:

```text
Identifier: seed-0.1.0-alpha.1
SourceKind: CheckedInSeed
Description: Small checked-in prerelease seed; not a full ISO/CLDR snapshot.
```

### Tests

- version identifier is non-empty;
- checked date is set;
- description mentions current limitation;
- generated data tests assert known package currencies still exist.

### Acceptance criteria

- README exposes `CurrencyDataVersion` in the data workflow section.
- Runtime users can log data provenance.
- No network calls are introduced.

## Hardening epic 4 — currency registry extensibility review

### Problem

`DefaultCurrencyRegistry` can be constructed from supplied metadata, but most APIs use `DefaultCurrencyRegistry.Instance` directly. This makes the core convenient but can limit advanced scenarios.

### MVP approach

Do not rewrite `Money` around service injection. Instead, introduce explicit factory/service types for consumers that need custom registries:

```csharp
public sealed class MoneyFactory
{
    public MoneyFactory(ICurrencyRegistry registry);

    public Money Of(decimal amount, CurrencyCode currency);
    public MoneyValidationResult TryCreate(decimal amount, CurrencyCode currency);
    public Money FromMinorUnits(long minorUnits, CurrencyCode currency);
}
```

Keep static `Money.Of(...)` using `DefaultCurrencyRegistry.Instance` for convenience.

### Tests

- custom registry can create money for a custom test currency;
- static APIs continue to use default registry;
- custom registry does not mutate default registry.

### Acceptance criteria

- Advanced users can avoid global default metadata.
- Simple users keep the existing API.
- README gets a small advanced registry example.

## Hardening epic 5 — rounding and allocation documentation polish

### Problem

Rounding and allocation are already strong features. Their value should be more obvious in the package README and tests.

### Changes

Add docs and tests for:

- tax calculation with explicit midpoint policy;
- percentage discount calculation;
- CHF cash rounding;
- negative money allocation;
- allocation remainder strategy comparisons;
- installment plan edge cases.

### Acceptance criteria

- README shows a compact “why explicit rounding matters” example.
- Tests cover negative allocations and custom increment rounding.
- Extended rigs continue to compile and run.

## Hardening epic 6 — first-class package quality tests

### Add tests/scripts

- `eng/smoke-test-package.ps1`
- optional `eng/smoke-test-package.sh`
- package inspection step in CI;
- test that README examples compile where feasible.

### Acceptance criteria

- package can be consumed from a local source before publish;
- CI uploads package artifacts;
- smoke test uses the packed NuGet package, not a project reference.

## Post-0.1.0 data epic — full auditable currency data snapshot

This is not required for the first NuGet release, but it is the highest credibility improvement after release.

### Target outcome

Replace or augment the small seed with generated, pinned, documented data comparable in discipline to `ISOCodex.Countries`.

### Requirements

- source files checked in or pinned by exact version/hash;
- generated `CurrencyData.generated.cs`;
- generated `CurrencyDataVersion`;
- changelog for data changes;
- tests for every generated entry;
- clear limitation statement that this is not an official ISO redistribution;
- no runtime network calls;
- no loose JSON dependency at runtime.

### Data fields to support

- alpha-3 code;
- numeric code;
- English name;
- standard minor unit;
- cash minor unit;
- cash rounding increment;
- kind;
- tender status;
- related territories as strings in core;
- optional historic validity fields later.

Country-typed territory relationships should live in `ISOCodex.Currency.Countries`, not core.

## Recommended first hardening sequence

1. Package smoke test and release docs.
2. Default value safety.
3. Non-throwing money validation result API.
4. Currency data version/provenance API.
5. Registry/factory extensibility.
6. Documentation polish.
7. Full data generation epic.
