using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ISOCodex.Currency.Analyzers;

/// <summary>
/// Reports symbol-based money parsing options that do not include an expected currency.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RequireExpectedCurrencyForSymbolParsingAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic identifier for symbol parsing without an expected currency.</summary>
    public const string DiagnosticId = "ISOCCUR005";

    private const long SymbolFlag = 2L;

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        "Require expected currency for symbol parsing",
        "Do not enable money symbol parsing without an expected currency",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Currency symbols are ambiguous. MoneyParseOptions should include an expected currency when symbol parsing is enabled.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
    }

    private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ObjectCreationExpressionSyntax)context.Node;
        var type = context.SemanticModel.GetTypeInfo(objectCreation, context.CancellationToken).Type;

        if (!IsMoneyParseOptions(type) || objectCreation.ArgumentList == null)
        {
            return;
        }

        var currencyStylesArgument = FindArgument(objectCreation.ArgumentList, "currencyStyles", 2);
        if (currencyStylesArgument == null || !IncludesSymbolParsing(context, currencyStylesArgument.Expression))
        {
            return;
        }

        var expectedCurrencyArgument = FindArgument(objectCreation.ArgumentList, "expectedCurrency", 1);
        if (expectedCurrencyArgument != null && !IsNullLikeExpectedCurrency(context, expectedCurrencyArgument.Expression))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Rule, currencyStylesArgument.GetLocation()));
    }

    private static bool IsMoneyParseOptions(ITypeSymbol? type)
    {
        return type is INamedTypeSymbol namedType
            && namedType.Name == "MoneyParseOptions"
            && namedType.ContainingNamespace.ToDisplayString() == "ISOCodex.Currency";
    }

    private static ArgumentSyntax? FindArgument(
        ArgumentListSyntax argumentList,
        string parameterName,
        int positionalIndex)
    {
        foreach (var argument in argumentList.Arguments)
        {
            if (argument.NameColon?.Name.Identifier.ValueText == parameterName)
            {
                return argument;
            }
        }

        var currentPositionalIndex = 0;
        foreach (var argument in argumentList.Arguments)
        {
            if (argument.NameColon != null)
            {
                continue;
            }

            if (currentPositionalIndex == positionalIndex)
            {
                return argument;
            }

            currentPositionalIndex++;
        }

        return null;
    }

    private static bool IncludesSymbolParsing(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
    {
        var constant = context.SemanticModel.GetConstantValue(expression, context.CancellationToken);
        if (!constant.HasValue || constant.Value == null)
        {
            return false;
        }

        try
        {
            var value = Convert.ToInt64(constant.Value);
            return (value & SymbolFlag) == SymbolFlag;
        }
        catch (InvalidCastException)
        {
            return false;
        }
    }

    private static bool IsNullLikeExpectedCurrency(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
    {
        if (expression.IsKind(SyntaxKind.NullLiteralExpression)
            || expression.IsKind(SyntaxKind.DefaultLiteralExpression))
        {
            return true;
        }

        if (expression is DefaultExpressionSyntax)
        {
            return true;
        }

        var constant = context.SemanticModel.GetConstantValue(expression, context.CancellationToken);
        return constant.HasValue && constant.Value == null;
    }
}
