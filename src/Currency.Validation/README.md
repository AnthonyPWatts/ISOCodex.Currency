# ISOCodex.Currency.Validation

Framework-neutral validation helpers for `ISOCodex.Currency`.

## Install

```bash
dotnet add package ISOCodex.Currency.Validation --version 0.9.0-alpha.12
```

This package targets `netstandard2.1` and does not depend on FluentValidation, ASP.NET Core, or DataAnnotations.

## Boundary Validation

Use `CurrencyBoundaryValidator` for primitive API, import, and integration inputs.

```csharp
using ISOCodex.Currency.Validation;

var validator = new CurrencyBoundaryValidator();
var result = validator.ValidateMoney(12.345m, "GBP", "amount", "currency");

if (!result.IsValid)
{
    var errors = result.ToErrorDictionary();
}
```

## Stable Issue Codes

Validation issues expose stable machine-readable codes and human-readable messages.

```csharp
var issue = result.Issues[0];

Console.WriteLine(issue.Code);         // Money.AmountPrecision
Console.WriteLine(issue.PropertyName); // amount
Console.WriteLine(issue.Message);
```

## Core Result Adapters

Existing core results can be adapted directly.

```csharp
var moneyResult = Money.TryCreate(12.345m, "GBP");
var validationResult = moneyResult.ToCurrencyValidationResult("amount");
```

The package is intentionally small. FluentValidation, DataAnnotations, and framework-specific adapters can build on these stable issue codes without adding those dependencies to the core currency package.
