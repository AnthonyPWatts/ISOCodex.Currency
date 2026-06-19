using ISOCodex.Currency;
using System.Globalization;

namespace Currency.Tests;

public class MoneyFormatterTests
{
    private readonly MoneyFormatter _formatter = new();

    [Fact]
    public void Format_UsesCurrencyCodeByDefault()
    {
        var result = _formatter.Format(
            Money.Of(12.34m, CurrencyCode.GBP),
            new MoneyFormatOptions(CultureInfo.InvariantCulture));

        Assert.Equal("GBP 12.34", result);
    }

    [Fact]
    public void Format_UsesCultureAwareSeparators()
    {
        var result = _formatter.Format(
            Money.Of(1234.56m, CurrencyCode.EUR),
            new MoneyFormatOptions(new CultureInfo("fr-FR")));

        Assert.Equal("EUR 1\u202f234,56", result);
    }

    [Fact]
    public void Format_UsesCurrencyMinorUnitDecimalPlaces()
    {
        var result = _formatter.Format(
            Money.Of(1234m, CurrencyCode.JPY),
            new MoneyFormatOptions(CultureInfo.InvariantCulture));

        Assert.Equal("JPY 1,234", result);
    }

    [Fact]
    public void Format_CanDisplaySymbol()
    {
        var result = _formatter.Format(
            Money.Of(12.34m, CurrencyCode.GBP),
            new MoneyFormatOptions(new CultureInfo("en-GB"), MoneyCurrencyDisplay.Symbol));

        Assert.Equal("£12.34", result);
    }

    [Fact]
    public void Format_CanDisplayCodeAndSymbol()
    {
        var result = _formatter.Format(
            Money.Of(12.34m, CurrencyCode.GBP),
            new MoneyFormatOptions(new CultureInfo("en-GB"), MoneyCurrencyDisplay.CodeAndSymbol));

        Assert.Equal("GBP £12.34", result);
    }

    [Fact]
    public void Format_CanSuppressCurrencyDisplay()
    {
        var result = _formatter.Format(
            Money.Of(12.34m, CurrencyCode.GBP),
            new MoneyFormatOptions(CultureInfo.InvariantCulture, MoneyCurrencyDisplay.None));

        Assert.Equal("12.34", result);
    }
}
