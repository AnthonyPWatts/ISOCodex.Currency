using ISOCodex.Currency;

namespace Currency.Tests;

public class MoneyRoundingTests
{
    [Fact]
    public void Round_UsesCashPolicyForMoney()
    {
        var money = Money.Of(1.03m, CurrencyCode.CHF);

        var rounded = money.Round(CurrencyRoundingPolicy.Cash());

        Assert.Equal(Money.Of(1.05m, CurrencyCode.CHF), rounded);
    }

    [Fact]
    public void Round_ThrowsForStandardPolicyWhenMinorUnitsAreNotApplicable()
    {
        var money = Money.Of(10m, CurrencyCode.XXX);

        Assert.Throws<InvalidOperationException>(() => money.Round(CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven)));
    }

    [Fact]
    public void Multiply_UsesExplicitStandardRoundingPolicy()
    {
        var money = Money.Of(1m, CurrencyCode.GBP);

        var rounded = money.Multiply(1.005m, CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven));

        Assert.Equal(Money.Of(1.00m, CurrencyCode.GBP), rounded);
    }

    [Fact]
    public void Multiply_UsesExplicitAwayFromZeroRoundingPolicy()
    {
        var money = Money.Of(1m, CurrencyCode.GBP);

        var rounded = money.Multiply(1.005m, CurrencyRoundingPolicy.AwayFromZero());

        Assert.Equal(Money.Of(1.01m, CurrencyCode.GBP), rounded);
    }

    [Fact]
    public void Multiply_SupportsNegativeValues()
    {
        var money = Money.Of(-1m, CurrencyCode.GBP);

        var rounded = money.Multiply(1.005m, CurrencyRoundingPolicy.AwayFromZero());

        Assert.Equal(Money.Of(-1.01m, CurrencyCode.GBP), rounded);
    }

    [Fact]
    public void Multiply_ThrowsWhenPolicyIsNull()
    {
        var money = Money.Of(1m, CurrencyCode.GBP);

        Assert.Throws<ArgumentNullException>(() => money.Multiply(2m, null!));
    }

    [Fact]
    public void Divide_UsesExplicitStandardRoundingPolicy()
    {
        var money = Money.Of(1m, CurrencyCode.GBP);

        var rounded = money.Divide(6m, CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven));

        Assert.Equal(Money.Of(0.17m, CurrencyCode.GBP), rounded);
    }

    [Fact]
    public void Divide_UsesCurrencyPrecision()
    {
        var money = Money.Of(1m, CurrencyCode.KWD);

        var rounded = money.Divide(6m, CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven));

        Assert.Equal(Money.Of(0.167m, CurrencyCode.KWD), rounded);
    }

    [Fact]
    public void Divide_ThrowsWhenDivisorIsZero()
    {
        var money = Money.Of(1m, CurrencyCode.GBP);

        Assert.Throws<DivideByZeroException>(() => money.Divide(0m, CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven)));
    }

    [Fact]
    public void Divide_ThrowsWhenPolicyIsNull()
    {
        var money = Money.Of(1m, CurrencyCode.GBP);

        Assert.Throws<ArgumentNullException>(() => money.Divide(2m, null!));
    }

    [Fact]
    public void CustomPolicyThatExceedsCurrencyPrecisionStillReturnsValidMoneyOrThrows()
    {
        var money = Money.Of(1m, CurrencyCode.GBP);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => money.Multiply(1.005m, CurrencyRoundingPolicy.Custom(3, MidpointRounding.ToEven)));

        Assert.Contains("GBP", exception.Message);
    }
}
