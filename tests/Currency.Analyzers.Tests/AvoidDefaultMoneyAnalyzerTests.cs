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

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = CreateReferences();
        var compilation = CSharpCompilation.Create(
            "AnalyzerTests",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new AvoidDefaultMoneyAnalyzer());
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
