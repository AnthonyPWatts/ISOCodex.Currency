using System;
using System.Globalization;

namespace ISOCodex.Currency;

/// <summary>
/// Represents an immutable monetary amount with an explicit currency.
/// </summary>
public readonly struct Money : IEquatable<Money>, IComparable<Money>
{
    private Money(decimal amount, CurrencyCode currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>Gets the monetary amount.</summary>
    public decimal Amount { get; }

    /// <summary>Gets the amount currency.</summary>
    public CurrencyCode Currency { get; }

    /// <summary>
    /// Creates a money value after validating the amount against the currency minor unit.
    /// </summary>
    public static Money Of(decimal amount, CurrencyCode currency)
    {
        var currencyInfo = DefaultCurrencyRegistry.Instance.Get(currency);
        ValidateAmountPrecision(amount, currencyInfo);

        return new Money(amount, currency);
    }

    /// <summary>
    /// Creates a money value after parsing the currency code and validating the amount.
    /// </summary>
    public static Money Of(decimal amount, string currencyCode)
    {
        return Of(amount, CurrencyCode.Parse(currencyCode));
    }

    /// <summary>
    /// Creates a zero money value for the supplied currency.
    /// </summary>
    public static Money Zero(CurrencyCode currency)
    {
        return Of(0m, currency);
    }

    /// <summary>
    /// Creates a money value from exact integer minor units.
    /// </summary>
    public static Money FromMinorUnits(long minorUnits, CurrencyCode currency)
    {
        var currencyInfo = DefaultCurrencyRegistry.Instance.Get(currency);

        if (!currencyInfo.MinorUnit.IsApplicable)
        {
            throw new InvalidOperationException($"Currency '{currency}' does not define applicable minor units.");
        }

        return new Money(minorUnits * currencyInfo.MinorUnit.Increment, currency);
    }

    /// <summary>
    /// Converts the amount to exact integer minor units.
    /// </summary>
    public long ToMinorUnits()
    {
        var currencyInfo = DefaultCurrencyRegistry.Instance.Get(Currency);

        if (!currencyInfo.MinorUnit.IsApplicable)
        {
            throw new InvalidOperationException($"Currency '{Currency}' does not define applicable minor units.");
        }

        var minorUnits = Amount / currencyInfo.MinorUnit.Increment;

        if (minorUnits != decimal.Truncate(minorUnits))
        {
            throw new InvalidOperationException($"Amount '{Amount}' cannot be represented as exact minor units for '{Currency}'.");
        }

        return checked((long)minorUnits);
    }

    /// <summary>
    /// Ensures another money value uses the same currency.
    /// </summary>
    public Money RequireSameCurrency(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException($"Money values must use the same currency. Actual currencies were '{Currency}' and '{other.Currency}'.");
        }

        return other;
    }

    /// <summary>
    /// Adds another same-currency money value.
    /// </summary>
    public Money Add(Money other)
    {
        RequireSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// Subtracts another same-currency money value.
    /// </summary>
    public Money Subtract(Money other)
    {
        RequireSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    /// <summary>
    /// Negates this money value.
    /// </summary>
    public Money Negate()
    {
        return new Money(-Amount, Currency);
    }

    /// <summary>
    /// Returns the absolute value of this money value.
    /// </summary>
    public Money Abs()
    {
        return new Money(Math.Abs(Amount), Currency);
    }

    /// <inheritdoc />
    public int CompareTo(Money other)
    {
        RequireSameCurrency(other);
        return Amount.CompareTo(other.Amount);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture, "{0} {1}", Currency, Amount);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Money other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(Money other)
    {
        return Amount == other.Amount && Currency == other.Currency;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (Amount.GetHashCode() * 397) ^ Currency.GetHashCode();
        }
    }

    /// <summary>Adds two same-currency money values.</summary>
    public static Money operator +(Money left, Money right)
    {
        return left.Add(right);
    }

    /// <summary>Subtracts two same-currency money values.</summary>
    public static Money operator -(Money left, Money right)
    {
        return left.Subtract(right);
    }

    /// <summary>Negates a money value.</summary>
    public static Money operator -(Money value)
    {
        return value.Negate();
    }

    /// <summary>Returns whether two money values are equal.</summary>
    public static bool operator ==(Money left, Money right)
    {
        return left.Equals(right);
    }

    /// <summary>Returns whether two money values are not equal.</summary>
    public static bool operator !=(Money left, Money right)
    {
        return !left.Equals(right);
    }

    private static void ValidateAmountPrecision(decimal amount, CurrencyInfo currencyInfo)
    {
        if (!currencyInfo.MinorUnit.IsApplicable)
        {
            if (amount != decimal.Truncate(amount))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    $"Currency '{currencyInfo.Code}' does not define applicable fractional minor units.");
            }

            return;
        }

        var rounded = Math.Round(amount, currencyInfo.MinorUnit.DecimalPlaces, MidpointRounding.ToEven);

        if (rounded != amount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                $"Amount '{amount}' has more than {currencyInfo.MinorUnit.DecimalPlaces} fraction digit(s) for currency '{currencyInfo.Code}'.");
        }
    }
}
