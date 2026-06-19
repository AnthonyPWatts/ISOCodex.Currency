using ISOCodex.Currency;

namespace Currency.Tests;

public class MoneyAllocatorTests
{
    [Fact]
    public void Allocate_PutsRemainderAtStart()
    {
        var allocation = Money.Of(10.00m, CurrencyCode.GBP)
            .Allocate(3, AllocationRemainderStrategy.First);

        Assert.Equal(Money.Of(10.00m, CurrencyCode.GBP), allocation.Total);
        Assert.Equal(
            new[] { 3.34m, 3.33m, 3.33m },
            allocation.Parts.Select(part => part.Amount.Amount));
        Assert.Equal(allocation.Total, Sum(allocation.Parts.Select(part => part.Amount), CurrencyCode.GBP));
    }

    [Fact]
    public void Allocate_PutsRemainderAtEnd()
    {
        var allocation = Money.Of(10.00m, CurrencyCode.GBP)
            .Allocate(3, AllocationRemainderStrategy.Last);

        Assert.Equal(
            new[] { 3.33m, 3.33m, 3.34m },
            allocation.Parts.Select(part => part.Amount.Amount));
        Assert.Equal(allocation.Total, Sum(allocation.Parts.Select(part => part.Amount), CurrencyCode.GBP));
    }

    [Fact]
    public void Allocate_SpreadsRemainderAcrossParts()
    {
        var allocation = Money.Of(10.05m, CurrencyCode.GBP)
            .Allocate(6, AllocationRemainderStrategy.Spread);

        Assert.Equal(
            new[] { 1.68m, 1.67m, 1.68m, 1.67m, 1.68m, 1.67m },
            allocation.Parts.Select(part => part.Amount.Amount));
        Assert.Equal(allocation.Total, Sum(allocation.Parts.Select(part => part.Amount), CurrencyCode.GBP));
    }

    [Fact]
    public void Allocate_PreservesZeroMinorUnitCurrencyTotals()
    {
        var allocation = Money.Of(100m, CurrencyCode.JPY)
            .Allocate(3, AllocationRemainderStrategy.First);

        Assert.Equal(
            new[] { 34m, 33m, 33m },
            allocation.Parts.Select(part => part.Amount.Amount));
        Assert.Equal(allocation.Total, Sum(allocation.Parts.Select(part => part.Amount), CurrencyCode.JPY));
    }

    [Fact]
    public void Allocate_PreservesThreeDecimalPlaceCurrencyTotals()
    {
        var allocation = Money.Of(1.000m, CurrencyCode.KWD)
            .Allocate(3, AllocationRemainderStrategy.First);

        Assert.Equal(
            new[] { 0.334m, 0.333m, 0.333m },
            allocation.Parts.Select(part => part.Amount.Amount));
        Assert.Equal(allocation.Total, Sum(allocation.Parts.Select(part => part.Amount), CurrencyCode.KWD));
    }

    [Fact]
    public void Allocate_PreservesNegativeTotals()
    {
        var allocation = Money.Of(-10.00m, CurrencyCode.GBP)
            .Allocate(3, AllocationRemainderStrategy.First);

        Assert.Equal(
            new[] { -3.34m, -3.33m, -3.33m },
            allocation.Parts.Select(part => part.Amount.Amount));
        Assert.Equal(allocation.Total, Sum(allocation.Parts.Select(part => part.Amount), CurrencyCode.GBP));
    }

    [Fact]
    public void Allocate_RejectsNonPositivePartCount()
    {
        var allocator = new MoneyAllocator();

        Assert.Throws<ArgumentOutOfRangeException>(() => allocator.Allocate(Money.Of(10m, CurrencyCode.GBP), 0));
    }

    [Fact]
    public void Allocate_RejectsCurrencyWithoutApplicableMinorUnits()
    {
        var allocator = new MoneyAllocator();

        Assert.Throws<InvalidOperationException>(() => allocator.Allocate(Money.Of(10m, CurrencyCode.XXX), 3));
    }

    [Fact]
    public void MoneyAllocation_RejectsPartsThatDoNotSumToTotal()
    {
        var parts = new[]
        {
            new MoneyAllocationPart(1, Money.Of(4m, CurrencyCode.GBP)),
            new MoneyAllocationPart(2, Money.Of(5m, CurrencyCode.GBP))
        };

        Assert.Throws<InvalidOperationException>(() => new MoneyAllocation(Money.Of(10m, CurrencyCode.GBP), parts));
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
