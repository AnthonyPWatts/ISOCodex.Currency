using System;

namespace ISOCodex.Currency;

/// <summary>
/// Describes an installment calculation request.
/// </summary>
public sealed class InstallmentRequest
{
    /// <summary>
    /// Creates an installment request.
    /// </summary>
    public InstallmentRequest(Money total, int numberOfInstallments)
    {
        if (numberOfInstallments <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfInstallments), "Number of installments must be greater than zero.");
        }

        if (total.Amount <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(total), total, "Installment total must be greater than zero.");
        }

        Total = total;
        NumberOfInstallments = numberOfInstallments;
    }

    /// <summary>Gets the total to split into installments.</summary>
    public Money Total { get; }

    /// <summary>Gets the requested number of installments.</summary>
    public int NumberOfInstallments { get; }
}
