using System.Globalization;
using ISOCodex.Currency;
using ISOCodex.Currency.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Currency.AspNetCore.Tests;

public class CurrencyAspNetCoreTests
{
    [Fact]
    public async Task CurrencyCodeModelBinder_BindsRegisteredCode()
    {
        var bindingContext = CreateBindingContext(typeof(CurrencyCode), "currency", "gbp");
        var binder = new CurrencyCodeModelBinder();

        await binder.BindModelAsync(bindingContext);

        Assert.True(bindingContext.Result.IsModelSet);
        Assert.Equal(CurrencyCode.GBP, bindingContext.Result.Model);
        Assert.Empty(GetModelStateErrors(bindingContext.ModelState));
    }

    [Fact]
    public async Task CurrencyCodeModelBinder_RejectsUnknownCode()
    {
        var bindingContext = CreateBindingContext(typeof(CurrencyCode), "currency", "zzz");
        var binder = new CurrencyCodeModelBinder();

        await binder.BindModelAsync(bindingContext);

        Assert.False(bindingContext.Result.IsModelSet);
        var error = Assert.Single(bindingContext.ModelState["currency"]!.Errors);
        Assert.Contains("not a registered ISO 4217-style currency code", error.ErrorMessage);
    }

    [Fact]
    public async Task CurrencyCodeModelBinder_AllowsEmptyNullableCurrencyCode()
    {
        var bindingContext = CreateBindingContext(typeof(CurrencyCode?), "currency", string.Empty);
        var binder = new CurrencyCodeModelBinder();

        await binder.BindModelAsync(bindingContext);

        Assert.True(bindingContext.Result.IsModelSet);
        Assert.Null(bindingContext.Result.Model);
        Assert.Empty(GetModelStateErrors(bindingContext.ModelState));
    }

    [Fact]
    public void AddCurrencyCodeModelBinder_InsertsProviderOnce()
    {
        var options = new MvcOptions();

        options.AddCurrencyCodeModelBinder();
        options.AddCurrencyCodeModelBinder();

        Assert.IsType<CurrencyCodeModelBinderProvider>(options.ModelBinderProviders[0]);
        Assert.Single(options.ModelBinderProviders.OfType<CurrencyCodeModelBinderProvider>());
    }

    [Fact]
    public void AddCurrencyAspNetCore_RegistersServicesAndMvcBinder()
    {
        var services = new ServiceCollection();

        services.AddCurrencyAspNetCore();

        using var provider = services.BuildServiceProvider();
        Assert.Same(DefaultCurrencyRegistry.Instance, provider.GetRequiredService<ICurrencyRegistry>());
        Assert.IsType<MoneyFormatter>(provider.GetRequiredService<IMoneyFormatter>());
        Assert.IsType<MoneyParser>(provider.GetRequiredService<MoneyParser>());

        var options = provider.GetRequiredService<IOptions<MvcOptions>>().Value;
        Assert.IsType<CurrencyCodeModelBinderProvider>(options.ModelBinderProviders[0]);
    }

    [Fact]
    public void MoneyValidationResult_CreatesValidationProblemDetails()
    {
        var result = Money.TryCreate(12.345m, CurrencyCode.GBP);

        var details = result.ToValidationProblemDetails("price");

        Assert.Equal(400, details.Status);
        Assert.Equal("Currency validation failed.", details.Title);
        Assert.Equal("AmountPrecision", details.Extensions["reason"]);
        Assert.Contains("price", details.Errors.Keys);
    }

    [Fact]
    public void MoneyParseResult_CreatesValidationProblemDictionary()
    {
        var result = new MoneyParser().Parse("XYZ 12.34");

        var errors = result.ToValidationProblemDictionary("total");

        var message = Assert.Single(errors["total"]);
        Assert.Equal("Money input contains an unknown currency code.", message);
    }

    private static DefaultModelBindingContext CreateBindingContext(Type modelType, string modelName, string value)
    {
        var metadataProvider = new EmptyModelMetadataProvider();

        return new DefaultModelBindingContext
        {
            ModelMetadata = metadataProvider.GetMetadataForType(modelType),
            ModelName = modelName,
            ModelState = new ModelStateDictionary(),
            ValueProvider = new QueryStringValueProvider(
                BindingSource.Query,
                new QueryCollection(new Dictionary<string, StringValues>
                {
                    [modelName] = new StringValues(value)
                }),
                CultureInfo.InvariantCulture)
        };
    }

    private static IReadOnlyList<ModelError> GetModelStateErrors(ModelStateDictionary modelState)
    {
        return modelState.SelectMany(entry => entry.Value?.Errors ?? Enumerable.Empty<ModelError>()).ToArray();
    }
}
