using System;

namespace ISOCodex.Currency;

/// <summary>
/// Represents a numbered money installment.
/// </summary>
public sealed class Installment
{
    /// <summary>
    /// Creates an installment.
    /// </summary>
    public Installment(int installmentNumber, Money amount)
    {
        if (installmentNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(installmentNumber), "Installment number must be greater than zero.");
        }

        InstallmentNumber = installmentNumber;
        Amount = amount;
    }

    /// <summary>Gets the one-based installment number.</summary>
    public int InstallmentNumber { get; }

    /// <summary>Gets the installment amount.</summary>
    public Money Amount { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{InstallmentNumber}: {Amount}";
    }
}
