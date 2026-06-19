namespace ISOCodex.Currency;

/// <summary>
/// Determines where indivisible minor-unit remainders are placed during allocation.
/// </summary>
public enum AllocationRemainderStrategy
{
    /// <summary>Place remainder units at the start of the allocation.</summary>
    First,

    /// <summary>Place remainder units at the end of the allocation.</summary>
    Last,

    /// <summary>Spread remainder units across the allocation as evenly as possible.</summary>
    Spread
}
