using System;

namespace ISOCodex.Currency.Exchange.Abstractions;

/// <summary>
/// Converts money using direct exchange rates supplied by an exchange-rate provider.
/// </summary>
public sealed class MoneyConverter
{
    private readonly IExchangeRateProvider _rateProvider;
    private readonly ICurrencyRegistry _currencyRegistry;
    private readonly ICurrencyRoundingService _roundingService;

    /// <summary>
    /// Creates a converter using the default currency registry and rounding service.
    /// </summary>
    public MoneyConverter(IExchangeRateProvider rateProvider)
        : this(rateProvider, DefaultCurrencyRegistry.Instance, new CurrencyRoundingService())
    {
    }

    /// <summary>
    /// Creates a converter using explicit dependencies.
    /// </summary>
    public MoneyConverter(
        IExchangeRateProvider rateProvider,
        ICurrencyRegistry currencyRegistry,
        ICurrencyRoundingService roundingService)
    {
        _rateProvider = rateProvider ?? throw new ArgumentNullException(nameof(rateProvider));
        _currencyRegistry = currencyRegistry ?? throw new ArgumentNullException(nameof(currencyRegistry));
        _roundingService = roundingService ?? throw new ArgumentNullException(nameof(roundingService));
    }

    /// <summary>
    /// Converts money using a direct exchange rate and an explicit rounding policy.
    /// </summary>
    public ConversionResult Convert(Money input, ConversionOptions options)
    {
        if (input.IsDefault)
        {
            throw new InvalidOperationException("Input money must be initialised before conversion.");
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var pair = new CurrencyPair(input.Currency, options.TargetCurrency);
        var lookup = _rateProvider.GetRate(pair, options.EffectiveDate, options.RateKind);

        if (lookup == null)
        {
            throw new InvalidOperationException("Exchange-rate provider returned null lookup result.");
        }

        if (!lookup.Succeeded || lookup.Rate == null)
        {
            throw new InvalidOperationException(
                $"No direct exchange rate was available for '{pair}' on '{options.EffectiveDate:O}' with kind '{options.RateKind}'. {lookup.ErrorMessage}");
        }

        var rate = lookup.Rate;
        ValidateReturnedRate(pair, options, rate);

        var targetCurrency = _currencyRegistry.Get(options.TargetCurrency);
        var rawAmount = input.Amount * rate.Rate;
        var roundedAmount = _roundingService.RoundAmount(rawAmount, targetCurrency, options.RoundingPolicy);
        var output = new MoneyFactory(_currencyRegistry).Of(roundedAmount, options.TargetCurrency);

        return new ConversionResult(
            input,
            output,
            rate,
            rawAmount,
            roundedAmount,
            options.RoundingPolicy,
            options.EffectiveDate,
            options.RateKind);
    }

    private static void ValidateReturnedRate(
        CurrencyPair requestedPair,
        ConversionOptions options,
        ExchangeRate rate)
    {
        if (rate.Pair != requestedPair)
        {
            throw new InvalidOperationException(
                $"Exchange-rate provider returned '{rate.Pair}' when '{requestedPair}' was requested. Inverse and triangulated conversion are not supported by this MVP.");
        }

        if (rate.EffectiveDate != options.EffectiveDate)
        {
            throw new InvalidOperationException(
                $"Exchange-rate provider returned effective date '{rate.EffectiveDate:O}' when '{options.EffectiveDate:O}' was requested.");
        }

        if (rate.Kind != options.RateKind)
        {
            throw new InvalidOperationException(
                $"Exchange-rate provider returned kind '{rate.Kind}' when '{options.RateKind}' was requested.");
        }
    }
}
