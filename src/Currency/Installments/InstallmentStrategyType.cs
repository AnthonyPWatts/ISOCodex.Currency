namespace ISOCodex.Currency;

/// <summary>
/// Names the built-in installment strategies.
/// </summary>
public enum InstallmentStrategyType
{
    /// <summary>Split evenly and place remainder units at the start.</summary>
    EvenSplitRemainderFirst,

    /// <summary>Split evenly and place remainder units at the end.</summary>
    EvenSplitRemainderLast,

    /// <summary>Split evenly and spread remainder units across installments.</summary>
    EvenSplitRemainderSpread,

    /// <summary>Use a fixed first installment, then split the remainder.</summary>
    FixedFirstInstallment,

    /// <summary>Make all non-first installments whole major-unit amounts.</summary>
    WholeMajorUnitFirstInstallment
}
