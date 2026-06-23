using System.Text.Json;
using ISOCodex.Currency;
using ISOCodex.Currency.Json.SystemTextJson;

namespace Currency.Json.SystemTextJson.Tests;

public class MoneyJsonConverterTests
{
    private static readonly JsonSerializerOptions Options = CreateOptions();

    [Fact]
    public void Serialize_WritesAmountCurrencyObject()
    {
        var json = JsonSerializer.Serialize(Money.Of(12.34m, CurrencyCode.GBP), Options);

        Assert.Equal("{\"amount\":12.34,\"currency\":\"GBP\"}", json);
    }

    [Fact]
    public void Deserialize_ReadsAmountCurrencyObject()
    {
        var money = JsonSerializer.Deserialize<Money>("{\"amount\":12.34,\"currency\":\"GBP\"}", Options);

        Assert.Equal(Money.Of(12.34m, CurrencyCode.GBP), money);
    }

    [Fact]
    public void RoundTrip_PreservesValidMoney()
    {
        var expected = Money.Of(1.234m, CurrencyCode.KWD);

        var json = JsonSerializer.Serialize(expected, Options);
        var actual = JsonSerializer.Deserialize<Money>(json, Options);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Deserialize_RejectsUnknownCurrency()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Money>("{\"amount\":12.34,\"currency\":\"ZZZ\"}", Options));
    }

    [Fact]
    public void Deserialize_RejectsOverPreciseAmount()
    {
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Money>("{\"amount\":12.345,\"currency\":\"GBP\"}", Options));

        Assert.Contains("GBP", exception.Message);
    }

    [Fact]
    public void Deserialize_RejectsFractionalZeroMinorUnitAmount()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Money>("{\"amount\":100.01,\"currency\":\"JPY\"}", Options));
    }

    [Theory]
    [InlineData("{\"currency\":\"GBP\"}")]
    [InlineData("{\"amount\":12.34}")]
    public void Deserialize_RejectsMissingRequiredProperties(string json)
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Money>(json, Options));
    }

    [Fact]
    public void Deserialize_RejectsStringAmountByDefault()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Money>("{\"amount\":\"12.34\",\"currency\":\"GBP\"}", Options));
    }

    [Fact]
    public void Deserialize_RejectsNonStringCurrency()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Money>("{\"amount\":12.34,\"currency\":826}", Options));
    }

    [Fact]
    public void Serialize_RejectsDefaultMoney()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Serialize(default(Money), Options));
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new CurrencyCodeJsonConverter());
        options.Converters.Add(new MoneyJsonConverter());
        return options;
    }
}
