using ISOCodex.Currency;
using System.Globalization;

namespace Currency.Tests;

public class CurrencyRoundingServiceTests
{
    private readonly CurrencyRoundingService _roundingService = new();
    private readonly ICurrencyRegistry _registry = DefaultCurrencyRegistry.Instance;

    [Theory]
    [InlineData("12.345", "12.34")]
    [InlineData("12.355", "12.36")]
    [InlineData("-12.345", "-12.34")]
    [InlineData("-12.355", "-12.36")]
    public void RoundAmount_UsesStandardMinorUnitsAndToEvenMidpointForGbp(string amountText, string expectedText)
    {
        var result = RoundAmount(amountText, CurrencyCode.GBP, CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven));

        Assert.Equal(ParseDecimal(expectedText), result);
    }

    [Theory]
    [InlineData("12.345", "12.35")]
    [InlineData("-12.345", "-12.35")]
    public void RoundAmount_UsesAwayFromZeroWhenPolicyRequestsIt(string amountText, string expectedText)
    {
        var result = RoundAmount(amountText, CurrencyCode.GBP, CurrencyRoundingPolicy.AwayFromZero());

        Assert.Equal(ParseDecimal(expectedText), result);
    }

    [Theory]
    [InlineData("122.5", "122")]
    [InlineData("123.5", "124")]
    [InlineData("123.6", "124")]
    [InlineData("123.4", "123")]
    public void RoundAmount_UsesZeroDecimalPlacesForJpy(string amountText, string expectedText)
    {
        var result = RoundAmount(amountText, CurrencyCode.JPY, CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven));

        Assert.Equal(ParseDecimal(expectedText), result);
    }

    [Theory]
    [InlineData("1.2345", "1.234")]
    [InlineData("1.2355", "1.236")]
    [InlineData("-1.2345", "-1.234")]
    [InlineData("-1.2355", "-1.236")]
    public void RoundAmount_UsesThreeDecimalPlacesForKwd(string amountText, string expectedText)
    {
        var result = RoundAmount(amountText, CurrencyCode.KWD, CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven));

        Assert.Equal(ParseDecimal(expectedText), result);
    }

    [Theory]
    [InlineData("1.23454", "1.2345")]
    [InlineData("1.23455", "1.2346")]
    [InlineData("1.23456", "1.2346")]
    public void RoundAmount_UsesFourDecimalPlacesForClf(string amountText, string expectedText)
    {
        var result = RoundAmount(amountText, CurrencyCode.Parse("CLF"), CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven));

        Assert.Equal(ParseDecimal(expectedText), result);
    }

    [Theory]
    [InlineData("1.02", "1.00")]
    [InlineData("1.03", "1.05")]
    [InlineData("1.025", "1.05")]
    [InlineData("-1.02", "-1.00")]
    [InlineData("-1.03", "-1.05")]
    [InlineData("-1.025", "-1.05")]
    public void RoundAmount_UsesCashIncrementForChf(string amountText, string expectedText)
    {
        var result = RoundAmount(amountText, CurrencyCode.CHF, CurrencyRoundingPolicy.Cash());

        Assert.Equal(ParseDecimal(expectedText), result);
    }

    [Theory]
    [InlineData("1.37", "1.25")]
    [InlineData("1.38", "1.50")]
    [InlineData("-1.37", "-1.25")]
    [InlineData("-1.38", "-1.50")]
    public void RoundAmount_UsesCustomIncrement(string amountText, string expectedText)
    {
        var result = RoundAmount(
            amountText,
            CurrencyCode.GBP,
            CurrencyRoundingPolicy.CustomIncrement(0.25m, MidpointRounding.AwayFromZero));

        Assert.Equal(ParseDecimal(expectedText), result);
    }

    [Theory]
    [InlineData("1.2345", "1.234")]
    [InlineData("1.2355", "1.236")]
    public void RoundAmount_UsesCustomDecimalPlaces(string amountText, string expectedText)
    {
        var result = RoundAmount(
            amountText,
            CurrencyCode.GBP,
            CurrencyRoundingPolicy.Custom(3, MidpointRounding.ToEven));

        Assert.Equal(ParseDecimal(expectedText), result);
    }

    [Fact]
    public void RoundAmount_ThrowsWhenStandardMinorUnitsAreNotApplicable()
    {
        var currency = _registry.Get(CurrencyCode.XXX);

        var exception = Assert.Throws<InvalidOperationException>(
            () => _roundingService.RoundAmount(12.34m, currency, CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven)));

        Assert.Contains("XXX", exception.Message);
    }

    [Fact]
    public void RoundAmount_AllowsCustomDecimalPlacesWhenMinorUnitsAreNotApplicable()
    {
        var currency = _registry.Get(CurrencyCode.XXX);

        var result = _roundingService.RoundAmount(12.345m, currency, CurrencyRoundingPolicy.Custom(2, MidpointRounding.AwayFromZero));

        Assert.Equal(12.35m, result);
    }

    [Fact]
    public void Round_ReturnsMoneyInSameCurrency()
    {
        var money = Money.Of(1.03m, CurrencyCode.CHF);

        var rounded = _roundingService.Round(money, CurrencyRoundingPolicy.Cash());

        Assert.Equal(Money.Of(1.05m, CurrencyCode.CHF), rounded);
    }

    [Fact]
    public void Round_ThrowsWhenPolicyIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _roundingService.Round(Money.Of(1m, CurrencyCode.GBP), null!));
    }

    [Fact]
    public void RoundAmount_ThrowsWhenCurrencyIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _roundingService.RoundAmount(1m, null!, CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven)));
    }

    [Fact]
    public void RoundAmount_ThrowsWhenPolicyIsNull()
    {
        var currency = _registry.Get(CurrencyCode.GBP);

        Assert.Throws<ArgumentNullException>(() => _roundingService.RoundAmount(1m, currency, null!));
    }

    private decimal RoundAmount(string amountText, CurrencyCode currencyCode, CurrencyRoundingPolicy policy)
    {
        return _roundingService.RoundAmount(ParseDecimal(amountText), _registry.Get(currencyCode), policy);
    }

    private static decimal ParseDecimal(string text)
    {
        return decimal.Parse(text, CultureInfo.InvariantCulture);
    }
}
