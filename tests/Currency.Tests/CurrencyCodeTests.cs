using ISOCodex.Currency;

namespace Currency.Tests;

public class CurrencyCodeTests
{
    [Fact]
    public void Parse_NormalizesLowercaseCode()
    {
        var code = CurrencyCode.Parse("gbp");

        Assert.Equal(CurrencyCode.GBP, code);
        Assert.Equal("GBP", code.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("GB")]
    [InlineData("US")]
    [InlineData("ABC")]
    [InlineData("123")]
    [InlineData("GB1")]
    public void TryParse_RejectsInvalidCodes(string input)
    {
        var parsed = CurrencyCode.TryParse(input, out _);

        Assert.False(parsed);
    }

    [Fact]
    public void All_MatchesDefaultRegistryCount()
    {
        Assert.Equal(DefaultCurrencyRegistry.Instance.All.Count, CurrencyCode.All.Count());
    }
}
