using System;
using System.Collections.Generic;
using System.Linq;

namespace ISOCodex.Currency;

/// <summary>
/// Uses a fixed first installment and splits the remaining amount across later installments.
/// </summary>
public sealed class FixedFirstInstallmentStrategy : IInstallmentStrategy
{
    private readonly Money _firstInstallmentAmount;
    private readonly AllocationRemainderStrategy _remainingRemainderStrategy;
    private readonly MoneyAllocator _allocator;

    /// <summary>
    /// Creates a fixed-first installment strategy.
    /// </summary>
    public FixedFirstInstallmentStrategy(
        Money firstInstallmentAmount,
        AllocationRemainderStrategy remainingRemainderStrategy = AllocationRemainderStrategy.First,
        MoneyAllocator? allocator = null)
    {
        if (firstInstallmentAmount.Amount <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(firstInstallmentAmount), firstInstallmentAmount, "First installment amount must be greater than zero.");
        }

        _firstInstallmentAmount = firstInstallmentAmount;
        _remainingRemainderStrategy = remainingRemainderStrategy;
        _allocator = allocator ?? new MoneyAllocator();
    }

    /// <inheritdoc />
    public InstallmentPlan CalculateInstallments(InstallmentRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        request.Total.RequireSameCurrency(_firstInstallmentAmount);

        if (request.NumberOfInstallments < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(request), request.NumberOfInstallments, "At least two installments are required when the first installment is fixed.");
        }

        if (_firstInstallmentAmount.CompareTo(request.Total) >= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(_firstInstallmentAmount), _firstInstallmentAmount, "First installment must be less than the total.");
        }

        var remainingTotal = request.Total - _firstInstallmentAmount;
        var remainingAllocation = _allocator.Allocate(
            remainingTotal,
            request.NumberOfInstallments - 1,
            _remainingRemainderStrategy);
        var installments = new List<Installment>
        {
            new Installment(1, _firstInstallmentAmount)
        };

        installments.AddRange(remainingAllocation.Parts
            .Select(part => new Installment(part.PartNumber + 1, part.Amount)));

        return new InstallmentPlan(request.Total, installments);
    }
}
