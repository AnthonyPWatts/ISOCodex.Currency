using System;
using System.Globalization;
using Newtonsoft.Json;

namespace ISOCodex.Currency.Json.NewtonsoftJson;

/// <summary>
/// Converts <see cref="Money"/> values to and from objects shaped as amount/currency pairs.
/// </summary>
public sealed class MoneyJsonConverter : JsonConverter<Money>
{
    private const string AmountPropertyName = "amount";
    private const string CurrencyPropertyName = "currency";

    /// <inheritdoc />
    public override Money ReadJson(
        JsonReader reader,
        Type objectType,
        Money existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.StartObject)
        {
            throw new JsonSerializationException("Money JSON value must be an object.");
        }

        decimal? amount = null;
        string? currencyCode = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return CreateMoney(amount, currencyCode);
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                throw new JsonSerializationException("Money JSON object contains an invalid token.");
            }

            var propertyName = reader.Value?.ToString();
            if (!reader.Read())
            {
                throw new JsonSerializationException("Money JSON object ended unexpectedly.");
            }

            if (string.Equals(propertyName, AmountPropertyName, StringComparison.Ordinal))
            {
                amount = ReadAmount(reader);
            }
            else if (string.Equals(propertyName, CurrencyPropertyName, StringComparison.Ordinal))
            {
                currencyCode = ReadCurrencyCode(reader);
            }
            else
            {
                reader.Skip();
            }
        }

        throw new JsonSerializationException("Money JSON object ended unexpectedly.");
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, Money value, JsonSerializer serializer)
    {
        if (value.IsDefault)
        {
            throw new JsonSerializationException("Money value must be initialised before it can be serialised.");
        }

        writer.WriteStartObject();
        writer.WritePropertyName(AmountPropertyName);
        writer.WriteValue(value.Amount);
        writer.WritePropertyName(CurrencyPropertyName);
        writer.WriteValue(value.Currency.Code);
        writer.WriteEndObject();
    }

    private static decimal ReadAmount(JsonReader reader)
    {
        if (reader.TokenType != JsonToken.Integer && reader.TokenType != JsonToken.Float)
        {
            throw new JsonSerializationException("Money JSON amount property must be a number.");
        }

        return Convert.ToDecimal(reader.Value, CultureInfo.InvariantCulture);
    }

    private static string? ReadCurrencyCode(JsonReader reader)
    {
        if (reader.TokenType != JsonToken.String)
        {
            throw new JsonSerializationException("Money JSON currency property must be a string.");
        }

        return reader.Value?.ToString();
    }

    private static Money CreateMoney(decimal? amount, string? currencyCode)
    {
        if (!amount.HasValue)
        {
            throw new JsonSerializationException("Money JSON object must contain an amount property.");
        }

        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            throw new JsonSerializationException("Money JSON object must contain a currency property.");
        }

        var result = Money.TryCreate(amount.Value, currencyCode);
        if (!result.Succeeded)
        {
            throw new JsonSerializationException(result.ErrorMessage ?? $"Money JSON object is invalid: {result.FailureReason}.");
        }

        return result.Money.GetValueOrDefault();
    }
}
