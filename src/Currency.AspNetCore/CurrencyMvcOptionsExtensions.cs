using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ISOCodex.Currency.AspNetCore;

/// <summary>
/// MVC option helpers for currency model binding.
/// </summary>
public static class CurrencyMvcOptionsExtensions
{
    /// <summary>
    /// Adds the <see cref="CurrencyCodeModelBinderProvider"/> to MVC model binding.
    /// </summary>
    public static MvcOptions AddCurrencyCodeModelBinder(this MvcOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (!options.ModelBinderProviders.OfType<CurrencyCodeModelBinderProvider>().Any())
        {
            options.ModelBinderProviders.Insert(0, new CurrencyCodeModelBinderProvider());
        }

        return options;
    }
}
