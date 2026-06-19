using System;

namespace ISOCodex.Currency;

/// <summary>
/// Default currency rounding service.
/// </summary>
public sealed class CurrencyRoundingService : ICurrencyRoundingService
{
    /// <inheritdoc />
    public Money Round(Money money, CurrencyRoundingPolicy policy)
    {
        if (policy == null)
        {
            throw new ArgumentNullException(nameof(policy));
        }

        var currency = DefaultCurrencyRegistry.Instance.Get(money.Currency);
        var roundedAmount = RoundAmount(money.Amount, currency, policy);

        return Money.Of(roundedAmount, money.Currency);
    }

    /// <inheritdoc />
    public decimal RoundAmount(decimal amount, CurrencyInfo currency, CurrencyRoundingPolicy policy)
    {
        if (currency == null)
        {
            throw new ArgumentNullException(nameof(currency));
        }

        if (policy == null)
        {
            throw new ArgumentNullException(nameof(policy));
        }

        if (policy.Increment.HasValue)
        {
            return RoundToIncrement(amount, policy.Increment.Value, policy.MidpointRounding);
        }

        if (policy.DecimalPlaces.HasValue)
        {
            return Math.Round(amount, policy.DecimalPlaces.Value, policy.MidpointRounding);
        }

        if (policy.Context == CurrencyRoundingContext.Cash)
        {
            if (currency.CashRoundingIncrement.HasValue)
            {
                return RoundToIncrement(amount, currency.CashRoundingIncrement.Value, policy.MidpointRounding);
            }

            return RoundToMinorUnit(amount, currency, currency.CashMinorUnit, policy);
        }

        return RoundToMinorUnit(amount, currency, currency.MinorUnit, policy);
    }

    private static decimal RoundToMinorUnit(
        decimal amount,
        CurrencyInfo currency,
        CurrencyMinorUnit minorUnit,
        CurrencyRoundingPolicy policy)
    {
        if (!minorUnit.IsApplicable)
        {
            throw new InvalidOperationException($"Currency '{currency.Code}' does not define applicable minor units for {policy.Context.ToString().ToLowerInvariant()} rounding.");
        }

        return Math.Round(amount, minorUnit.DecimalPlaces, policy.MidpointRounding);
    }

    private static decimal RoundToIncrement(decimal amount, decimal increment, MidpointRounding midpointRounding)
    {
        if (increment <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(increment), "Rounding increment must be greater than zero.");
        }

        return Math.Round(amount / increment, 0, midpointRounding) * increment;
    }
}
