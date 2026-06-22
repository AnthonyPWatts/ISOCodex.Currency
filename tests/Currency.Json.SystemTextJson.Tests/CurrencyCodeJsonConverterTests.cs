using System.Text.Json;
using ISOCodex.Currency;
using ISOCodex.Currency.Json.SystemTextJson;

namespace Currency.Json.SystemTextJson.Tests;

public class CurrencyCodeJsonConverterTests
{
    private static readonly JsonSerializerOptions Options = CreateOptions();

    [Fact]
    public void Serialize_WritesAlpha3String()
    {
        var json = JsonSerializer.Serialize(CurrencyCode.GBP, Options);

        Assert.Equal("\"GBP\"", json);
    }

    [Fact]
    public void Deserialize_ReadsRegisteredAlpha3String()
    {
        var currency = JsonSerializer.Deserialize<CurrencyCode>("\"gbp\"", Options);

        Assert.Equal(CurrencyCode.GBP, currency);
    }

    [Theory]
    [InlineData("\"GB\"")]
    [InlineData("\"ZZZ\"")]
    [InlineData("123")]
    public void Deserialize_RejectsInvalidCurrencyCode(string json)
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CurrencyCode>(json, Options));
    }

    [Fact]
    public void Serialize_RejectsDefaultCurrencyCode()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Serialize(default(CurrencyCode), Options));
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new CurrencyCodeJsonConverter());
        return options;
    }
}
