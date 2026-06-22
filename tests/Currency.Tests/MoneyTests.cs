using ISOCodex.Currency;

namespace Currency.Tests;

public class MoneyTests
{
    [Fact]
    public void IsDefault_ReturnsTrueForDefaultValue()
    {
        Assert.True(default(Money).IsDefault);
    }

    [Fact]
    public void IsDefault_ReturnsFalseForInitializedValue()
    {
        Assert.False(Money.Zero(CurrencyCode.GBP).IsDefault);
    }

    [Fact]
    public void Equality_RemainsSafeForDefaultValues()
    {
        var left = default(Money);
        var right = default(Money);

        Assert.True(left == right);
        Assert.True(left.Equals(right));
        _ = left.GetHashCode();
    }

    [Fact]
    public void Of_AcceptsAmountWithinCurrencyPrecision()
    {
        var money = Money.Of(12.34m, CurrencyCode.GBP);

        Assert.Equal(12.34m, money.Amount);
        Assert.Equal(CurrencyCode.GBP, money.Currency);
    }

    [Fact]
    public void Of_RejectsAmountWithTooManyFractionDigits()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Money.Of(12.345m, CurrencyCode.GBP));

        Assert.Contains("GBP", exception.Message);
        Assert.Contains("2", exception.Message);
    }

    [Fact]
    public void Of_AcceptsZeroMinorUnitCurrencyWithoutFractions()
    {
        var money = Money.Of(100m, CurrencyCode.JPY);

        Assert.Equal(100m, money.Amount);
        Assert.Equal(CurrencyCode.JPY, money.Currency);
    }

    [Fact]
    public void Of_RejectsFractionsForZeroMinorUnitCurrency()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Money.Of(100.01m, CurrencyCode.JPY));

        Assert.Contains("JPY", exception.Message);
    }

    [Fact]
    public void Of_AcceptsThreeDecimalPlaceCurrency()
    {
        var money = Money.Of(1.234m, CurrencyCode.KWD);

        Assert.Equal(1.234m, money.Amount);
        Assert.Equal(CurrencyCode.KWD, money.Currency);
    }

    [Fact]
    public void Of_ParsesCurrencyCode()
    {
        var money = Money.Of(12.34m, "gbp");

        Assert.Equal(CurrencyCode.GBP, money.Currency);
    }

    [Fact]
    public void Of_RejectsDefaultCurrency()
    {
        var exception = Assert.Throws<ArgumentException>(() => Money.Of(12.34m, default(CurrencyCode)));

        Assert.Contains("initialised", exception.Message);
    }

    [Fact]
    public void Zero_CreatesZeroMoneyForCurrency()
    {
        var money = Money.Zero(CurrencyCode.USD);

        Assert.Equal(0m, money.Amount);
        Assert.Equal(CurrencyCode.USD, money.Currency);
    }

    [Fact]
    public void FromMinorUnits_CreatesMoneyUsingCurrencyIncrement()
    {
        var money = Money.FromMinorUnits(1234, CurrencyCode.GBP);

        Assert.Equal(Money.Of(12.34m, CurrencyCode.GBP), money);
    }

    [Fact]
    public void FromMinorUnits_RejectsDefaultCurrency()
    {
        var exception = Assert.Throws<ArgumentException>(() => Money.FromMinorUnits(1234, default(CurrencyCode)));

        Assert.Contains("initialised", exception.Message);
    }

    [Fact]
    public void FromMinorUnits_UsesZeroMinorUnitIncrement()
    {
        var money = Money.FromMinorUnits(100, CurrencyCode.JPY);

        Assert.Equal(Money.Of(100m, CurrencyCode.JPY), money);
    }

    [Fact]
    public void ToMinorUnits_ReturnsExactMinorUnits()
    {
        var money = Money.Of(12.34m, CurrencyCode.GBP);

        Assert.Equal(1234, money.ToMinorUnits());
    }

    [Fact]
    public void ToMinorUnits_RejectsDefaultMoney()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => default(Money).ToMinorUnits());

        Assert.Contains("default", exception.Message);
        Assert.Contains("Money.Zero", exception.Message);
    }

    [Fact]
    public void Add_AddsSameCurrencyValues()
    {
        var result = Money.Of(10m, CurrencyCode.GBP) + Money.Of(2.34m, CurrencyCode.GBP);

        Assert.Equal(Money.Of(12.34m, CurrencyCode.GBP), result);
    }

    [Fact]
    public void Add_RejectsDifferentCurrencyValues()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => Money.Of(10m, CurrencyCode.GBP) + Money.Of(10m, CurrencyCode.USD));

        Assert.Contains("GBP", exception.Message);
        Assert.Contains("USD", exception.Message);
    }

    [Fact]
    public void Add_RejectsDefaultMoney()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => default(Money) + Money.Of(10m, CurrencyCode.GBP));

        Assert.Contains("default", exception.Message);
    }

    [Fact]
    public void Add_RejectsDefaultOtherMoney()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => Money.Of(10m, CurrencyCode.GBP) + default(Money));

        Assert.Contains("default", exception.Message);
    }

    [Fact]
    public void Subtract_SubtractsSameCurrencyValues()
    {
        var result = Money.Of(10m, CurrencyCode.GBP) - Money.Of(2.34m, CurrencyCode.GBP);

        Assert.Equal(Money.Of(7.66m, CurrencyCode.GBP), result);
    }

    [Fact]
    public void Negate_ReturnsNegativeValue()
    {
        Assert.Equal(Money.Of(-10m, CurrencyCode.GBP), -Money.Of(10m, CurrencyCode.GBP));
    }

    [Fact]
    public void Negate_RejectsDefaultMoney()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => -default(Money));

        Assert.Contains("default", exception.Message);
    }

    [Fact]
    public void Abs_ReturnsAbsoluteValue()
    {
        Assert.Equal(Money.Of(10m, CurrencyCode.GBP), Money.Of(-10m, CurrencyCode.GBP).Abs());
    }

    [Fact]
    public void Abs_RejectsDefaultMoney()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => default(Money).Abs());

        Assert.Contains("default", exception.Message);
    }

    [Fact]
    public void CompareTo_ComparesSameCurrencyValues()
    {
        Assert.True(Money.Of(12m, CurrencyCode.GBP).CompareTo(Money.Of(10m, CurrencyCode.GBP)) > 0);
    }

    [Fact]
    public void CompareTo_RejectsDifferentCurrencyValues()
    {
        Assert.Throws<InvalidOperationException>(() => Money.Of(10m, CurrencyCode.GBP).CompareTo(Money.Of(10m, CurrencyCode.USD)));
    }

    [Fact]
    public void CompareTo_RejectsDefaultMoney()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => default(Money).CompareTo(Money.Of(10m, CurrencyCode.GBP)));

        Assert.Contains("default", exception.Message);
    }

    [Fact]
    public void Multiply_RejectsDefaultMoney()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => default(Money).Multiply(2m, CurrencyRoundingPolicy.AwayFromZero()));

        Assert.Contains("default", exception.Message);
    }

    [Fact]
    public void Divide_RejectsDefaultMoney()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => default(Money).Divide(2m, CurrencyRoundingPolicy.AwayFromZero()));

        Assert.Contains("default", exception.Message);
    }

    [Fact]
    public void Round_RejectsDefaultMoney()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => default(Money).Round(CurrencyRoundingPolicy.AwayFromZero()));

        Assert.Contains("default", exception.Message);
    }

    [Fact]
    public void Allocate_RejectsDefaultMoney()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => default(Money).Allocate(2));

        Assert.Contains("default", exception.Message);
    }

    [Fact]
    public void ToString_ReturnsCodeAndAmount()
    {
        Assert.Equal("GBP 12.34", Money.Of(12.34m, CurrencyCode.GBP).ToString());
    }
}
