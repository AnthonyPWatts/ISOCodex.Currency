using ISOCodex.Currency;
using ISOCodex.Currency.Validation;

namespace Currency.Validation.Tests;

public class CurrencyValidationTests
{
    [Fact]
    public void ValidateCurrencyCode_ReturnsValidForRegisteredCurrency()
    {
        var validator = new CurrencyBoundaryValidator();

        var result = validator.ValidateCurrencyCode("gbp");

        Assert.True(result.IsValid);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public void ValidateCurrencyCode_ReturnsStableIssueForUnknownCurrency()
    {
        var validator = new CurrencyBoundaryValidator();

        var result = validator.ValidateCurrencyCode("ZZZ", "currency");

        var issue = Assert.Single(result.Issues);
        Assert.False(result.IsValid);
        Assert.Equal(CurrencyValidationIssueCodes.CurrencyUnknown, issue.Code);
        Assert.Equal("currency", issue.PropertyName);
    }

    [Fact]
    public void ValidateMoney_MapsAmountPrecisionToAmountProperty()
    {
        var validator = new CurrencyBoundaryValidator();

        var result = validator.ValidateMoney(12.345m, "GBP", "amount", "currency");

        var issue = Assert.Single(result.Issues);
        Assert.Equal(CurrencyValidationIssueCodes.MoneyAmountPrecision, issue.Code);
        Assert.Equal("amount", issue.PropertyName);
    }

    [Fact]
    public void ValidateMoney_MapsUnknownCurrencyToCurrencyProperty()
    {
        var validator = new CurrencyBoundaryValidator();

        var result = validator.ValidateMoney(12.34m, "ZZZ", "amount", "currency");

        var issue = Assert.Single(result.Issues);
        Assert.Equal(CurrencyValidationIssueCodes.CurrencyUnknown, issue.Code);
        Assert.Equal("currency", issue.PropertyName);
    }

    [Fact]
    public void ValidateMinorUnits_MapsMinorUnitFailureToMinorUnitsProperty()
    {
        var validator = new CurrencyBoundaryValidator();

        var result = validator.ValidateMinorUnits(123, "XXX", "minorUnits", "currency");

        var issue = Assert.Single(result.Issues);
        Assert.Equal(CurrencyValidationIssueCodes.MoneyMinorUnitNotApplicable, issue.Code);
        Assert.Equal("minorUnits", issue.PropertyName);
    }

    [Fact]
    public void MoneyParseResultAdapter_ReturnsStableParseIssue()
    {
        var parseResult = new MoneyParser().Parse("XYZ 12.34");

        var result = parseResult.ToCurrencyValidationResult("total");

        var issue = Assert.Single(result.Issues);
        Assert.Equal(CurrencyValidationIssueCodes.MoneyParseUnknownCurrency, issue.Code);
        Assert.Equal("total", issue.PropertyName);
    }

    [Fact]
    public void ToErrorDictionary_GroupsMessagesByProperty()
    {
        var result = CurrencyValidationResult.FromIssues(new[]
        {
            new CurrencyValidationIssue("A", "First amount issue.", "amount"),
            new CurrencyValidationIssue("B", "Second amount issue.", "amount"),
            new CurrencyValidationIssue("C", "Currency issue.", "currency")
        });

        var errors = result.ToErrorDictionary();

        Assert.Equal(new[] { "First amount issue.", "Second amount issue." }, errors["amount"]);
        Assert.Equal(new[] { "Currency issue." }, errors["currency"]);
    }

    [Fact]
    public void MoneyValidationResultAdapter_ReturnsValidForSuccessfulResult()
    {
        var moneyResult = Money.TryCreate(12.34m, CurrencyCode.GBP);

        var result = moneyResult.ToCurrencyValidationResult("amount");

        Assert.True(result.IsValid);
        Assert.Empty(result.Issues);
    }
}
