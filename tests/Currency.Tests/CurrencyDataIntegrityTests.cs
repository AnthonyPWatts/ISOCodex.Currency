using ISOCodex.Currency;

namespace Currency.Tests;

public class CurrencyDataIntegrityTests
{
    [Fact]
    public void CurrencyCodes_AreUniqueThreeLetterUppercaseAsciiCodes()
    {
        var codes = DefaultCurrencyRegistry.Instance.All.Select(currency => currency.Code.Code).ToArray();

        Assert.All(codes, code => Assert.Matches("^[A-Z]{3}$", code));
        Assert.Equal(codes.Length, codes.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void NumericCodes_AreUniqueThreeDigitCodes()
    {
        var numericCodes = DefaultCurrencyRegistry.Instance.All
            .Select(currency => currency.NumericCode)
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .ToArray();

        Assert.All(numericCodes, code => Assert.Matches("^[0-9]{3}$", code));
        Assert.Equal(numericCodes.Length, numericCodes.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void MinorUnits_AreValidOrExplicitlyNotApplicable()
    {
        Assert.All(DefaultCurrencyRegistry.Instance.All, currency =>
        {
            if (currency.MinorUnit.IsApplicable)
            {
                Assert.True(currency.MinorUnit.DecimalPlaces >= 0);
                Assert.True(currency.MinorUnit.Increment > 0m);
            }
            else
            {
                Assert.Equal(0m, currency.MinorUnit.Increment);
            }
        });
    }

    [Fact]
    public void EveryRegistryCode_CanBeParsed()
    {
        Assert.All(DefaultCurrencyRegistry.Instance.All, currency =>
        {
            var parsed = CurrencyCode.Parse(currency.Code.Code);

            Assert.Equal(currency.Code, parsed);
        });
    }
}
