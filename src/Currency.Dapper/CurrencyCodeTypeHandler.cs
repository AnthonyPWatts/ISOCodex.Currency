using System;
using System.Data;
using System.Globalization;
using Dapper;

namespace ISOCodex.Currency.Dapper;

/// <summary>
/// Maps <see cref="CurrencyCode"/> values to and from alpha-3 database strings.
/// </summary>
public sealed class CurrencyCodeTypeHandler : SqlMapper.TypeHandler<CurrencyCode>
{
    /// <summary>Gets a reusable handler instance.</summary>
    public static CurrencyCodeTypeHandler Instance { get; } = new CurrencyCodeTypeHandler();

    /// <inheritdoc />
    public override void SetValue(IDbDataParameter parameter, CurrencyCode value)
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (value.IsDefault)
        {
            throw new InvalidOperationException("Currency code must be initialised before it can be used as a Dapper parameter.");
        }

        parameter.DbType = DbType.String;
        parameter.Size = 3;
        parameter.Value = value.Code;
    }

    /// <inheritdoc />
    public override CurrencyCode Parse(object value)
    {
        if (value == null || value is DBNull)
        {
            throw new DataException("Currency code database value cannot be null.");
        }

        if (value is CurrencyCode currencyCode)
        {
            return currencyCode;
        }

        var text = value as string ?? Convert.ToString(value, CultureInfo.InvariantCulture);
        if (!CurrencyCode.TryParse(text, out var parsed))
        {
            throw new DataException($"Currency code database value '{text}' is not a registered ISO 4217-style currency code.");
        }

        return parsed;
    }
}
