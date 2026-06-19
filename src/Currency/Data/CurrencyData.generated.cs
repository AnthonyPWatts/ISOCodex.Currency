using System;
using System.Collections.Generic;
using System.Linq;

namespace ISOCodex.Currency;

internal static class CurrencyData
{
    private static readonly CurrencyDataEntry[] Entries =
    {
        new("AED", "784", "UAE Dirham", 2, CurrencyKind.NationalCurrency, true, new[] { "AE" }),
        new("AUD", "036", "Australian Dollar", 2, CurrencyKind.NationalCurrency, true, new[] { "AU" }),
        new("BHD", "048", "Bahraini Dinar", 3, CurrencyKind.NationalCurrency, true, new[] { "BH" }),
        new("CAD", "124", "Canadian Dollar", 2, CurrencyKind.NationalCurrency, true, new[] { "CA" }),
        new("CHF", "756", "Swiss Franc", 2, CurrencyKind.NationalCurrency, true, new[] { "CH", "LI" }, 2, 0.05m),
        new("CLF", "990", "Unidad de Fomento", 4, CurrencyKind.Fund, false, new[] { "CL" }),
        new("CLP", "152", "Chilean Peso", 0, CurrencyKind.NationalCurrency, true, new[] { "CL" }),
        new("CNY", "156", "Yuan Renminbi", 2, CurrencyKind.NationalCurrency, true, new[] { "CN" }),
        new("DKK", "208", "Danish Krone", 2, CurrencyKind.NationalCurrency, true, new[] { "DK" }),
        new("EUR", "978", "Euro", 2, CurrencyKind.NationalCurrency, true, new[] { "AD", "AT", "BE", "CY", "DE", "EE", "ES", "FI", "FR", "GR", "HR", "IE", "IT", "LT", "LU", "LV", "MC", "MT", "NL", "PT", "SI", "SK", "SM", "VA" }),
        new("GBP", "826", "Pound Sterling", 2, CurrencyKind.NationalCurrency, true, new[] { "GB", "GG", "IM", "JE" }),
        new("HKD", "344", "Hong Kong Dollar", 2, CurrencyKind.NationalCurrency, true, new[] { "HK" }),
        new("ILS", "376", "New Israeli Sheqel", 2, CurrencyKind.NationalCurrency, true, new[] { "IL" }),
        new("INR", "356", "Indian Rupee", 2, CurrencyKind.NationalCurrency, true, new[] { "IN" }),
        new("JPY", "392", "Yen", 0, CurrencyKind.NationalCurrency, true, new[] { "JP" }),
        new("KRW", "410", "Won", 0, CurrencyKind.NationalCurrency, true, new[] { "KR" }),
        new("KWD", "414", "Kuwaiti Dinar", 3, CurrencyKind.NationalCurrency, true, new[] { "KW" }),
        new("MXN", "484", "Mexican Peso", 2, CurrencyKind.NationalCurrency, true, new[] { "MX" }),
        new("NOK", "578", "Norwegian Krone", 2, CurrencyKind.NationalCurrency, true, new[] { "NO" }),
        new("NZD", "554", "New Zealand Dollar", 2, CurrencyKind.NationalCurrency, true, new[] { "NZ" }),
        new("OMR", "512", "Rial Omani", 3, CurrencyKind.NationalCurrency, true, new[] { "OM" }),
        new("SEK", "752", "Swedish Krona", 2, CurrencyKind.NationalCurrency, true, new[] { "SE" }),
        new("SGD", "702", "Singapore Dollar", 2, CurrencyKind.NationalCurrency, true, new[] { "SG" }),
        new("TND", "788", "Tunisian Dinar", 3, CurrencyKind.NationalCurrency, true, new[] { "TN" }),
        new("USD", "840", "US Dollar", 2, CurrencyKind.NationalCurrency, true, new[] { "US" }),
        new("UYW", "927", "Unidad Previsional", 4, CurrencyKind.Fund, false, new[] { "UY" }),
        new("VND", "704", "Dong", 0, CurrencyKind.NationalCurrency, true, new[] { "VN" }),
        new("XAG", "961", "Silver", null, CurrencyKind.PreciousMetal, false, Array.Empty<string>()),
        new("XAU", "959", "Gold", null, CurrencyKind.PreciousMetal, false, Array.Empty<string>()),
        new("XXX", "999", "No Currency", null, CurrencyKind.NoCurrency, false, Array.Empty<string>())
    };

    public static bool ContainsCode(string code)
    {
        return Entries.Any(entry => string.Equals(entry.Code, code, StringComparison.Ordinal));
    }

    public static IReadOnlyCollection<CurrencyInfo> CreateCurrencyInfos()
    {
        return Entries
            .Select(entry => new CurrencyInfo(
                new CurrencyCode(entry.Code, true),
                entry.NumericCode,
                entry.EnglishName,
                entry.DecimalPlaces.HasValue ? new CurrencyMinorUnit(entry.DecimalPlaces.Value) : CurrencyMinorUnit.NotApplicable,
                entry.Kind,
                entry.IsTender,
                entry.Territories,
                entry.CashDecimalPlaces.HasValue ? new CurrencyMinorUnit(entry.CashDecimalPlaces.Value) : null,
                entry.CashRoundingIncrement))
            .ToArray();
    }

    private sealed class CurrencyDataEntry
    {
        public CurrencyDataEntry(
            string code,
            string numericCode,
            string englishName,
            int? decimalPlaces,
            CurrencyKind kind,
            bool isTender,
            string[] territories,
            int? cashDecimalPlaces = null,
            decimal? cashRoundingIncrement = null)
        {
            Code = code;
            NumericCode = numericCode;
            EnglishName = englishName;
            DecimalPlaces = decimalPlaces;
            Kind = kind;
            IsTender = isTender;
            Territories = territories;
            CashDecimalPlaces = cashDecimalPlaces;
            CashRoundingIncrement = cashRoundingIncrement;
        }

        public string Code { get; }

        public string NumericCode { get; }

        public string EnglishName { get; }

        public int? DecimalPlaces { get; }

        public CurrencyKind Kind { get; }

        public bool IsTender { get; }

        public string[] Territories { get; }

        public int? CashDecimalPlaces { get; }

        public decimal? CashRoundingIncrement { get; }
    }
}
