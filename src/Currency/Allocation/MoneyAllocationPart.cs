using System;

namespace ISOCodex.Currency;

/// <summary>
/// Represents one part of an allocated money value.
/// </summary>
public sealed class MoneyAllocationPart
{
    /// <summary>
    /// Creates an allocation part.
    /// </summary>
    public MoneyAllocationPart(int partNumber, Money amount)
    {
        if (partNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(partNumber), "Part number must be greater than zero.");
        }

        PartNumber = partNumber;
        Amount = amount;
    }

    /// <summary>Gets the one-based part number.</summary>
    public int PartNumber { get; }

    /// <summary>Gets the allocated amount.</summary>
    public Money Amount { get; }
}
