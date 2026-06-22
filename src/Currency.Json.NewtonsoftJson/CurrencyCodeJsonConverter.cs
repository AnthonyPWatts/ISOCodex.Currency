using System;
using Newtonsoft.Json;

namespace ISOCodex.Currency.Json.NewtonsoftJson;

/// <summary>
/// Converts <see cref="CurrencyCode"/> values to and from alpha-3 JSON strings.
/// </summary>
public sealed class CurrencyCodeJsonConverter : JsonConverter<CurrencyCode>
{
    /// <inheritdoc />
    public override CurrencyCode ReadJson(
        JsonReader reader,
        Type objectType,
        CurrencyCode existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.String)
        {
            throw new JsonSerializationException("Currency code JSON value must be a string.");
        }

        var value = reader.Value?.ToString();
        if (!CurrencyCode.TryParse(value, out var currencyCode))
        {
            throw new JsonSerializationException($"Currency code '{value}' is not a registered ISO 4217-style currency code.");
        }

        return currencyCode;
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, CurrencyCode value, JsonSerializer serializer)
    {
        if (value.IsDefault)
        {
            throw new JsonSerializationException("Currency code must be initialised before it can be serialised.");
        }

        writer.WriteValue(value.Code);
    }
}
