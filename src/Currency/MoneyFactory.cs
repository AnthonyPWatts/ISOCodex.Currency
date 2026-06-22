using System;

namespace ISOCodex.Currency;

/// <summary>
/// Creates and validates money values using an explicit currency registry.
/// </summary>
public sealed class MoneyFactory
{
    private readonly ICurrencyRegistry _registry;

    /// <summary>
    /// Creates a money factory backed by the supplied currency registry.
    /// </summary>
    public MoneyFactory(ICurrencyRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    /// <summary>
    /// Creates a money value after validating the amount against the configured registry.
    /// </summary>
    public Money Of(decimal amount, CurrencyCode currency)
    {
        return Money.Of(amount, currency, _registry, nameof(currency));
    }

    /// <summary>
    /// Attempts to create a money value without throwing for ordinary validation failures.
    /// </summary>
    public MoneyValidationResult TryCreate(decimal amount, CurrencyCode currency)
    {
        return Money.TryCreate(amount, currency, _registry);
    }

    /// <summary>
    /// Creates a money value from exact integer minor units using the configured registry.
    /// </summary>
    public Money FromMinorUnits(long minorUnits, CurrencyCode currency)
    {
        return Money.FromMinorUnits(minorUnits, currency, _registry, nameof(currency));
    }

    /// <summary>
    /// Attempts to create a money value from exact integer minor units without throwing for ordinary validation failures.
    /// </summary>
    public MoneyValidationResult TryFromMinorUnits(long minorUnits, CurrencyCode currency)
    {
        return Money.TryFromMinorUnits(minorUnits, currency, _registry);
    }
}
