using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ISOCodex.Currency;

/// <summary>
/// Conservative parser for money values.
/// </summary>
public sealed class MoneyParser
{
    private static readonly Regex CodeTokenPattern = new Regex(
        @"(?<![A-Za-z])([A-Za-z]{3})(?![A-Za-z])",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    /// <summary>
    /// Parses a money value using default options.
    /// </summary>
    public MoneyParseResult Parse(string? input)
    {
        return Parse(input, MoneyParseOptions.Default);
    }

    /// <summary>
    /// Parses a money value using explicit options.
    /// </summary>
    public MoneyParseResult Parse(string? input, MoneyParseOptions? options)
    {
        options ??= MoneyParseOptions.Default;

        if (string.IsNullOrWhiteSpace(input))
        {
            return MoneyParseResult.Failure(MoneyParseFailureReason.EmptyInput, "Money input cannot be empty.");
        }

        var text = input.Trim();
        var codeResult = ExtractCurrencyCode(text, options);

        if (codeResult.Result != null)
        {
            return codeResult.Result;
        }

        var currency = codeResult.Currency ?? options.ExpectedCurrency;
        var amountText = codeResult.AmountText ?? text;

        if (!currency.HasValue)
        {
            return MoneyParseResult.Failure(
                MoneyParseFailureReason.MissingCurrency,
                "Money input must include an alpha-3 currency code unless an expected currency is supplied.");
        }

        var amountStyles = NumberStyles.Number;

        if (codeResult.Currency == null
            && options.CurrencyStyles.HasFlag(MoneyParseCurrencyStyles.Symbol))
        {
            amountStyles |= NumberStyles.AllowCurrencySymbol;
        }

        var numberFormat = CreateNumberFormat(options.Culture, currency.Value);

        if (!decimal.TryParse(amountText.Trim(), amountStyles, numberFormat, out var amount))
        {
            return MoneyParseResult.Failure(
                MoneyParseFailureReason.InvalidAmount,
                $"'{amountText.Trim()}' is not a valid money amount for culture '{options.Culture.Name}'.");
        }

        try
        {
            return MoneyParseResult.Success(Money.Of(amount, currency.Value));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return MoneyParseResult.Failure(MoneyParseFailureReason.AmountPrecision, ex.Message);
        }
    }

    /// <summary>
    /// Attempts to parse a money value using default options.
    /// </summary>
    public bool TryParse(string? input, out Money money)
    {
        return TryParse(input, MoneyParseOptions.Default, out money);
    }

    /// <summary>
    /// Attempts to parse a money value using explicit options.
    /// </summary>
    public bool TryParse(string? input, MoneyParseOptions? options, out Money money)
    {
        var result = Parse(input, options);

        if (result.Succeeded)
        {
            money = result.Money!.Value;
            return true;
        }

        money = default;
        return false;
    }

    private static CodeExtractionResult ExtractCurrencyCode(string text, MoneyParseOptions options)
    {
        if (!options.CurrencyStyles.HasFlag(MoneyParseCurrencyStyles.Code))
        {
            return new CodeExtractionResult(null, null, null);
        }

        var matches = CodeTokenPattern.Matches(text);
        var knownMatches = new List<Match>();
        var unknownCode = false;

        foreach (Match match in matches)
        {
            var codeText = match.Groups[1].Value;

            if (CurrencyCode.TryParse(codeText, out _))
            {
                knownMatches.Add(match);
            }
            else
            {
                unknownCode = true;
            }
        }

        if (knownMatches.Count == 0)
        {
            if (unknownCode)
            {
                return new CodeExtractionResult(
                    null,
                    null,
                    MoneyParseResult.Failure(MoneyParseFailureReason.UnknownCurrency, "Money input contains an unknown currency code."));
            }

            return new CodeExtractionResult(null, null, null);
        }

        if (knownMatches.Count > 1)
        {
            return new CodeExtractionResult(
                null,
                null,
                MoneyParseResult.Failure(MoneyParseFailureReason.AmbiguousCurrency, "Money input contains more than one currency code."));
        }

        var code = CurrencyCode.Parse(knownMatches[0].Groups[1].Value);

        if (options.ExpectedCurrency.HasValue && code != options.ExpectedCurrency.Value)
        {
            return new CodeExtractionResult(
                null,
                null,
                MoneyParseResult.Failure(
                    MoneyParseFailureReason.CurrencyMismatch,
                    $"Money input used currency '{code}' but expected '{options.ExpectedCurrency.Value}'."));
        }

        var amountText = RemoveMatch(text, knownMatches[0]);
        return new CodeExtractionResult(code, amountText, null);
    }

    private static string RemoveMatch(string text, Match match)
    {
        return text.Remove(match.Index, match.Length).Trim();
    }

    private static NumberFormatInfo CreateNumberFormat(CultureInfo culture, CurrencyCode currency)
    {
        var numberFormat = (NumberFormatInfo)culture.NumberFormat.Clone();
        numberFormat.CurrencySymbol = MoneyFormatter.GetSymbol(currency, culture);
        return numberFormat;
    }

    private sealed class CodeExtractionResult
    {
        public CodeExtractionResult(CurrencyCode? currency, string? amountText, MoneyParseResult? result)
        {
            Currency = currency;
            AmountText = amountText;
            Result = result;
        }

        public CurrencyCode? Currency { get; }

        public string? AmountText { get; }

        public MoneyParseResult? Result { get; }
    }
}
