using ISOCodex.Currency;

namespace Currency.Tests;

public class CurrencyCodeTests
{
    [Fact]
    public void IsDefault_ReturnsTrueForDefaultValue()
    {
        Assert.True(default(CurrencyCode).IsDefault);
    }

    [Fact]
    public void IsDefault_ReturnsFalseForInitializedValue()
    {
        Assert.False(CurrencyCode.GBP.IsDefault);
    }

    [Fact]
    public void Parse_NormalizesLowercaseCode()
    {
        var code = CurrencyCode.Parse("gbp");

        Assert.Equal(CurrencyCode.GBP, code);
        Assert.Equal("GBP", code.Code);
    }

    [Fact]
    public void CreateCustom_NormalizesAlpha3CodeWithoutRegisteringIt()
    {
        var code = CurrencyCode.CreateCustom("zza");

        Assert.Equal("ZZA", code.Code);
        Assert.False(CurrencyCode.TryParse("ZZA", out _));
        Assert.False(DefaultCurrencyRegistry.Instance.TryGet(code, out _));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("GB")]
    [InlineData("GB1")]
    [InlineData("ABCD")]
    public void CreateCustom_RejectsNonAlpha3Input(string input)
    {
        Assert.Throws<ArgumentException>(() => CurrencyCode.CreateCustom(input));
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
