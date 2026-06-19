using System;
using System.Collections.Generic;
using System.Linq;

namespace ISOCodex.Currency;

/// <summary>
/// Represents a completed money allocation whose parts sum exactly to the original total.
/// </summary>
public sealed class MoneyAllocation
{
    /// <summary>
    /// Creates an allocation result and verifies that its parts preserve the total.
    /// </summary>
    public MoneyAllocation(Money total, IEnumerable<MoneyAllocationPart> parts)
    {
        if (parts == null)
        {
            throw new ArgumentNullException(nameof(parts));
        }

        var materializedParts = parts.ToArray();

        if (materializedParts.Length == 0)
        {
            throw new ArgumentException("Allocation must contain at least one part.", nameof(parts));
        }

        var partTotal = Money.Zero(total.Currency);

        foreach (var part in materializedParts)
        {
            partTotal += part.Amount;
        }

        if (partTotal != total)
        {
            throw new InvalidOperationException($"Allocation parts must sum to {total}. Actual total was {partTotal}.");
        }

        Total = total;
        Parts = materializedParts;
    }

    /// <summary>Gets the original total.</summary>
    public Money Total { get; }

    /// <summary>Gets the allocation parts.</summary>
    public IReadOnlyList<MoneyAllocationPart> Parts { get; }
}
