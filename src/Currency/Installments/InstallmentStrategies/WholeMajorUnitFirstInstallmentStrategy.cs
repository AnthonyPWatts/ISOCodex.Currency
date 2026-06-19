using System;
using System.Collections.Generic;

namespace ISOCodex.Currency;

/// <summary>
/// Makes every non-first installment a whole major-unit amount and places the fractional remainder in the first installment.
/// </summary>
public sealed class WholeMajorUnitFirstInstallmentStrategy : IInstallmentStrategy
{
    /// <inheritdoc />
    public InstallmentPlan CalculateInstallments(InstallmentRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var currency = DefaultCurrencyRegistry.Instance.Get(request.Total.Currency);

        if (!currency.MinorUnit.IsApplicable)
        {
            throw new InvalidOperationException($"Currency '{request.Total.Currency}' does not define applicable minor units.");
        }

        if (currency.MinorUnit.DecimalPlaces == 0)
        {
            throw new InvalidOperationException($"Currency '{request.Total.Currency}' has no fractional minor units, so whole-major-unit installment handling is not applicable.");
        }

        var totalWholeMajorUnits = decimal.Truncate(request.Total.Amount);
        var regularMajorUnits = (long)(totalWholeMajorUnits / request.NumberOfInstallments);
        var regularInstallment = Money.Of(regularMajorUnits, request.Total.Currency);
        var nonFirstTotal = Money.Zero(request.Total.Currency);

        for (var index = 1; index < request.NumberOfInstallments; index++)
        {
            nonFirstTotal += regularInstallment;
        }

        var firstInstallment = request.Total - nonFirstTotal;
        var installments = new List<Installment>
        {
            new Installment(1, firstInstallment)
        };

        for (var installmentNumber = 2; installmentNumber <= request.NumberOfInstallments; installmentNumber++)
        {
            installments.Add(new Installment(installmentNumber, regularInstallment));
        }

        return new InstallmentPlan(request.Total, installments);
    }
}
