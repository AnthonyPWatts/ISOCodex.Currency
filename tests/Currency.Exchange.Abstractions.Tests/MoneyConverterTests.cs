using ISOCodex.Currency;
using ISOCodex.Currency.Exchange.Abstractions;

namespace Currency.Exchange.Abstractions.Tests;

public class MoneyConverterTests
{
    private static readonly DateTime EffectiveDate = new(2026, 6, 22, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Convert_UsesDirectRateAndReturnsAuditFields()
    {
        var rate = new ExchangeRate(
            new CurrencyPair(CurrencyCode.GBP, CurrencyCode.USD),
            1.25m,
            ExchangeRateKind.MidMarket,
            EffectiveDate,
            "unit-test-feed");
        var roundingPolicy = CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven);
        var converter = new MoneyConverter(new StaticExchangeRateProvider(rate));

        var result = converter.Convert(
            Money.Of(10.00m, CurrencyCode.GBP),
            new ConversionOptions(CurrencyCode.USD, EffectiveDate, ExchangeRateKind.MidMarket, roundingPolicy));

        Assert.Equal(Money.Of(10.00m, CurrencyCode.GBP), result.Input);
        Assert.Equal(Money.Of(12.50m, CurrencyCode.USD), result.Output);
        Assert.Same(rate, result.Rate);
        Assert.Equal(12.50m, result.RawAmount);
        Assert.Equal(12.50m, result.RoundedAmount);
        Assert.Same(roundingPolicy, result.RoundingPolicy);
        Assert.Equal(EffectiveDate, result.RequestedEffectiveDate);
        Assert.Equal(ExchangeRateKind.MidMarket, result.RequestedRateKind);
        Assert.Equal(CurrencyCode.GBP, result.SourceCurrency);
        Assert.Equal(CurrencyCode.USD, result.TargetCurrency);
        Assert.Equal("unit-test-feed", result.RateSource);
    }

    [Fact]
    public void Convert_RequiresExplicitRoundingPolicy()
    {
        var rate = new ExchangeRate(
            new CurrencyPair(CurrencyCode.GBP, CurrencyCode.USD),
            1.2345m,
            ExchangeRateKind.MidMarket,
            EffectiveDate,
            "unit-test-feed");
        var converter = new MoneyConverter(new StaticExchangeRateProvider(rate));

        var result = converter.Convert(
            Money.Of(10.00m, CurrencyCode.GBP),
            new ConversionOptions(
                CurrencyCode.USD,
                EffectiveDate,
                ExchangeRateKind.MidMarket,
                CurrencyRoundingPolicy.AwayFromZero()));

        Assert.Equal(12.345000m, result.RawAmount);
        Assert.Equal(12.35m, result.RoundedAmount);
        Assert.Equal(Money.Of(12.35m, CurrencyCode.USD), result.Output);
    }

    [Fact]
    public void ConversionOptions_RejectsMissingRoundingPolicy()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new ConversionOptions(
                CurrencyCode.USD,
                EffectiveDate,
                ExchangeRateKind.MidMarket,
                null!));

        Assert.Equal("roundingPolicy", exception.ParamName);
    }

    [Fact]
    public void Convert_DoesNotUseInverseRates()
    {
        var inverseRate = new ExchangeRate(
            new CurrencyPair(CurrencyCode.USD, CurrencyCode.GBP),
            0.80m,
            ExchangeRateKind.MidMarket,
            EffectiveDate,
            "unit-test-feed");
        var converter = new MoneyConverter(new LenientExchangeRateProvider(inverseRate));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            converter.Convert(
                Money.Of(10.00m, CurrencyCode.GBP),
                new ConversionOptions(
                    CurrencyCode.USD,
                    EffectiveDate,
                    ExchangeRateKind.MidMarket,
                    CurrencyRoundingPolicy.Standard())));

        Assert.Contains("Inverse and triangulated conversion are not supported", exception.Message);
    }

    [Fact]
    public void CoreCurrencyPackage_DoesNotDependOnExchangeAbstractions()
    {
        var projectFile = Path.Combine(FindRepositoryRoot(), "src", "Currency", "Currency.csproj");
        var projectText = File.ReadAllText(projectFile);

        Assert.DoesNotContain("Currency.Exchange.Abstractions", projectText, StringComparison.Ordinal);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "ISOCodex.Currency.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find the repository root.");
    }

    private sealed class StaticExchangeRateProvider : IExchangeRateProvider
    {
        private readonly ExchangeRate _rate;

        public StaticExchangeRateProvider(ExchangeRate rate)
        {
            _rate = rate;
        }

        public ExchangeRateLookupResult GetRate(
            CurrencyPair pair,
            DateTime effectiveDate,
            ExchangeRateKind rateKind)
        {
            if (_rate.Pair == pair && _rate.EffectiveDate == effectiveDate && _rate.Kind == rateKind)
            {
                return ExchangeRateLookupResult.Success(_rate);
            }

            return ExchangeRateLookupResult.Failure(
                ExchangeRateLookupFailureReason.RateUnavailable,
                $"No direct rate for {pair} on {effectiveDate:O}.");
        }
    }

    private sealed class LenientExchangeRateProvider : IExchangeRateProvider
    {
        private readonly ExchangeRate _rate;

        public LenientExchangeRateProvider(ExchangeRate rate)
        {
            _rate = rate;
        }

        public ExchangeRateLookupResult GetRate(
            CurrencyPair pair,
            DateTime effectiveDate,
            ExchangeRateKind rateKind)
        {
            return ExchangeRateLookupResult.Success(_rate);
        }
    }
}
