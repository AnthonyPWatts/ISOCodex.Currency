using ISOCodex.Currency;
using System.Globalization;

namespace Currency.Tests;

public class CurrencyRoundingPolicyTests
{
    [Fact]
    public void Standard_CreatesStandardPolicyWithToEvenMidpointByDefault()
    {
        var policy = CurrencyRoundingPolicy.Standard();

        Assert.Equal(CurrencyRoundingContext.Standard, policy.Context);
        Assert.Null(policy.DecimalPlaces);
        Assert.Null(policy.Increment);
        Assert.Equal(MidpointRounding.ToEven, policy.MidpointRounding);
    }

    [Fact]
    public void AwayFromZero_CreatesStandardPolicyWithAwayFromZeroMidpoint()
    {
        var policy = CurrencyRoundingPolicy.AwayFromZero();

        Assert.Equal(CurrencyRoundingContext.Standard, policy.Context);
        Assert.Equal(MidpointRounding.AwayFromZero, policy.MidpointRounding);
    }

    [Fact]
    public void Cash_CreatesCashPolicyWithAwayFromZeroMidpointByDefault()
    {
        var policy = CurrencyRoundingPolicy.Cash();

        Assert.Equal(CurrencyRoundingContext.Cash, policy.Context);
        Assert.Equal(MidpointRounding.AwayFromZero, policy.MidpointRounding);
    }

    [Fact]
    public void Custom_CreatesDecimalPlacePolicy()
    {
        var policy = CurrencyRoundingPolicy.Custom(3, MidpointRounding.ToEven);

        Assert.Equal(CurrencyRoundingContext.Custom, policy.Context);
        Assert.Equal(3, policy.DecimalPlaces);
        Assert.Null(policy.Increment);
        Assert.Equal(MidpointRounding.ToEven, policy.MidpointRounding);
    }

    [Fact]
    public void Custom_RejectsNegativeDecimalPlaces()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CurrencyRoundingPolicy.Custom(-1, MidpointRounding.ToEven));
    }

    [Fact]
    public void CustomIncrement_CreatesIncrementPolicy()
    {
        var policy = CurrencyRoundingPolicy.CustomIncrement(0.05m, MidpointRounding.AwayFromZero);

        Assert.Equal(CurrencyRoundingContext.Custom, policy.Context);
        Assert.Null(policy.DecimalPlaces);
        Assert.Equal(0.05m, policy.Increment);
        Assert.Equal(MidpointRounding.AwayFromZero, policy.MidpointRounding);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-0.01")]
    public void CustomIncrement_RejectsNonPositiveIncrement(string incrementText)
    {
        var increment = decimal.Parse(incrementText, CultureInfo.InvariantCulture);

        Assert.Throws<ArgumentOutOfRangeException>(() => CurrencyRoundingPolicy.CustomIncrement(increment, MidpointRounding.ToEven));
    }
}
