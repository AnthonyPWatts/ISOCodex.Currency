using System;
using System.Linq;

namespace ISOCodex.Currency;

/// <summary>
/// Splits a total evenly and places indivisible minor-unit remainders according to the configured strategy.
/// </summary>
public sealed class EvenSplitInstallmentStrategy : IInstallmentStrategy
{
    private readonly AllocationRemainderStrategy _remainderStrategy;
    private readonly MoneyAllocator _allocator;

    /// <summary>
    /// Creates an even-split installment strategy.
    /// </summary>
    public EvenSplitInstallmentStrategy(
        AllocationRemainderStrategy remainderStrategy = AllocationRemainderStrategy.First,
        MoneyAllocator? allocator = null)
    {
        _remainderStrategy = remainderStrategy;
        _allocator = allocator ?? new MoneyAllocator();
    }

    /// <inheritdoc />
    public InstallmentPlan CalculateInstallments(InstallmentRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var allocation = _allocator.Allocate(request.Total, request.NumberOfInstallments, _remainderStrategy);
        var installments = allocation.Parts
            .Select(part => new Installment(part.PartNumber, part.Amount))
            .ToArray();

        return new InstallmentPlan(request.Total, installments);
    }
}
