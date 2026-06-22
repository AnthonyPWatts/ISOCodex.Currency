using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISOCodex.Currency.AspNetCore;

/// <summary>
/// Dependency injection helpers for ASP.NET Core applications using currency services.
/// </summary>
public static class CurrencyServiceCollectionExtensions
{
    /// <summary>
    /// Registers default currency services and MVC model binding helpers.
    /// </summary>
    public static IServiceCollection AddCurrencyAspNetCore(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.TryAddSingleton<ICurrencyRegistry>(DefaultCurrencyRegistry.Instance);
        services.TryAddSingleton<IMoneyFormatter, MoneyFormatter>();
        services.TryAddSingleton<MoneyParser>();
        services.Configure<MvcOptions>(options => options.AddCurrencyCodeModelBinder());

        return services;
    }
}
