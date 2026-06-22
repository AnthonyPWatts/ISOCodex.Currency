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

Tasks 1-10 from the original implementation pack were completed during the NuGet-first release preparation, `0.1.0-alpha.2` default-value safety work, `0.1.0-alpha.3` non-throwing money validation work, `0.1.0-alpha.4` currency data provenance work, `0.1.0-alpha.5` System.Text.Json integration work, and `0.1.0-alpha.6` Countries bridge work, then removed from this remaining-task prompt list.

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


