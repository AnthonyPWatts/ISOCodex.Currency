namespace ISOCodex.Currency;

/// <summary>
/// Calculates an installment plan.
/// </summary>
public interface IInstallmentStrategy
{
    /// <summary>
    /// Calculates installments for the supplied request.
    /// </summary>
    InstallmentPlan CalculateInstallments(InstallmentRequest request);
}
