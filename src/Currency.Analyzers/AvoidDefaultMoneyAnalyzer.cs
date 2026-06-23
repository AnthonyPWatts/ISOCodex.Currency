using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ISOCodex.Currency.Analyzers;

/// <summary>
/// Reports direct default Currency value creation.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AvoidDefaultMoneyAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic identifier for default Money usage.</summary>
    public const string DiagnosticId = "ISOCCUR001";

    /// <summary>The diagnostic identifier for default CurrencyCode usage.</summary>
    public const string CurrencyCodeDiagnosticId = "ISOCCUR002";

    private static readonly DiagnosticDescriptor MoneyRule = new DiagnosticDescriptor(
        DiagnosticId,
        "Avoid default Money values",
        "Avoid default(Money). Use Money.Zero(currency) or Money.Of(amount, currency).",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Money is a value type, so default(Money) can exist but is not a meaningful domain value.");

    private static readonly DiagnosticDescriptor CurrencyCodeRule = new DiagnosticDescriptor(
        CurrencyCodeDiagnosticId,
        "Avoid default CurrencyCode values",
        "Avoid default(CurrencyCode). Parse, TryParse, or use a known currency code.",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "CurrencyCode is a value type, so default(CurrencyCode) can exist but is not a meaningful domain value.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(MoneyRule, CurrencyCodeRule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeDefaultExpression, SyntaxKind.DefaultExpression);
        context.RegisterSyntaxNodeAction(AnalyzeDefaultLiteral, SyntaxKind.DefaultLiteralExpression);
    }

    private static void AnalyzeDefaultExpression(SyntaxNodeAnalysisContext context)
    {
        var defaultExpression = (DefaultExpressionSyntax)context.Node;
        var type = context.SemanticModel.GetTypeInfo(defaultExpression.Type, context.CancellationToken).Type;

        var rule = GetDefaultValueRule(type);
        if (rule != null)
        {
            context.ReportDiagnostic(Diagnostic.Create(rule, defaultExpression.GetLocation()));
        }
    }

    private static void AnalyzeDefaultLiteral(SyntaxNodeAnalysisContext context)
    {
        var defaultLiteral = (LiteralExpressionSyntax)context.Node;
        var type = context.SemanticModel.GetTypeInfo(defaultLiteral, context.CancellationToken).ConvertedType;

        var rule = GetDefaultValueRule(type);
        if (rule != null)
        {
            context.ReportDiagnostic(Diagnostic.Create(rule, defaultLiteral.GetLocation()));
        }
    }

    private static DiagnosticDescriptor? GetDefaultValueRule(ITypeSymbol? type)
    {
        if (type is not INamedTypeSymbol namedType
            || namedType.ContainingNamespace.ToDisplayString() != "ISOCodex.Currency")
        {
            return null;
        }

        return namedType.Name switch
        {
            "Money" => MoneyRule,
            "CurrencyCode" => CurrencyCodeRule,
            _ => null
        };
    }
}
