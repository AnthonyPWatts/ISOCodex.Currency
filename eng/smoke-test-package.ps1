param(
    [string]$Configuration = "Release",
    [string]$Version = "0.9.0-alpha.14",
    [switch]$UseMajorRollForward
)

$ErrorActionPreference = "Stop"

if ($UseMajorRollForward) {
    $env:DOTNET_ROLL_FORWARD = "Major"
}

function Invoke-DotNet {
    $dotnetArguments = $args

    & dotnet @dotnetArguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet $($dotnetArguments -join ' ') failed with exit code $LASTEXITCODE."
    }
}

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$artifacts = Join-Path $repoRoot "artifacts\smoke"
$tempRoot = Join-Path $repoRoot "artifacts\smoke-consumer"
$packages = Join-Path $repoRoot "artifacts\smoke-packages"
$consumerProject = Join-Path $tempRoot "CurrencySmoke.csproj"
$nugetConfig = Join-Path $tempRoot "NuGet.config"

Remove-Item $artifacts -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item $tempRoot -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item $packages -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force -Path $artifacts | Out-Null
New-Item -ItemType Directory -Force -Path $tempRoot | Out-Null

Invoke-DotNet restore "$repoRoot\ISOCodex.Currency.sln"
Invoke-DotNet build "$repoRoot\ISOCodex.Currency.sln" -c $Configuration --no-restore /p:Version=$Version
Invoke-DotNet test "$repoRoot\ISOCodex.Currency.sln" -c $Configuration --no-build
& "$PSScriptRoot\pack-packages.ps1" -Configuration $Configuration -OutputPath $artifacts -Version $Version
if ($LASTEXITCODE -ne 0) {
    throw "Package creation failed with exit code $LASTEXITCODE."
}

Invoke-DotNet new console -n CurrencySmoke -o $tempRoot

$program = @'
using System;
using System.Text.Json;
using ISOCodex.Addressing.Validation;
using ISOCodex.Currency;
using ISOCodex.Currency.AspNetCore;
using ISOCodex.Currency.Countries;
using ISOCodex.Currency.Dapper;
using ISOCodex.Currency.EntityFrameworkCore;
using ISOCodex.Currency.Exchange.Abstractions;
using ISOCodex.Currency.Json.SystemTextJson;
using ISOCodex.Currency.Validation;
using CountryAlpha2Code = ISOCodex.Countries.CountryAlpha2Code;
using AddressCountryCode = ISOCodex.Addressing.CountryCode;
using AddressPostalCode = ISOCodex.Addressing.PostalCode;
using CurrencyAddressing = ISOCodex.Currency.Addressing;
using NewtonsoftCurrencyCodeJsonConverter = ISOCodex.Currency.Json.NewtonsoftJson.CurrencyCodeJsonConverter;
using NewtonsoftJson = Newtonsoft.Json.JsonConvert;
using NewtonsoftMoneyJsonConverter = ISOCodex.Currency.Json.NewtonsoftJson.MoneyJsonConverter;
using PostalAddress = ISOCodex.Addressing.Address;

var gbp = CurrencyCode.Parse("gbp");
var metadata = DefaultCurrencyRegistry.Instance.Get(gbp);
var amount = Money.Of(12.34m, gbp);
var shipping = Money.Of(3.21m, CurrencyCode.GBP);
var total = amount + shipping;
var tax = amount.Multiply(0.2m, CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven));
var minorUnits = amount.ToMinorUnits();
var roundTrip = Money.FromMinorUnits(minorUnits, gbp);
var strategy = new EvenSplitInstallmentStrategy(AllocationRemainderStrategy.Last);
var installmentPlan = strategy.CalculateInstallments(new InstallmentRequest(total, 3));
var formatted = new MoneyFormatter().Format(amount);
var parsed = new MoneyParser().Parse("GBP 12.34", MoneyParseOptions.Default);
var validated = Money.TryCreate(12.34m, "GBP");
var invalidPrecision = Money.TryCreate(12.345m, CurrencyCode.GBP);
var invalidMinorUnits = Money.TryFromMinorUnits(123, CurrencyCode.XXX);
var dataVersion = CurrencyDataVersion.Identifier;
var efCurrency = (CurrencyCode)CurrencyCodeValueConverter.Instance.ConvertFromProvider("gbp")!;
var dapperCurrency = CurrencyCodeTypeHandler.Instance.Parse("gbp");
DapperCurrencyTypeHandlers.Register();
var validationProblem = invalidPrecision.ToValidationProblemDetails("amount");
var boundaryValidation = new CurrencyBoundaryValidator().ValidateMoney(12.345m, "GBP", "amount", "currency");
var customCode = CurrencyCode.CreateCustom("zza");
var customRegistry = new DefaultCurrencyRegistry(new[]
{
    new CurrencyInfo(
        customCode,
        "999",
        "Smoke test account unit",
        new CurrencyMinorUnit(4),
        CurrencyKind.Testing,
        false)
});
var customMoneyFactory = new MoneyFactory(customRegistry);
var customMoney = customMoneyFactory.Of(1.2345m, customCode);
var customValidation = customMoneyFactory.TryCreate(1.23456m, customCode);
var jsonOptions = new JsonSerializerOptions();
jsonOptions.Converters.Add(new CurrencyCodeJsonConverter());
jsonOptions.Converters.Add(new MoneyJsonConverter());
var moneyJson = JsonSerializer.Serialize(amount, jsonOptions);
var parsedJsonMoney = JsonSerializer.Deserialize<Money>(moneyJson, jsonOptions);
var newtonsoftSettings = new Newtonsoft.Json.JsonSerializerSettings();
newtonsoftSettings.Converters.Add(new NewtonsoftCurrencyCodeJsonConverter());
newtonsoftSettings.Converters.Add(new NewtonsoftMoneyJsonConverter());
var newtonsoftMoneyJson = NewtonsoftJson.SerializeObject(amount, newtonsoftSettings);
var parsedNewtonsoftMoney = NewtonsoftJson.DeserializeObject<Money>(newtonsoftMoneyJson, newtonsoftSettings);
var countryCurrency = DefaultCountryCurrencyRegistry.Instance.Validate(
    CountryAlpha2Code.Parse("GB"),
    CurrencyCode.GBP,
    CountryCurrencyValidationPolicy.PrimaryTenderOnly);
var billingAddress = new PostalAddress(
    "1 High Street",
    null,
    "London",
    null,
    new AddressPostalCode("SW1A 1AA"),
    AddressCountryCode.GB);
var addressCurrencyValidator = new CurrencyAddressing.AddressCurrencyValidator(
    new SmokeAddressValidatorFactory(AddressValidationResult.Success));
var addressCurrencyValidation = addressCurrencyValidator.Validate(
    billingAddress,
    CurrencyCode.GBP,
    CurrencyAddressing.AddressCurrencyValidationPolicy.CheckoutDefault);
var exchangeEffectiveDate = new DateTime(2026, 6, 22, 0, 0, 0, DateTimeKind.Utc);
var exchangeRate = new ExchangeRate(
    new CurrencyPair(CurrencyCode.GBP, CurrencyCode.USD),
    1.2345m,
    ExchangeRateKind.MidMarket,
    exchangeEffectiveDate,
    "smoke-test-feed");
var exchangeConverter = new MoneyConverter(new StaticExchangeRateProvider(exchangeRate));
var converted = exchangeConverter.Convert(
    Money.Of(10.00m, CurrencyCode.GBP),
    new ConversionOptions(
        CurrencyCode.USD,
        exchangeEffectiveDate,
        ExchangeRateKind.MidMarket,
        CurrencyRoundingPolicy.AwayFromZero()));
#pragma warning disable ISOCCUR001, ISOCCUR002
var defaultCurrencyDetected = default(CurrencyCode).IsDefault;
var defaultMoneyDetected = default(Money).IsDefault;
#pragma warning restore ISOCCUR001, ISOCCUR002

if (metadata.Code != gbp)
{
    throw new InvalidOperationException("Registry lookup returned the wrong currency.");
}

if (roundTrip != amount)
{
    throw new InvalidOperationException("Minor-unit round trip failed.");
}

if (total != Money.Of(15.55m, gbp))
{
    throw new InvalidOperationException("Same-currency addition failed.");
}

if (installmentPlan.Installments.Count != 3 || installmentPlan.Total != total)
{
    throw new InvalidOperationException("Installment strategy smoke test failed.");
}

if (formatted != "GBP 12.34")
{
    throw new InvalidOperationException("Money formatting smoke test failed.");
}

if (!parsed.Succeeded || parsed.Money.GetValueOrDefault() != amount)
{
    throw new InvalidOperationException("Money parsing smoke test failed.");
}

if (!validated.Succeeded || validated.Money.GetValueOrDefault() != amount)
{
    throw new InvalidOperationException("Money validation smoke test failed.");
}

if (invalidPrecision.Succeeded || invalidPrecision.FailureReason != MoneyValidationFailureReason.AmountPrecision)
{
    throw new InvalidOperationException("Money validation precision failure smoke test failed.");
}

if (invalidMinorUnits.Succeeded || invalidMinorUnits.FailureReason != MoneyValidationFailureReason.MinorUnitNotApplicable)
{
    throw new InvalidOperationException("Money minor-unit validation smoke test failed.");
}

if (!defaultCurrencyDetected || !defaultMoneyDetected || amount.IsDefault)
{
    throw new InvalidOperationException("Default-value detection smoke test failed.");
}

if (dataVersion != "iso4217-cldr-2026-06-22-c1f3aaea" || CurrencyDataVersion.SourceKind != "SIX-ISO4217+Unicode-CLDR")
{
    throw new InvalidOperationException("Currency data version smoke test failed.");
}

if (efCurrency != gbp)
{
    throw new InvalidOperationException("Entity Framework Core converter smoke test failed.");
}

if (dapperCurrency != gbp)
{
    throw new InvalidOperationException("Dapper type handler smoke test failed.");
}

if ((string?)validationProblem.Extensions["reason"] != nameof(MoneyValidationFailureReason.AmountPrecision)
    || !validationProblem.Errors.ContainsKey("amount"))
{
    throw new InvalidOperationException("ASP.NET Core problem-details smoke test failed.");
}

if (boundaryValidation.IsValid
    || boundaryValidation.Issues.Count != 1
    || boundaryValidation.Issues[0].Code != CurrencyValidationIssueCodes.MoneyAmountPrecision)
{
    throw new InvalidOperationException("Validation package smoke test failed.");
}

if (customMoney.Currency != customCode || customMoney.Amount != 1.2345m || customValidation.Succeeded || customValidation.FailureReason != MoneyValidationFailureReason.AmountPrecision)
{
    throw new InvalidOperationException("MoneyFactory custom registry smoke test failed.");
}

if (moneyJson != "{\"amount\":12.34,\"currency\":\"GBP\"}" || parsedJsonMoney != amount)
{
    throw new InvalidOperationException("System.Text.Json converter smoke test failed.");
}

if (newtonsoftMoneyJson != "{\"amount\":12.34,\"currency\":\"GBP\"}" || parsedNewtonsoftMoney != amount)
{
    throw new InvalidOperationException("Newtonsoft.Json converter smoke test failed.");
}

if (!countryCurrency.Succeeded || countryCurrency.CountryCurrency?.CurrencyCode != CurrencyCode.GBP)
{
    throw new InvalidOperationException("Country/currency bridge smoke test failed.");
}

if (!addressCurrencyValidation.Succeeded
    || addressCurrencyValidation.CountryCurrencyValidationResult?.CountryCurrency?.CurrencyCode != CurrencyCode.GBP)
{
    throw new InvalidOperationException("Address/currency bridge smoke test failed.");
}

if (converted.RawAmount != 12.345000m || converted.Output != Money.Of(12.35m, CurrencyCode.USD) || converted.RateSource != "smoke-test-feed")
{
    throw new InvalidOperationException("Exchange abstractions smoke test failed.");
}

Console.WriteLine(gbp);
Console.WriteLine(metadata.EnglishName);
Console.WriteLine(amount);
Console.WriteLine(total);
Console.WriteLine(tax);
Console.WriteLine(minorUnits);
Console.WriteLine(installmentPlan.Installments.Count);
Console.WriteLine(formatted);
Console.WriteLine(parsed.Money);
Console.WriteLine(validated.Succeeded);
Console.WriteLine(dataVersion);
Console.WriteLine(efCurrency);
Console.WriteLine(dapperCurrency);
Console.WriteLine(validationProblem.Extensions["reason"]);
Console.WriteLine(boundaryValidation.Issues[0].Code);
Console.WriteLine(customMoney);
Console.WriteLine(customValidation.FailureReason);
Console.WriteLine(moneyJson);
Console.WriteLine(newtonsoftMoneyJson);
Console.WriteLine(countryCurrency.CountryCurrency?.CountryAlpha2Code);
Console.WriteLine(addressCurrencyValidation.Succeeded);
Console.WriteLine(defaultMoneyDetected);
Console.WriteLine(converted.Output);
Console.WriteLine(converted.RateSource);

sealed class StaticExchangeRateProvider : IExchangeRateProvider
{
    private readonly ExchangeRate _rate;

    public StaticExchangeRateProvider(ExchangeRate rate)
    {
        _rate = rate;
    }

    public ExchangeRateLookupResult GetRate(
        CurrencyPair pair,
        DateTime effectiveDate,
        ExchangeRateKind rateKind)
    {
        if (_rate.Pair == pair && _rate.EffectiveDate == effectiveDate && _rate.Kind == rateKind)
        {
            return ExchangeRateLookupResult.Success(_rate);
        }

        return ExchangeRateLookupResult.Failure(
            ExchangeRateLookupFailureReason.RateUnavailable,
            $"No direct rate for {pair} on {effectiveDate:O}.");
    }
}

sealed class SmokeAddressValidatorFactory : IAddressValidatorFactory
{
    private readonly IAddressValidator _validator;

    public SmokeAddressValidatorFactory(AddressValidationResult result)
    {
        _validator = new SmokeAddressValidator(result);
    }

    public IAddressValidator GetValidator(AddressCountryCode countryCode)
    {
        return _validator;
    }

    public void RegisterValidator(AddressCountryCode countryCode, IAddressValidator validator)
    {
    }
}

sealed class SmokeAddressValidator : IAddressValidator
{
    private readonly AddressValidationResult _result;

    public SmokeAddressValidator(AddressValidationResult result)
    {
        _result = result;
    }

    public AddressValidationResult Validate(PostalAddress? address)
    {
        return _result;
    }
}
'@

$project = @'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
'@

$config = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local-smoke" value="$artifacts" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="local-smoke">
      <package pattern="ISOCodex.Currency*" />
    </packageSource>
    <packageSource key="nuget.org">
      <package pattern="*" />
    </packageSource>
  </packageSourceMapping>
</configuration>
"@

Set-Content -Path $consumerProject -Value $project -Encoding UTF8
Set-Content -Path $nugetConfig -Value $config -Encoding UTF8
$env:NUGET_PACKAGES = $packages
Invoke-DotNet add $consumerProject package ISOCodex.Currency --version $Version --no-restore
Invoke-DotNet add $consumerProject package ISOCodex.Currency.Analyzers --version $Version --no-restore
Invoke-DotNet add $consumerProject package ISOCodex.Currency.Addressing --version $Version --no-restore
Invoke-DotNet add $consumerProject package ISOCodex.Currency.AspNetCore --version $Version --no-restore
Invoke-DotNet add $consumerProject package ISOCodex.Currency.Countries --version $Version --no-restore
Invoke-DotNet add $consumerProject package ISOCodex.Currency.Dapper --version $Version --no-restore
Invoke-DotNet add $consumerProject package ISOCodex.Currency.EntityFrameworkCore --version $Version --no-restore
Invoke-DotNet add $consumerProject package ISOCodex.Currency.Exchange.Abstractions --version $Version --no-restore
Invoke-DotNet add $consumerProject package ISOCodex.Currency.Json.NewtonsoftJson --version $Version --no-restore
Invoke-DotNet add $consumerProject package ISOCodex.Currency.Json.SystemTextJson --version $Version --no-restore
Invoke-DotNet add $consumerProject package ISOCodex.Currency.Validation --version $Version --no-restore
Set-Content -Path "$tempRoot\Program.cs" -Value $program -Encoding UTF8
Invoke-DotNet restore $consumerProject --configfile $nugetConfig
Invoke-DotNet run --project $consumerProject -c Release --no-restore
