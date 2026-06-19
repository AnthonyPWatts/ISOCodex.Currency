using ISOCodex.Currency;

namespace Currency.Tests;

public class CurrencyRegistryTests
{
    [Theory]
    [InlineData("GBP", "826", 2)]
    [InlineData("USD", "840", 2)]
    [InlineData("EUR", "978", 2)]
    [InlineData("JPY", "392", 0)]
    [InlineData("CHF", "756", 2)]
    [InlineData("KWD", "414", 3)]
    [InlineData("BHD", "048", 3)]
    [InlineData("CLF", "990", 4)]
    [InlineData("UYW", "927", 4)]
    public void Registry_ResolvesCommonCurrencyMetadata(string code, string numericCode, int decimalPlaces)
    {
        var currency = DefaultCurrencyRegistry.Instance.Get(CurrencyCode.Parse(code));

        Assert.Equal(code, currency.Code.Code);
        Assert.Equal(numericCode, currency.NumericCode);
        Assert.True(currency.MinorUnit.IsApplicable);
        Assert.Equal(decimalPlaces, currency.MinorUnit.DecimalPlaces);
    }

    [Fact]
    public void Registry_ResolvesNoCurrencyMetadata()
    {
        var currency = DefaultCurrencyRegistry.Instance.Get(CurrencyCode.XXX);

        Assert.Equal(CurrencyKind.NoCurrency, currency.Kind);
        Assert.False(currency.IsTender);
        Assert.False(currency.MinorUnit.IsApplicable);
    }

    [Fact]
    public void Registry_ResolvesByNumericCode()
    {
        var currency = DefaultCurrencyRegistry.Instance.GetByNumericCode("826");

        Assert.Equal(CurrencyCode.GBP, currency.Code);
    }

    [Fact]
    public void Registry_ReturnsFalseForUnknownNumericCode()
    {
        var found = DefaultCurrencyRegistry.Instance.TryGetByNumericCode("000", out _);

        Assert.False(found);
    }
}
