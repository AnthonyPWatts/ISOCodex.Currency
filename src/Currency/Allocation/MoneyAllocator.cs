using System;
using System.Collections.Generic;

namespace ISOCodex.Currency;

/// <summary>
/// Allocates money into exact minor-unit parts while preserving the original total.
/// </summary>
public sealed class MoneyAllocator
{
    /// <summary>
    /// Allocates a money value into the requested number of parts.
    /// </summary>
    public MoneyAllocation Allocate(
        Money total,
        int parts,
        AllocationRemainderStrategy remainderStrategy = AllocationRemainderStrategy.First)
    {
        if (parts <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(parts), "Number of parts must be greater than zero.");
        }

        var totalMinorUnits = total.ToMinorUnits();
        var sign = totalMinorUnits < 0 ? -1L : 1L;
        var absoluteMinorUnits = GetAbsoluteMinorUnits(totalMinorUnits);
        var baseMinorUnits = absoluteMinorUnits / parts;
        var remainder = (int)(absoluteMinorUnits % parts);
        var allocations = new long[parts];

        for (var index = 0; index < parts; index++)
        {
            allocations[index] = baseMinorUnits;
        }

        foreach (var index in GetRemainderIndexes(parts, remainder, remainderStrategy))
        {
            allocations[index]++;
        }

        var allocationParts = new List<MoneyAllocationPart>(parts);

        for (var index = 0; index < parts; index++)
        {
            var signedMinorUnits = checked(allocations[index] * sign);
            allocationParts.Add(new MoneyAllocationPart(
                index + 1,
                Money.FromMinorUnits(signedMinorUnits, total.Currency)));
        }

        return new MoneyAllocation(total, allocationParts);
    }

    private static long GetAbsoluteMinorUnits(long value)
    {
        if (value == long.MinValue)
        {
            throw new OverflowException("Money amount is too large to allocate safely.");
        }

        return Math.Abs(value);
    }

    private static IEnumerable<int> GetRemainderIndexes(
        int parts,
        int remainder,
        AllocationRemainderStrategy strategy)
    {
        if (remainder == 0)
        {
            yield break;
        }

        switch (strategy)
        {
            case AllocationRemainderStrategy.First:
                for (var index = 0; index < remainder; index++)
                {
                    yield return index;
                }

                break;

            case AllocationRemainderStrategy.Last:
                for (var index = parts - remainder; index < parts; index++)
                {
                    yield return index;
                }

                break;

            case AllocationRemainderStrategy.Spread:
                for (var index = 0; index < remainder; index++)
                {
                    yield return (int)((long)index * parts / remainder);
                }

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(strategy), strategy, "Unknown allocation remainder strategy.");
        }
    }
}
