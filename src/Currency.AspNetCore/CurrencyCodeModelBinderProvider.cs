using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ISOCodex.Currency.AspNetCore;

/// <summary>
/// Provides the <see cref="CurrencyCodeModelBinder"/> for <see cref="CurrencyCode"/> and nullable <see cref="CurrencyCode"/> values.
/// </summary>
public sealed class CurrencyCodeModelBinderProvider : IModelBinderProvider
{
    private static readonly CurrencyCodeModelBinder Binder = new CurrencyCodeModelBinder();

    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var modelType = Nullable.GetUnderlyingType(context.Metadata.ModelType) ?? context.Metadata.ModelType;
        return modelType == typeof(CurrencyCode)
            ? Binder
            : null;
    }
}
