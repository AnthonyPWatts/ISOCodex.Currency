using ISOCodex.Currency;

var registry = DefaultCurrencyRegistry.Instance;

foreach (var code in new[] { CurrencyCode.GBP, CurrencyCode.USD, CurrencyCode.EUR, CurrencyCode.JPY, CurrencyCode.CHF, CurrencyCode.KWD })
{
    var currency = registry.Get(code);
    var minorUnit = currency.MinorUnit.IsApplicable
        ? currency.MinorUnit.DecimalPlaces.ToString()
        : "not applicable";

    Console.WriteLine($"{currency.Code}: {currency.EnglishName} ({currency.NumericCode}), minor units: {minorUnit}");
}
