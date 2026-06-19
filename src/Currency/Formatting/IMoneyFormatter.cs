namespace ISOCodex.Currency;

/// <summary>
/// Formats money values for display.
/// </summary>
public interface IMoneyFormatter
{
    /// <summary>
    /// Formats a money value using the formatter's default options.
    /// </summary>
    string Format(Money money);

    /// <summary>
    /// Formats a money value using explicit options.
    /// </summary>
    string Format(Money money, MoneyFormatOptions? options);
}
