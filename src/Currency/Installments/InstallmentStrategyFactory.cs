using System;

namespace ISOCodex.Currency;

/// <summary>
/// Creates built-in installment strategies.
/// </summary>
public static class InstallmentStrategyFactory
{
    /// <summary>
    /// Creates a built-in installment strategy.
    /// </summary>
    public static IInstallmentStrategy GetStrategy(
        InstallmentStrategyType strategyType,
        Money? fixedFirstInstallment = null)
    {
        switch (strategyType)
        {
            case InstallmentStrategyType.EvenSplitRemainderFirst:
                return new EvenSplitInstallmentStrategy(AllocationRemainderStrategy.First);

            case InstallmentStrategyType.EvenSplitRemainderLast:
                return new EvenSplitInstallmentStrategy(AllocationRemainderStrategy.Last);

            case InstallmentStrategyType.EvenSplitRemainderSpread:
                return new EvenSplitInstallmentStrategy(AllocationRemainderStrategy.Spread);

            case InstallmentStrategyType.FixedFirstInstallment:
                if (!fixedFirstInstallment.HasValue)
                {
                    throw new ArgumentNullException(nameof(fixedFirstInstallment), "A fixed first installment amount is required.");
                }

                return new FixedFirstInstallmentStrategy(fixedFirstInstallment.Value);

            case InstallmentStrategyType.WholeMajorUnitFirstInstallment:
                return new WholeMajorUnitFirstInstallmentStrategy();

            default:
                throw new ArgumentOutOfRangeException(nameof(strategyType), strategyType, "Unknown installment strategy type.");
        }
    }
}
