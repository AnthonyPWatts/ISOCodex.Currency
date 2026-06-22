using ISOCodex.Currency;
using ISOCodex.Currency.Json.NewtonsoftJson;
using Newtonsoft.Json;

namespace Currency.Json.NewtonsoftJson.Tests;

public class MoneyJsonConverterTests
{
    private static readonly JsonSerializerSettings Settings = CreateSettings();

    [Fact]
    public void Serialize_WritesAmountCurrencyObject()
    {
        var json = JsonConvert.SerializeObject(Money.Of(12.34m, CurrencyCode.GBP), Settings);

        Assert.Equal("{\"amount\":12.34,\"currency\":\"GBP\"}", json);
    }

    [Fact]
    public void Deserialize_ReadsAmountCurrencyObject()
    {
        var money = JsonConvert.DeserializeObject<Money>("{\"amount\":12.34,\"currency\":\"GBP\"}", Settings);

        Assert.Equal(Money.Of(12.34m, CurrencyCode.GBP), money);
    }

    [Fact]
    public void RoundTrip_PreservesValidMoney()
    {
        var expected = Money.Of(1.234m, CurrencyCode.KWD);

        var json = JsonConvert.SerializeObject(expected, Settings);
        var actual = JsonConvert.DeserializeObject<Money>(json, Settings);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Deserialize_RejectsUnknownCurrency()
    {
        Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<Money>("{\"amount\":12.34,\"currency\":\"ZZZ\"}", Settings));
    }

    [Fact]
    public void Deserialize_RejectsOverPreciseAmount()
    {
        var exception = Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<Money>("{\"amount\":12.345,\"currency\":\"GBP\"}", Settings));

        Assert.Contains("GBP", exception.Message);
    }

    [Fact]
    public void Deserialize_RejectsFractionalZeroMinorUnitAmount()
    {
        Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<Money>("{\"amount\":100.01,\"currency\":\"JPY\"}", Settings));
    }

    [Theory]
    [InlineData("{\"currency\":\"GBP\"}")]
    [InlineData("{\"amount\":12.34}")]
    public void Deserialize_RejectsMissingRequiredProperties(string json)
    {
        Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<Money>(json, Settings));
    }

    [Fact]
    public void Deserialize_RejectsStringAmountByDefault()
    {
        Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<Money>("{\"amount\":\"12.34\",\"currency\":\"GBP\"}", Settings));
    }

    [Fact]
    public void Serialize_RejectsDefaultMoney()
    {
        Assert.Throws<JsonSerializationException>(() => JsonConvert.SerializeObject(default(Money), Settings));
    }

    private static JsonSerializerSettings CreateSettings()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new CurrencyCodeJsonConverter());
        settings.Converters.Add(new MoneyJsonConverter());
        return settings;
    }
}
