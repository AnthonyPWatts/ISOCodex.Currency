using ISOCodex.Currency;

namespace Currency.Tests;

public class MoneyFactoryTests
{
    private static readonly CurrencyCode CustomCode = CurrencyCode.CreateCustom("zza");

    [Fact]
    public void Of_CreatesMoneyUsingCustomRegistry()
    {
        var factory = new MoneyFactory(CreateCustomRegistry());

        var money = factory.Of(12.3456m, CustomCode);

        Assert.Equal(12.3456m, money.Amount);
        Assert.Equal(CustomCode, money.Currency);
    }

    [Fact]
    public void TryCreate_UsesCustomRegistryValidation()
    {
        var factory = new MoneyFactory(CreateCustomRegistry());

        var result = factory.TryCreate(12.34567m, CustomCode);

        Assert.False(result.Succeeded);
        Assert.Equal(MoneyValidationFailureReason.AmountPrecision, result.FailureReason);
        Assert.Contains("ZZA", result.ErrorMessage);
        Assert.Contains("4", result.ErrorMessage);
    }

    [Fact]
    public void FromMinorUnits_UsesCustomRegistryIncrement()
    {
        var factory = new MoneyFactory(CreateCustomRegistry());

        var money = factory.FromMinorUnits(123456, CustomCode);

        Assert.Equal(12.3456m, money.Amount);
        Assert.Equal(CustomCode, money.Currency);
    }

    [Fact]
    public void TryFromMinorUnits_UsesCustomRegistryIncrement()
    {
        var factory = new MoneyFactory(CreateCustomRegistry());

        var result = factory.TryFromMinorUnits(123456, CustomCode);

        Assert.True(result.Succeeded);
        Assert.Equal(CustomCode, result.Money?.Currency);
        Assert.Equal(12.3456m, result.Money?.Amount);
    }

    [Fact]
    public void StaticMoneyApi_ContinuesToUseDefaultRegistry()
    {
        var result = Money.TryCreate(12.3456m, CustomCode);

        Assert.False(result.Succeeded);
        Assert.Equal(MoneyValidationFailureReason.UnknownCurrency, result.FailureReason);
    }

    [Fact]
    public void CustomRegistry_DoesNotMutateDefaultRegistry()
    {
        _ = new MoneyFactory(CreateCustomRegistry()).Of(12.3456m, CustomCode);

        Assert.False(DefaultCurrencyRegistry.Instance.TryGet(CustomCode, out _));
    }

    private static DefaultCurrencyRegistry CreateCustomRegistry()
    {
        return new DefaultCurrencyRegistry(new[]
        {
            new CurrencyInfo(
                CustomCode,
                "999",
                "Test account unit",
                new CurrencyMinorUnit(4),
                CurrencyKind.Testing,
                false)
        });
    }
}
