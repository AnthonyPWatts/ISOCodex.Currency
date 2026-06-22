using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ISOCodex.Currency.Analyzers;

/// <summary>
/// Reports direct default Money creation.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AvoidDefaultMoneyAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic identifier for default Money usage.</summary>
    public const string DiagnosticId = "ISOCCUR001";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        "Avoid default Money values",
        "Avoid default(Money). Use Money.Zero(currency) or Money.Of(amount, currency).",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Money is a value type, so default(Money) can exist but is not a meaningful domain value.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

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

        if (IsMoney(type))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, defaultExpression.GetLocation()));
        }
    }

    private static void AnalyzeDefaultLiteral(SyntaxNodeAnalysisContext context)
    {
        var defaultLiteral = (LiteralExpressionSyntax)context.Node;
        var type = context.SemanticModel.GetTypeInfo(defaultLiteral, context.CancellationToken).ConvertedType;

        if (IsMoney(type))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, defaultLiteral.GetLocation()));
        }
    }

    private static bool IsMoney(ITypeSymbol? type)
    {
        return type is INamedTypeSymbol namedType
            && namedType.Name == "Money"
            && namedType.ContainingNamespace.ToDisplayString() == "ISOCodex.Currency";
    }
}
