using ISOCodex.Currency;

namespace Currency.Tests;

public class MoneyTests
{
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
    public void Abs_ReturnsAbsoluteValue()
    {
        Assert.Equal(Money.Of(10m, CurrencyCode.GBP), Money.Of(-10m, CurrencyCode.GBP).Abs());
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
    public void ToString_ReturnsCodeAndAmount()
    {
        Assert.Equal("GBP 12.34", Money.Of(12.34m, CurrencyCode.GBP).ToString());
    }
}
