using ISOCodex.Currency;
using System.Globalization;

namespace Currency.Tests;

public class MoneyParserTests
{
    private readonly MoneyParser _parser = new();

    [Fact]
    public void Parse_AcceptsCodeBeforeAmount()
    {
        var result = _parser.Parse("GBP 12.34", new MoneyParseOptions(CultureInfo.InvariantCulture));

        Assert.True(result.Succeeded);
        Assert.Equal(Money.Of(12.34m, CurrencyCode.GBP), result.Money);
    }

    [Fact]
    public void Parse_AcceptsCodeAfterAmount()
    {
        var result = _parser.Parse("12.34 GBP", new MoneyParseOptions(CultureInfo.InvariantCulture));

        Assert.True(result.Succeeded);
        Assert.Equal(Money.Of(12.34m, CurrencyCode.GBP), result.Money);
    }

    [Fact]
    public void Parse_UsesCultureAwareDecimalSeparator()
    {
        var result = _parser.Parse("EUR 12,34", new MoneyParseOptions(new CultureInfo("fr-FR")));

        Assert.True(result.Succeeded);
        Assert.Equal(Money.Of(12.34m, CurrencyCode.EUR), result.Money);
    }

    [Fact]
    public void Parse_RequiresCurrencyWhenExpectedCurrencyIsNotSupplied()
    {
        var result = _parser.Parse("12.34", new MoneyParseOptions(CultureInfo.InvariantCulture));

        Assert.False(result.Succeeded);
        Assert.Equal(MoneyParseFailureReason.MissingCurrency, result.FailureReason);
    }

    [Fact]
    public void Parse_AcceptsCurrencylessAmountWhenExpectedCurrencyIsSupplied()
    {
        var result = _parser.Parse(
            "12.34",
            new MoneyParseOptions(CultureInfo.InvariantCulture, CurrencyCode.GBP));

        Assert.True(result.Succeeded);
        Assert.Equal(Money.Of(12.34m, CurrencyCode.GBP), result.Money);
    }

    [Fact]
    public void Parse_RejectsSymbolWithoutExpectedCurrency()
    {
        var result = _parser.Parse(
            "£12.34",
            new MoneyParseOptions(new CultureInfo("en-GB"), currencyStyles: MoneyParseCurrencyStyles.CodeOrSymbol));

        Assert.False(result.Succeeded);
        Assert.Equal(MoneyParseFailureReason.MissingCurrency, result.FailureReason);
    }

    [Fact]
    public void Parse_AcceptsSymbolWithExpectedCurrency()
    {
        var result = _parser.Parse(
            "£12.34",
            new MoneyParseOptions(
                new CultureInfo("en-GB"),
                CurrencyCode.GBP,
                MoneyParseCurrencyStyles.CodeOrSymbol));

        Assert.True(result.Succeeded);
        Assert.Equal(Money.Of(12.34m, CurrencyCode.GBP), result.Money);
    }

    [Fact]
    public void Parse_RejectsUnknownCurrencyCode()
    {
        var result = _parser.Parse("ABC 12.34", new MoneyParseOptions(CultureInfo.InvariantCulture));

        Assert.False(result.Succeeded);
        Assert.Equal(MoneyParseFailureReason.UnknownCurrency, result.FailureReason);
    }

    [Fact]
    public void Parse_RejectsMismatchedExpectedCurrency()
    {
        var result = _parser.Parse(
            "USD 12.34",
            new MoneyParseOptions(CultureInfo.InvariantCulture, CurrencyCode.GBP));

        Assert.False(result.Succeeded);
        Assert.Equal(MoneyParseFailureReason.CurrencyMismatch, result.FailureReason);
    }

    [Fact]
    public void Parse_ReturnsAmountPrecisionFailureForOverPreciseAmounts()
    {
        var result = _parser.Parse("GBP 12.345", new MoneyParseOptions(CultureInfo.InvariantCulture));

        Assert.False(result.Succeeded);
        Assert.Equal(MoneyParseFailureReason.AmountPrecision, result.FailureReason);
    }

    [Fact]
    public void TryParse_ReturnsMoneyWhenInputIsValid()
    {
        var parsed = _parser.TryParse("GBP 12.34", new MoneyParseOptions(CultureInfo.InvariantCulture), out var money);

        Assert.True(parsed);
        Assert.Equal(Money.Of(12.34m, CurrencyCode.GBP), money);
    }
}
