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

    /// <summary>Gets whether this value is the uninitialised default money value.</summary>
    public bool IsDefault => Currency.IsDefault;

    /// <summary>
    /// Creates a money value after validating the amount against the currency minor unit.
    /// </summary>
    public static Money Of(decimal amount, CurrencyCode currency)
    {
        ThrowIfDefaultCurrency(currency, nameof(currency));

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
    /// Attempts to create a money value without throwing for ordinary validation failures.
    /// </summary>
    public static MoneyValidationResult TryCreate(decimal amount, CurrencyCode currency)
    {
        if (currency.IsDefault)
        {
            return MoneyValidationResult.Failure(
                MoneyValidationFailureReason.DefaultCurrency,
                "Currency code must be initialised before it can be used to create money.");
        }

        if (!DefaultCurrencyRegistry.Instance.TryGet(currency, out var currencyInfo))
        {
            return MoneyValidationResult.Failure(
                MoneyValidationFailureReason.UnknownCurrency,
                $"Currency code '{currency}' is not registered.");
        }

        var validationFailure = ValidateAmountPrecisionOrFailure(amount, currencyInfo);
        if (validationFailure != null)
        {
            return validationFailure;
        }

        return MoneyValidationResult.Success(new Money(amount, currency));
    }

    /// <summary>
    /// Attempts to parse a currency code and create a money value without throwing for ordinary validation failures.
    /// </summary>
    public static MoneyValidationResult TryCreate(decimal amount, string currencyCode)
    {
        if (!CurrencyCode.TryParse(currencyCode, out var parsedCurrency))
        {
            return MoneyValidationResult.Failure(
                MoneyValidationFailureReason.UnknownCurrency,
                $"Currency code '{currencyCode}' is not a registered ISO 4217-style currency code.");
        }

        return TryCreate(amount, parsedCurrency);
    }

    /// <summary>
    /// Attempts to create a money value without throwing for ordinary validation failures.
    /// </summary>
    public static bool TryCreate(decimal amount, CurrencyCode currency, out Money money)
    {
        var result = TryCreate(amount, currency);
        money = result.Money.GetValueOrDefault();
        return result.Succeeded;
    }

    /// <summary>
    /// Attempts to parse a currency code and create a money value without throwing for ordinary validation failures.
    /// </summary>
    public static bool TryCreate(decimal amount, string currencyCode, out Money money)
    {
        var result = TryCreate(amount, currencyCode);
        money = result.Money.GetValueOrDefault();
        return result.Succeeded;
    }

    /// <summary>
    /// Creates a zero money value for the supplied currency.
    /// </summary>
    public static Money Zero(CurrencyCode currency)
    {
        ThrowIfDefaultCurrency(currency, nameof(currency));

        return Of(0m, currency);
    }

    /// <summary>
    /// Creates a money value from exact integer minor units.
    /// </summary>
    public static Money FromMinorUnits(long minorUnits, CurrencyCode currency)
    {
        ThrowIfDefaultCurrency(currency, nameof(currency));

        var currencyInfo = DefaultCurrencyRegistry.Instance.Get(currency);

        if (!currencyInfo.MinorUnit.IsApplicable)
        {
            throw new InvalidOperationException($"Currency '{currency}' does not define applicable minor units.");
        }

        return new Money(minorUnits * currencyInfo.MinorUnit.Increment, currency);
    }

    /// <summary>
    /// Attempts to create a money value from exact integer minor units without throwing for ordinary validation failures.
    /// </summary>
    public static MoneyValidationResult TryFromMinorUnits(long minorUnits, CurrencyCode currency)
    {
        if (currency.IsDefault)
        {
            return MoneyValidationResult.Failure(
                MoneyValidationFailureReason.DefaultCurrency,
                "Currency code must be initialised before it can be used to create money.");
        }

        if (!DefaultCurrencyRegistry.Instance.TryGet(currency, out var currencyInfo))
        {
            return MoneyValidationResult.Failure(
                MoneyValidationFailureReason.UnknownCurrency,
                $"Currency code '{currency}' is not registered.");
        }

        if (!currencyInfo.MinorUnit.IsApplicable)
        {
            return MoneyValidationResult.Failure(
                MoneyValidationFailureReason.MinorUnitNotApplicable,
                $"Currency '{currency}' does not define applicable minor units.");
        }

        try
        {
            return MoneyValidationResult.Success(new Money(minorUnits * currencyInfo.MinorUnit.Increment, currency));
        }
        catch (OverflowException)
        {
            return MoneyValidationResult.Failure(
                MoneyValidationFailureReason.Overflow,
                $"Minor-unit value '{minorUnits}' is too large to convert for currency '{currency}'.");
        }
    }

    /// <summary>
    /// Converts the amount to exact integer minor units.
    /// </summary>
    public long ToMinorUnits()
    {
        ThrowIfDefault();

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
        ThrowIfDefault();
        ThrowIfDefault(other, nameof(other));

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
        ThrowIfDefault();

        return new Money(-Amount, Currency);
    }

    /// <summary>
    /// Returns the absolute value of this money value.
    /// </summary>
    public Money Abs()
    {
        ThrowIfDefault();

        return new Money(Math.Abs(Amount), Currency);
    }

    /// <summary>
    /// Multiplies this money value and rounds the result using an explicit policy.
    /// </summary>
    public Money Multiply(decimal factor, CurrencyRoundingPolicy roundingPolicy)
    {
        ThrowIfDefault();

        if (roundingPolicy == null)
        {
            throw new ArgumentNullException(nameof(roundingPolicy));
        }

        var currency = DefaultCurrencyRegistry.Instance.Get(Currency);
        var roundedAmount = new CurrencyRoundingService().RoundAmount(Amount * factor, currency, roundingPolicy);

        return Of(roundedAmount, Currency);
    }

    /// <summary>
    /// Divides this money value and rounds the result using an explicit policy.
    /// </summary>
    public Money Divide(decimal divisor, CurrencyRoundingPolicy roundingPolicy)
    {
        ThrowIfDefault();

        if (divisor == 0m)
        {
            throw new DivideByZeroException("Money cannot be divided by zero.");
        }

        if (roundingPolicy == null)
        {
            throw new ArgumentNullException(nameof(roundingPolicy));
        }

        var currency = DefaultCurrencyRegistry.Instance.Get(Currency);
        var roundedAmount = new CurrencyRoundingService().RoundAmount(Amount / divisor, currency, roundingPolicy);

        return Of(roundedAmount, Currency);
    }

    /// <summary>
    /// Rounds this money value using an explicit policy.
    /// </summary>
    public Money Round(CurrencyRoundingPolicy policy)
    {
        ThrowIfDefault();

        return new CurrencyRoundingService().Round(this, policy);
    }

    /// <summary>
    /// Allocates this money value into exact minor-unit parts while preserving the total.
    /// </summary>
    public MoneyAllocation Allocate(
        int parts,
        AllocationRemainderStrategy remainderStrategy = AllocationRemainderStrategy.First)
    {
        ThrowIfDefault();

        return new MoneyAllocator().Allocate(this, parts, remainderStrategy);
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

    private static void ThrowIfDefaultCurrency(CurrencyCode currency, string parameterName)
    {
        if (currency.IsDefault)
        {
            throw new ArgumentException("Currency code must be initialised before it can be used to create money.", parameterName);
        }
    }

    private void ThrowIfDefault()
    {
        ThrowIfDefault(this, nameof(Money));
    }

    private static void ThrowIfDefault(Money money, string parameterName)
    {
        if (money.IsDefault)
        {
            throw new InvalidOperationException($"Money value '{parameterName}' is the uninitialised default value. Use Money.Zero(currency) or Money.Of(amount, currency).");
        }
    }

    private static void ValidateAmountPrecision(decimal amount, CurrencyInfo currencyInfo)
    {
        var failure = ValidateAmountPrecisionOrFailure(amount, currencyInfo);
        if (failure != null)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), amount, failure.ErrorMessage);
        }
    }

    private static MoneyValidationResult? ValidateAmountPrecisionOrFailure(decimal amount, CurrencyInfo currencyInfo)
    {
        if (!currencyInfo.MinorUnit.IsApplicable)
        {
            return amount == decimal.Truncate(amount)
                ? null
                : MoneyValidationResult.Failure(
                    MoneyValidationFailureReason.AmountPrecision,
                    $"Currency '{currencyInfo.Code}' does not define applicable fractional minor units.");
        }

        var rounded = Math.Round(amount, currencyInfo.MinorUnit.DecimalPlaces, MidpointRounding.ToEven);

        return rounded == amount
            ? null
            : MoneyValidationResult.Failure(
                MoneyValidationFailureReason.AmountPrecision,
                $"Amount '{amount}' has more than {currencyInfo.MinorUnit.DecimalPlaces} fraction digit(s) for currency '{currencyInfo.Code}'.");
    }
}
