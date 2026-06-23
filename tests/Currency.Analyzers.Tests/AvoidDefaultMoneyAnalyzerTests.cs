using System.Collections.Immutable;
using ISOCodex.Currency;
using ISOCodex.Currency.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Currency.Analyzers.Tests;

public class AvoidDefaultMoneyAnalyzerTests
{
    [Fact]
    public async Task ReportsDefaultMoneyExpression()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using ISOCodex.Currency;

            public class Sample
            {
                public Money Create() => default(Money);
            }
            """);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(AvoidDefaultMoneyAnalyzer.DiagnosticId, diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Warning, diagnostic.Severity);
    }

    [Fact]
    public async Task ReportsDefaultLiteralConvertedToMoney()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using ISOCodex.Currency;

            public class Sample
            {
                public Money Create() => default;
            }
            """);

        Assert.Single(diagnostics);
    }

    [Fact]
    public async Task DoesNotReportValidMoneyFactoryUsage()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using ISOCodex.Currency;

            public class Sample
            {
                public Money Create() => Money.Zero(CurrencyCode.GBP);
            }
            """);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task ReportsDefaultCurrencyCodeExpression()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using ISOCodex.Currency;

            public class Sample
            {
                public CurrencyCode Create() => default(CurrencyCode);
            }
            """);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(AvoidDefaultMoneyAnalyzer.CurrencyCodeDiagnosticId, diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Warning, diagnostic.Severity);
    }

    [Fact]
    public async Task ReportsDefaultLiteralConvertedToCurrencyCode()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using ISOCodex.Currency;

            public class Sample
            {
                public CurrencyCode Create() => default;
            }
            """);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(AvoidDefaultMoneyAnalyzer.CurrencyCodeDiagnosticId, diagnostic.Id);
    }

    [Fact]
    public async Task DoesNotReportValidCurrencyCodeFactoryUsage()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using ISOCodex.Currency;

            public class Sample
            {
                public CurrencyCode Create() => CurrencyCode.Parse("GBP");
            }
            """);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task ReportsIgnoredMoneyValidationResult()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using ISOCodex.Currency;

            public class Sample
            {
                public void Create()
                {
                    Money.TryCreate(12.34m, CurrencyCode.GBP);
                }
            }
            """);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(DoNotIgnoreMoneyResultAnalyzer.DiagnosticId, diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Warning, diagnostic.Severity);
    }

    [Fact]
    public async Task ReportsIgnoredMoneyParseResult()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using ISOCodex.Currency;

            public class Sample
            {
                public void Parse()
                {
                    new MoneyParser().Parse("GBP 12.34");
                }
            }
            """);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(DoNotIgnoreMoneyResultAnalyzer.DiagnosticId, diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Warning, diagnostic.Severity);
    }

    [Fact]
    public async Task ReportsDiscardedMoneyValidationResult()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using ISOCodex.Currency;

            public class Sample
            {
                public void Create()
                {
                    _ = Money.TryCreate(12.34m, CurrencyCode.GBP);
                }
            }
            """);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(DoNotIgnoreMoneyResultAnalyzer.DiagnosticId, diagnostic.Id);
    }

    [Fact]
    public async Task DoesNotReportInspectedMoneyValidationResult()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using ISOCodex.Currency;

            public class Sample
            {
                public bool Create()
                {
                    var result = Money.TryCreate(12.34m, CurrencyCode.GBP);
                    return result.Succeeded;
                }
            }
            """);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task DoesNotReportBoolTryCreateOverload()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using ISOCodex.Currency;

            public class Sample
            {
                public bool Create()
                {
                    return Money.TryCreate(12.34m, CurrencyCode.GBP, out var money);
                }
            }
            """);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task ReportsSymbolParsingWithoutExpectedCurrency()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using System.Globalization;
            using ISOCodex.Currency;

            public class Sample
            {
                public MoneyParseOptions Create()
                {
                    return new MoneyParseOptions(
                        new CultureInfo("en-GB"),
                        currencyStyles: MoneyParseCurrencyStyles.Symbol);
                }
            }
            """);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(RequireExpectedCurrencyForSymbolParsingAnalyzer.DiagnosticId, diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Warning, diagnostic.Severity);
    }

    [Fact]
    public async Task ReportsCodeOrSymbolParsingWithoutExpectedCurrency()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using System.Globalization;
            using ISOCodex.Currency;

            public class Sample
            {
                public MoneyParseOptions Create()
                {
                    return new MoneyParseOptions(
                        new CultureInfo("en-GB"),
                        null,
                        MoneyParseCurrencyStyles.CodeOrSymbol);
                }
            }
            """);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(RequireExpectedCurrencyForSymbolParsingAnalyzer.DiagnosticId, diagnostic.Id);
    }

    [Fact]
    public async Task DoesNotReportSymbolParsingWithExpectedCurrency()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using System.Globalization;
            using ISOCodex.Currency;

            public class Sample
            {
                public MoneyParseOptions Create()
                {
                    return new MoneyParseOptions(
                        new CultureInfo("en-GB"),
                        CurrencyCode.GBP,
                        MoneyParseCurrencyStyles.CodeOrSymbol);
                }
            }
            """);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task DoesNotReportCodeOnlyParsingWithoutExpectedCurrency()
    {
        var diagnostics = await GetDiagnosticsAsync("""
            using System.Globalization;
            using ISOCodex.Currency;

            public class Sample
            {
                public MoneyParseOptions Create()
                {
                    return new MoneyParseOptions(
                        new CultureInfo("en-GB"),
                        currencyStyles: MoneyParseCurrencyStyles.Code);
                }
            }
            """);

        Assert.Empty(diagnostics);
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = CreateReferences();
        var compilation = CSharpCompilation.Create(
            "AnalyzerTests",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(
            new AvoidDefaultMoneyAnalyzer(),
            new DoNotIgnoreMoneyResultAnalyzer(),
            new RequireExpectedCurrencyForSymbolParsingAnalyzer());
        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);

        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
    }

    private static IEnumerable<MetadataReference> CreateReferences()
    {
        var trustedPlatformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?? throw new InvalidOperationException("Trusted platform assemblies were not available.");

        foreach (var path in trustedPlatformAssemblies.Split(Path.PathSeparator))
        {
            yield return MetadataReference.CreateFromFile(path);
        }

        yield return MetadataReference.CreateFromFile(typeof(Money).Assembly.Location);
    }
}
