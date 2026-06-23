using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ISOCodex.Currency.Analyzers;

/// <summary>
/// Reports ignored money validation and parse result values.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DoNotIgnoreMoneyResultAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic identifier for ignored money validation or parse result values.</summary>
    public const string DiagnosticId = "ISOCCUR003";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        "Do not ignore money validation or parse results",
        "Do not ignore {0}. Check Succeeded and inspect the failure reason before continuing.",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "MoneyValidationResult and MoneyParseResult carry validation state that should be checked at application boundaries.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeExpressionStatement, SyntaxKind.ExpressionStatement);
    }

    private static void AnalyzeExpressionStatement(SyntaxNodeAnalysisContext context)
    {
        var statement = (ExpressionStatementSyntax)context.Node;
        var expression = statement.Expression;

        if (expression is AssignmentExpressionSyntax assignment
            && assignment.Left is IdentifierNameSyntax identifier
            && identifier.Identifier.ValueText == "_")
        {
            ReportIfIgnoredResult(context, assignment.Right, assignment.Right.GetLocation());
            return;
        }

        ReportIfIgnoredResult(context, expression, expression.GetLocation());
    }

    private static void ReportIfIgnoredResult(
        SyntaxNodeAnalysisContext context,
        ExpressionSyntax expression,
        Location location)
    {
        var type = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken).Type;

        if (type is INamedTypeSymbol namedType
            && namedType.ContainingNamespace.ToDisplayString() == "ISOCodex.Currency"
            && (namedType.Name == "MoneyValidationResult" || namedType.Name == "MoneyParseResult"))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, location, namedType.Name));
        }
    }
}
