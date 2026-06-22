using System.Globalization;
using System.Linq;
using System.Text.Json;
using ISOCodex.Currency;
using ISOCodex.Currency.Countries;
using ISOCodex.Currency.Exchange.Abstractions;
using CountryAlpha2Code = ISOCodex.Countries.CountryAlpha2Code;
using NewtonsoftJson = Newtonsoft.Json.JsonConvert;

namespace Currency.ReadmeExamples.Tests;

public class ReadmeExampleTests
{
    [Fact]
    public void QuickStartExample_ResolvesCurrencyMetadata()
    {
        var code = CurrencyCode.Parse("gbp");
        var currency = DefaultCurrencyRegistry.Instance.Get(code);

        Assert.Equal("Pound Sterling", currency.EnglishName);
        Assert.Equal(2, currency.MinorUnit.DecimalPlaces);
    }

    [Fact]
    public void MoneyAndRoundingExamples_ProduceDocumentedValues()
    {
        var price = Money.Of(12.34m, CurrencyCode.GBP);
        var shipping = Money.Of(3.49m, CurrencyCode.GBP);
        var total = price + shipping;

        Assert.Equal(Money.Of(15.83m, CurrencyCode.GBP), total);

        var taxableAmount = Money.Of(10.05m, CurrencyCode.GBP);
        var toEvenTax = taxableAmount
            .Multiply(0.10m, CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven));
        var awayFromZeroTax = taxableAmount
            .Multiply(0.10m, CurrencyRoundingPolicy.AwayFromZero());

        Assert.Equal(Money.Of(1.00m, CurrencyCode.GBP), toEvenTax);
        Assert.Equal(Money.Of(1.01m, CurrencyCode.GBP), awayFromZeroTax);
    }

    [Fact]
    public void AllocationAndInstallmentExamples_PreserveTotals()
    {
        var allocation = Money.Of(10.00m, CurrencyCode.GBP)
            .Allocate(3, AllocationRemainderStrategy.Last);

        Assert.Equal(
            new[] { Money.Of(3.33m, CurrencyCode.GBP), Money.Of(3.33m, CurrencyCode.GBP), Money.Of(3.34m, CurrencyCode.GBP) },
            allocation.Parts.Select(part => part.Amount).ToArray());

        var strategy = new FixedFirstInstallmentStrategy(
            Money.Of(4.00m, CurrencyCode.GBP));

        var plan = strategy.CalculateInstallments(
            new InstallmentRequest(Money.Of(10.01m, CurrencyCode.GBP), 3));

        Assert.Equal(Money.Of(10.01m, CurrencyCode.GBP), plan.Total);
        Assert.Equal(
            new[] { Money.Of(4.00m, CurrencyCode.GBP), Money.Of(3.01m, CurrencyCode.GBP), Money.Of(3.00m, CurrencyCode.GBP) },
            plan.Installments.Select(installment => installment.Amount).ToArray());
    }

    [Fact]
    public void FormattingAndParsingExamples_RoundTripMoney()
    {
        var formatter = new MoneyFormatter();
        var price = Money.Of(12.34m, CurrencyCode.GBP);

        Assert.Equal("GBP 12.34", formatter.Format(price));
        Assert.Equal(
            "£12.34",
            formatter.Format(price, new MoneyFormatOptions(new CultureInfo("en-GB"), MoneyCurrencyDisplay.Symbol)));

        var parser = new MoneyParser();
        var result = parser.Parse("GBP 12.34", new MoneyParseOptions(CultureInfo.InvariantCulture));

        Assert.True(result.Succeeded);
        Assert.Equal(price, result.Money);
    }

    [Fact]
    public void JsonExamples_UseDocumentedWireShape()
    {
        var money = Money.Of(12.34m, CurrencyCode.GBP);
        var systemTextJsonOptions = new JsonSerializerOptions();
        systemTextJsonOptions.Converters.Add(new ISOCodex.Currency.Json.SystemTextJson.CurrencyCodeJsonConverter());
        systemTextJsonOptions.Converters.Add(new ISOCodex.Currency.Json.SystemTextJson.MoneyJsonConverter());

        var systemTextJson = JsonSerializer.Serialize(money, systemTextJsonOptions);
        var systemTextJsonRoundTrip = JsonSerializer.Deserialize<Money>(systemTextJson, systemTextJsonOptions);

        Assert.Equal("{\"amount\":12.34,\"currency\":\"GBP\"}", systemTextJson);
        Assert.Equal(money, systemTextJsonRoundTrip);

        var newtonsoftJsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
        newtonsoftJsonSettings.Converters.Add(new ISOCodex.Currency.Json.NewtonsoftJson.CurrencyCodeJsonConverter());
        newtonsoftJsonSettings.Converters.Add(new ISOCodex.Currency.Json.NewtonsoftJson.MoneyJsonConverter());

        var newtonsoftJson = NewtonsoftJson.SerializeObject(money, newtonsoftJsonSettings);
        var newtonsoftJsonRoundTrip = NewtonsoftJson.DeserializeObject<Money>(newtonsoftJson, newtonsoftJsonSettings);

        Assert.Equal("{\"amount\":12.34,\"currency\":\"GBP\"}", newtonsoftJson);
        Assert.Equal(money, newtonsoftJsonRoundTrip);
    }

    [Fact]
    public void CountryCurrencyExample_ValidatesPrimaryTender()
    {
        var result = DefaultCountryCurrencyRegistry.Instance.Validate(
            CountryAlpha2Code.Parse("GB"),
            CurrencyCode.GBP,
            CountryCurrencyValidationPolicy.PrimaryTenderOnly);

        Assert.True(result.Succeeded);
        Assert.Equal(CurrencyCode.GBP, result.CountryCurrency?.CurrencyCode);
    }

    [Fact]
    public void ExchangeExample_ConvertsWithExplicitRateAndRounding()
    {
        var effectiveDate = new DateTime(2026, 6, 22, 0, 0, 0, DateTimeKind.Utc);
        var exchangeRate = new ExchangeRate(
            new CurrencyPair(CurrencyCode.GBP, CurrencyCode.USD),
            1.2345m,
            ExchangeRateKind.MidMarket,
            effectiveDate,
            "readme-test-feed");
        var converter = new MoneyConverter(new StaticExchangeRateProvider(exchangeRate));

        var result = converter.Convert(
            Money.Of(10.00m, CurrencyCode.GBP),
            new ConversionOptions(
                CurrencyCode.USD,
                effectiveDate,
                ExchangeRateKind.MidMarket,
                CurrencyRoundingPolicy.AwayFromZero()));

        Assert.Equal(12.345000m, result.RawAmount);
        Assert.Equal(Money.Of(12.35m, CurrencyCode.USD), result.Output);
        Assert.Equal("readme-test-feed", result.RateSource);
    }

    [Fact]
    public void BoundaryAndAdvancedRegistryExamples_UsePublicApis()
    {
        var validation = Money.TryCreate(12.34m, "GBP");
        Assert.True(validation.Succeeded);
        Assert.Equal(Money.Of(12.34m, CurrencyCode.GBP), validation.Money);

        var customCode = CurrencyCode.CreateCustom("ZZA");
        var registry = new DefaultCurrencyRegistry(new[]
        {
            new CurrencyInfo(
                customCode,
                "999",
                "Internal test unit",
                new CurrencyMinorUnit(4),
                CurrencyKind.Testing,
                false)
        });

        var factory = new MoneyFactory(registry);
        var money = factory.Of(12.3456m, customCode);

        Assert.Equal(customCode, money.Currency);
        Assert.Equal(12.3456m, money.Amount);
    }

    [Fact]
    public void CurrencyDataVersionExample_ExposesRuntimeProvenance()
    {
        Assert.Equal("seed-2026-06-22-7d26419d", CurrencyDataVersion.Identifier);
        Assert.Equal("CheckedInSeed", CurrencyDataVersion.SourceKind);
        Assert.Equal(new DateTime(2026, 6, 22, 0, 0, 0, DateTimeKind.Utc), CurrencyDataVersion.CheckedOn);
        Assert.Contains("not a full ISO/CLDR snapshot", CurrencyDataVersion.Description);
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
}
