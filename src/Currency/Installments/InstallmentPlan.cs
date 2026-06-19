using System;
using System.Collections.Generic;
using System.Linq;

namespace ISOCodex.Currency;

/// <summary>
/// Represents a set of installments whose amounts preserve the original total.
/// </summary>
public sealed class InstallmentPlan
{
    /// <summary>
    /// Creates an installment plan and verifies that its installments preserve the total.
    /// </summary>
    public InstallmentPlan(Money total, IEnumerable<Installment> installments)
    {
        if (installments == null)
        {
            throw new ArgumentNullException(nameof(installments));
        }

        var materializedInstallments = installments.ToArray();

        if (materializedInstallments.Length == 0)
        {
            throw new ArgumentException("Installment plan must contain at least one installment.", nameof(installments));
        }

        var installmentTotal = Money.Zero(total.Currency);

        foreach (var installment in materializedInstallments)
        {
            installmentTotal += installment.Amount;
        }

        if (installmentTotal != total)
        {
            throw new InvalidOperationException($"Installment amounts must sum to {total}. Actual total was {installmentTotal}.");
        }

        Total = total;
        Installments = materializedInstallments;
    }

    /// <summary>Gets the original total.</summary>
    public Money Total { get; }

    /// <summary>Gets the installments.</summary>
    public IReadOnlyList<Installment> Installments { get; }
}
