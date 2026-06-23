# ISOCodex.Currency.AspNetCore

ASP.NET Core integration helpers for `ISOCodex.Currency`.

## Install

```bash
dotnet add package ISOCodex.Currency.AspNetCore --version 1.0.0
```

This package targets `net9.0` and uses the ASP.NET Core shared framework.

## Register

```csharp
using ISOCodex.Currency.AspNetCore;

builder.Services.AddCurrencyAspNetCore();
```

This registers the default currency registry, `IMoneyFormatter`, `MoneyParser`, and MVC model binding for `CurrencyCode`.

## MVC Model Binding

`CurrencyCode` action parameters and model properties bind from route, query, and form values when MVC model binding is active.

```csharp
app.MapControllerRoute("default", "{controller=Prices}/{action=Get}/{currency?}");
```

Invalid or unknown codes add a model-state error instead of throwing.

## Minimal APIs

`CurrencyCode` already exposes `TryParse(string?, out CurrencyCode)`, so Minimal API route and query binding can use the core type directly.

```csharp
app.MapGet("/currencies/{currency}", (CurrencyCode currency) =>
{
    return Results.Ok(currency.Code);
});
```

For `Money`, keep primitive request DTOs at the boundary and convert explicitly with `Money.TryCreate(...)`.

## Validation Problems

Use the problem-details helpers when invalid money input should become a structured HTTP 400 response.

```csharp
var result = Money.TryCreate(request.Amount, request.Currency);

return result.Succeeded
    ? Results.Ok(result.Money)
    : Results.ValidationProblem(result.ToValidationProblemDictionary("amount"));
```

The helpers preserve the stable failure reason in `ValidationProblemDetails.Extensions["reason"]`.
