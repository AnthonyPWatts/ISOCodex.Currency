using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISOCodex.Currency.Json.SystemTextJson;

/// <summary>
/// Converts <see cref="CurrencyCode"/> values to and from alpha-3 JSON strings.
/// </summary>
public sealed class CurrencyCodeJsonConverter : JsonConverter<CurrencyCode>
{
    /// <inheritdoc />
    public override CurrencyCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Currency code JSON value must be a string.");
        }

        var value = reader.GetString();
        if (!CurrencyCode.TryParse(value, out var currencyCode))
        {
            throw new JsonException($"Currency code '{value}' is not a registered ISO 4217-style currency code.");
        }

        return currencyCode;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, CurrencyCode value, JsonSerializerOptions options)
    {
        if (value.IsDefault)
        {
            throw new JsonException("Currency code must be initialised before it can be serialised.");
        }

        writer.WriteStringValue(value.Code);
    }
}
