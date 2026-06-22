using ISOCodex.Currency;
using ISOCodex.Currency.Json.NewtonsoftJson;
using Newtonsoft.Json;

namespace Currency.Json.NewtonsoftJson.Tests;

public class CurrencyCodeJsonConverterTests
{
    private static readonly JsonSerializerSettings Settings = CreateSettings();

    [Fact]
    public void Serialize_WritesAlpha3String()
    {
        var json = JsonConvert.SerializeObject(CurrencyCode.GBP, Settings);

        Assert.Equal("\"GBP\"", json);
    }

    [Fact]
    public void Deserialize_ReadsRegisteredAlpha3String()
    {
        var currency = JsonConvert.DeserializeObject<CurrencyCode>("\"gbp\"", Settings);

        Assert.Equal(CurrencyCode.GBP, currency);
    }

    [Theory]
    [InlineData("\"GB\"")]
    [InlineData("\"ZZZ\"")]
    [InlineData("123")]
    public void Deserialize_RejectsInvalidCurrencyCode(string json)
    {
        Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<CurrencyCode>(json, Settings));
    }

    [Fact]
    public void Serialize_RejectsDefaultCurrencyCode()
    {
        Assert.Throws<JsonSerializationException>(() => JsonConvert.SerializeObject(default(CurrencyCode), Settings));
    }

    private static JsonSerializerSettings CreateSettings()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new CurrencyCodeJsonConverter());
        return settings;
    }
}
