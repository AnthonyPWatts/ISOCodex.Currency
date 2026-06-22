using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ISOCodex.Currency.AspNetCore;

/// <summary>
/// Binds ASP.NET Core MVC action parameters and model properties to <see cref="CurrencyCode"/>.
/// </summary>
public sealed class CurrencyCodeModelBinder : IModelBinder
{
    /// <inheritdoc />
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;
        if (string.IsNullOrWhiteSpace(value))
        {
            if (AllowsNull(bindingContext))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Currency code is required.");
            return Task.CompletedTask;
        }

        if (!CurrencyCode.TryParse(value, out var currencyCode))
        {
            bindingContext.ModelState.TryAddModelError(
                bindingContext.ModelName,
                $"Currency code '{value}' is not a registered ISO 4217-style currency code.");
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(currencyCode);
        return Task.CompletedTask;
    }

    private static bool AllowsNull(ModelBindingContext bindingContext)
    {
        return Nullable.GetUnderlyingType(bindingContext.ModelMetadata.ModelType) != null
            || bindingContext.ModelMetadata.IsReferenceOrNullableType;
    }
}
