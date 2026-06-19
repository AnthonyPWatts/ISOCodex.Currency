using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ISOCodex.Currency;

/// <summary>
/// Default culture-aware money formatter.
/// </summary>
public sealed class MoneyFormatter : IMoneyFormatter
{
    private static readonly Lazy<IReadOnlyDictionary<CurrencyCode, string>> SymbolsByCurrency =
        new Lazy<IReadOnlyDictionary<CurrencyCode, string>>(CreateSymbolLookup);

    private readonly ICurrencyRegistry _registry;
    private readonly MoneyFormatOptions _defaultOptions;

    /// <summary>
    /// Creates a formatter backed by the default currency registry.
    /// </summary>
    public MoneyFormatter()
        : this(DefaultCurrencyRegistry.Instance, MoneyFormatOptions.Default)
    {
    }

    /// <summary>
    /// Creates a formatter with explicit dependencies and defaults.
    /// </summary>
    public MoneyFormatter(ICurrencyRegistry? registry = null, MoneyFormatOptions? defaultOptions = null)
    {
        _registry = registry ?? DefaultCurrencyRegistry.Instance;
        _defaultOptions = defaultOptions ?? MoneyFormatOptions.Default;
    }

    /// <inheritdoc />
    public string Format(Money money)
    {
        return Format(money, _defaultOptions);
    }

    /// <inheritdoc />
    public string Format(Money money, MoneyFormatOptions? options)
    {
        options ??= _defaultOptions;

        var currency = _registry.Get(money.Currency);
        var numberFormat = CreateNumberFormat(options.Culture, currency, options.UseCurrencyDecimalPlaces);

        switch (options.CurrencyDisplay)
        {
            case MoneyCurrencyDisplay.None:
                return money.Amount.ToString("N", numberFormat);

            case MoneyCurrencyDisplay.Symbol:
                numberFormat.CurrencySymbol = GetSymbol(money.Currency, options.Culture);
                return money.Amount.ToString("C", numberFormat);

            case MoneyCurrencyDisplay.CodeAndSymbol:
                numberFormat.CurrencySymbol = GetSymbol(money.Currency, options.Culture);
                return string.Format(
                    options.Culture,
                    "{0} {1}",
                    money.Currency,
                    money.Amount.ToString("C", numberFormat));

            case MoneyCurrencyDisplay.Code:
                return string.Format(
                    options.Culture,
                    "{0} {1}",
                    money.Currency,
                    money.Amount.ToString("N", numberFormat));

            default:
                throw new ArgumentOutOfRangeException(nameof(options), options.CurrencyDisplay, "Unknown currency display option.");
        }
    }

    internal static string GetSymbol(CurrencyCode currencyCode, CultureInfo culture)
    {
        if (TryGetRegion(culture, out var region)
            && string.Equals(region.ISOCurrencySymbol, currencyCode.Code, StringComparison.OrdinalIgnoreCase))
        {
            return region.CurrencySymbol;
        }

        if (SymbolsByCurrency.Value.TryGetValue(currencyCode, out var symbol))
        {
            return symbol;
        }

        return currencyCode.Code;
    }

    private static NumberFormatInfo CreateNumberFormat(
        CultureInfo culture,
        CurrencyInfo currency,
        bool useCurrencyDecimalPlaces)
    {
        var numberFormat = (NumberFormatInfo)culture.NumberFormat.Clone();

        if (useCurrencyDecimalPlaces)
        {
            var decimalPlaces = currency.MinorUnit.IsApplicable
                ? currency.MinorUnit.DecimalPlaces
                : 0;

            numberFormat.NumberDecimalDigits = decimalPlaces;
            numberFormat.CurrencyDecimalDigits = decimalPlaces;
        }

        return numberFormat;
    }

    private static IReadOnlyDictionary<CurrencyCode, string> CreateSymbolLookup()
    {
        return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(CreateRegion)
            .Where(region => region != null && CurrencyCode.TryParse(region.ISOCurrencySymbol, out _))
            .GroupBy(region => CurrencyCode.Parse(region!.ISOCurrencySymbol))
            .ToDictionary(group => group.Key, group => group.First()!.CurrencySymbol);
    }

    private static RegionInfo? CreateRegion(CultureInfo culture)
    {
        try
        {
            return new RegionInfo(culture.Name);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private static bool TryGetRegion(CultureInfo culture, out RegionInfo region)
    {
        region = null!;

        if (string.IsNullOrWhiteSpace(culture.Name))
        {
            return false;
        }

        try
        {
            region = new RegionInfo(culture.Name);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
