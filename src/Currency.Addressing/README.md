# ISOCodex.Currency.Addressing

Address/currency validation helpers for `ISOCodex.Currency` and `ISOCodex.Addressing`.

## Install

```bash
dotnet add package ISOCodex.Currency.Addressing --version 1.0.2
```

This package targets `netstandard2.1` and depends on `ISOCodex.Addressing`, `ISOCodex.Countries`, and `ISOCodex.Currency.Countries`.

## Address/Currency Validation

Use `AddressCurrencyValidator` when an application needs to validate both an address and the selected payment/reporting currency.

```csharp
using ISOCodex.Currency;
using ISOCodex.Currency.Addressing;

var validator = new AddressCurrencyValidator(addressValidatorFactory);
var result = validator.Validate(
    billingAddress,
    CurrencyCode.GBP,
    AddressCurrencyValidationPolicy.CheckoutDefault);

if (!result.Succeeded)
{
    foreach (var issue in result.Issues)
    {
        Console.WriteLine($"{issue.Source}: {issue.Code}");
    }
}
```

The package delegates address validation to `ISOCodex.Addressing` and country/currency validation to `ISOCodex.Currency.Countries`. It does not calculate tax, shipping, payment acceptance, fraud risk, or deliverability.
