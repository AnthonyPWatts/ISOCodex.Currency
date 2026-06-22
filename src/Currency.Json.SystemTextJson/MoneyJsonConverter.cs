using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISOCodex.Currency.Json.SystemTextJson;

/// <summary>
/// Converts <see cref="Money"/> values to and from objects shaped as amount/currency pairs.
/// </summary>
public sealed class MoneyJsonConverter : JsonConverter<Money>
{
    private const string AmountPropertyName = "amount";
    private const string CurrencyPropertyName = "currency";

    /// <inheritdoc />
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Money JSON value must be an object.");
        }

        decimal? amount = null;
        string? currencyCode = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return CreateMoney(amount, currencyCode);
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Money JSON object contains an invalid token.");
            }

            var propertyName = reader.GetString();
            if (!reader.Read())
            {
                throw new JsonException("Money JSON object ended unexpectedly.");
            }

            if (string.Equals(propertyName, AmountPropertyName, StringComparison.Ordinal))
            {
                amount = reader.GetDecimal();
            }
            else if (string.Equals(propertyName, CurrencyPropertyName, StringComparison.Ordinal))
            {
                currencyCode = reader.GetString();
            }
            else
            {
                reader.Skip();
            }
        }

        throw new JsonException("Money JSON object ended unexpectedly.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
    {
        if (value.IsDefault)
        {
            throw new JsonException("Money value must be initialised before it can be serialised.");
        }

        writer.WriteStartObject();
        writer.WriteNumber(AmountPropertyName, value.Amount);
        writer.WriteString(CurrencyPropertyName, value.Currency.Code);
        writer.WriteEndObject();
    }

    private static Money CreateMoney(decimal? amount, string? currencyCode)
    {
        if (!amount.HasValue)
        {
            throw new JsonException("Money JSON object must contain an amount property.");
        }

        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            throw new JsonException("Money JSON object must contain a currency property.");
        }

        var result = Money.TryCreate(amount.Value, currencyCode);
        if (!result.Succeeded)
        {
            throw new JsonException(result.ErrorMessage);
        }

        return result.Money.GetValueOrDefault();
    }
}
