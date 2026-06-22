using ISOCodex.Currency;

namespace Currency.Tests;

public class InstallmentStrategyTests
{
    [Fact]
    public void EvenSplit_CreatesInstallmentsThatPreserveTotal()
    {
        var strategy = new EvenSplitInstallmentStrategy(AllocationRemainderStrategy.Last);

        var plan = strategy.CalculateInstallments(new InstallmentRequest(Money.Of(10.00m, CurrencyCode.GBP), 3));

        Assert.Equal(Money.Of(10.00m, CurrencyCode.GBP), plan.Total);
        Assert.Equal(new[] { 3.33m, 3.33m, 3.34m }, plan.Installments.Select(installment => installment.Amount.Amount));
        Assert.Equal(plan.Total, Sum(plan.Installments.Select(installment => installment.Amount), CurrencyCode.GBP));
    }

    [Fact]
    public void EvenSplit_WorksForZeroMinorUnitCurrency()
    {
        var strategy = new EvenSplitInstallmentStrategy(AllocationRemainderStrategy.First);

        var plan = strategy.CalculateInstallments(new InstallmentRequest(Money.Of(100m, CurrencyCode.JPY), 3));

        Assert.Equal(new[] { 34m, 33m, 33m }, plan.Installments.Select(installment => installment.Amount.Amount));
        Assert.Equal(plan.Total, Sum(plan.Installments.Select(installment => installment.Amount), CurrencyCode.JPY));
    }

    [Fact]
    public void EvenSplit_WorksForThreeDecimalPlaceCurrency()
    {
        var strategy = new EvenSplitInstallmentStrategy(AllocationRemainderStrategy.First);

        var plan = strategy.CalculateInstallments(new InstallmentRequest(Money.Of(1.000m, CurrencyCode.KWD), 3));

        Assert.Equal(new[] { 0.334m, 0.333m, 0.333m }, plan.Installments.Select(installment => installment.Amount.Amount));
        Assert.Equal(plan.Total, Sum(plan.Installments.Select(installment => installment.Amount), CurrencyCode.KWD));
    }

    [Fact]
    public void FixedFirstInstallment_UsesMoneyAndPlacesRemainingRemainderIntoSecondInstallmentByDefault()
    {
        var strategy = new FixedFirstInstallmentStrategy(Money.Of(4.00m, CurrencyCode.GBP));

        var plan = strategy.CalculateInstallments(new InstallmentRequest(Money.Of(10.01m, CurrencyCode.GBP), 3));

        Assert.Equal(new[] { 4.00m, 3.01m, 3.00m }, plan.Installments.Select(installment => installment.Amount.Amount));
        Assert.Equal(plan.Total, Sum(plan.Installments.Select(installment => installment.Amount), CurrencyCode.GBP));
    }

    [Fact]
    public void FixedFirstInstallment_HonoursConfiguredRemainderStrategyForRemainingInstallments()
    {
        var strategy = new FixedFirstInstallmentStrategy(
            Money.Of(4.00m, CurrencyCode.GBP),
            AllocationRemainderStrategy.Last);

        var plan = strategy.CalculateInstallments(new InstallmentRequest(Money.Of(10.04m, CurrencyCode.GBP), 4));

        Assert.Equal(new[] { 4.00m, 2.01m, 2.01m, 2.02m }, plan.Installments.Select(installment => installment.Amount.Amount));
        Assert.Equal(plan.Total, Sum(plan.Installments.Select(installment => installment.Amount), CurrencyCode.GBP));
    }

    [Fact]
    public void FixedFirstInstallment_RejectsCurrencyMismatch()
    {
        var strategy = new FixedFirstInstallmentStrategy(Money.Of(4.00m, CurrencyCode.GBP));

        Assert.Throws<InvalidOperationException>(
            () => strategy.CalculateInstallments(new InstallmentRequest(Money.Of(10.00m, CurrencyCode.USD), 3)));
    }

    [Fact]
    public void FixedFirstInstallment_RejectsSingleInstallmentRequest()
    {
        var strategy = new FixedFirstInstallmentStrategy(Money.Of(4.00m, CurrencyCode.GBP));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => strategy.CalculateInstallments(new InstallmentRequest(Money.Of(10.00m, CurrencyCode.GBP), 1)));
    }

    [Fact]
    public void FixedFirstInstallment_RejectsFirstInstallmentThatConsumesTotal()
    {
        var strategy = new FixedFirstInstallmentStrategy(Money.Of(10.00m, CurrencyCode.GBP));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => strategy.CalculateInstallments(new InstallmentRequest(Money.Of(10.00m, CurrencyCode.GBP), 3)));
    }

    [Fact]
    public void WholeMajorUnitFirstInstallment_PutsFractionalRemainderIntoFirstInstallment()
    {
        var strategy = new WholeMajorUnitFirstInstallmentStrategy();

        var plan = strategy.CalculateInstallments(new InstallmentRequest(Money.Of(10.99m, CurrencyCode.GBP), 3));

        Assert.Equal(new[] { 4.99m, 3.00m, 3.00m }, plan.Installments.Select(installment => installment.Amount.Amount));
        Assert.Equal(plan.Total, Sum(plan.Installments.Select(installment => installment.Amount), CurrencyCode.GBP));
    }

    [Fact]
    public void WholeMajorUnitFirstInstallment_FailsClearlyForZeroMinorUnitCurrency()
    {
        var strategy = new WholeMajorUnitFirstInstallmentStrategy();

        var exception = Assert.Throws<InvalidOperationException>(
            () => strategy.CalculateInstallments(new InstallmentRequest(Money.Of(100m, CurrencyCode.JPY), 3)));

        Assert.Contains("no fractional minor units", exception.Message);
    }

    [Fact]
    public void Installment_ToString_UsesMoneyCodeAndDoesNotHardCodeCurrencySymbol()
    {
        var installment = new Installment(1, Money.Of(4.99m, CurrencyCode.GBP));

        var text = installment.ToString();

        Assert.Equal("1: GBP 4.99", text);
        Assert.DoesNotContain("£", text);
    }

    [Fact]
    public void Factory_CreatesBuiltInStrategies()
    {
        Assert.IsType<EvenSplitInstallmentStrategy>(InstallmentStrategyFactory.GetStrategy(InstallmentStrategyType.EvenSplitRemainderFirst));
        Assert.IsType<EvenSplitInstallmentStrategy>(InstallmentStrategyFactory.GetStrategy(InstallmentStrategyType.EvenSplitRemainderLast));
        Assert.IsType<EvenSplitInstallmentStrategy>(InstallmentStrategyFactory.GetStrategy(InstallmentStrategyType.EvenSplitRemainderSpread));
        Assert.IsType<FixedFirstInstallmentStrategy>(InstallmentStrategyFactory.GetStrategy(InstallmentStrategyType.FixedFirstInstallment, Money.Of(4m, CurrencyCode.GBP)));
        Assert.IsType<WholeMajorUnitFirstInstallmentStrategy>(InstallmentStrategyFactory.GetStrategy(InstallmentStrategyType.WholeMajorUnitFirstInstallment));
    }

    [Fact]
    public void Factory_RequiresFixedFirstInstallmentAmount()
    {
        Assert.Throws<ArgumentNullException>(() => InstallmentStrategyFactory.GetStrategy(InstallmentStrategyType.FixedFirstInstallment));
    }

    [Fact]
    public void InstallmentRequest_RejectsNonPositiveTotals()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new InstallmentRequest(Money.Zero(CurrencyCode.GBP), 3));
    }

    [Fact]
    public void InstallmentPlan_RejectsInstallmentsThatDoNotSumToTotal()
    {
        var installments = new[]
        {
            new Installment(1, Money.Of(4m, CurrencyCode.GBP)),
            new Installment(2, Money.Of(5m, CurrencyCode.GBP))
        };

        Assert.Throws<InvalidOperationException>(() => new InstallmentPlan(Money.Of(10m, CurrencyCode.GBP), installments));
    }

    private static Money Sum(IEnumerable<Money> values, CurrencyCode currency)
    {
        var total = Money.Zero(currency);

        foreach (var value in values)
        {
            total += value;
        }

        return total;
    }
}
