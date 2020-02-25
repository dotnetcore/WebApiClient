using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// Represents the calling context
    /// </summary>
    class InvocationContext
    {
        /// <summary>
        /// Get syntax node context
        /// </summary>
        public SyntaxNodeAnalysisContext SyntaxNodeContext { get; }

        /// <summary>
        /// Get call syntax tree
        /// </summary>
        public InvocationExpressionSyntax InvocationSyntax { get; }

        /// <summary>
        /// Get whether method call
        /// </summary>
        public bool IsMethodInvocation { get; }

        /// <summary>
        /// Get called method
        /// </summary>
        public IMethodSymbol MethodSymbol { get; }

        /// <summary>
        /// Calling context
        /// </summary>
        /// <param name="syntaxNodeContext"></param>
        public InvocationContext(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            this.SyntaxNodeContext = syntaxNodeContext;
            this.InvocationSyntax = syntaxNodeContext.Node as InvocationExpressionSyntax;

            var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNodeContext.Node);
            this.IsMethodInvocation = symbolInfo.Symbol?.Kind == SymbolKind.Method;

            if (this.IsMethodInvocation == true)
            {
                this.MethodSymbol = symbolInfo.Symbol as IMethodSymbol;
            }
        }
    }
}
