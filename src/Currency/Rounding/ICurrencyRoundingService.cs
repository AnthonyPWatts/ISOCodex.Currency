namespace ISOCodex.Currency;

/// <summary>
/// Rounds currency amounts according to explicit rounding policies.
/// </summary>
public interface ICurrencyRoundingService
{
    /// <summary>
    /// Rounds a money value and returns a valid money value in the same currency.
    /// </summary>
    Money Round(Money money, CurrencyRoundingPolicy policy);

    /// <summary>
    /// Rounds a raw amount using the supplied currency metadata.
    /// </summary>
    decimal RoundAmount(decimal amount, CurrencyInfo currency, CurrencyRoundingPolicy policy);
}
