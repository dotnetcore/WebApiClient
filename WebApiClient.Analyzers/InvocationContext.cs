using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// 表示调用上下文
    /// </summary>
    class InvocationContext
    {
        /// <summary>
        /// 获取语法节点上下文
        /// </summary>
        public SyntaxNodeAnalysisContext SyntaxNodeContext { get; }

        /// <summary>
        /// 获取调用语法树
        /// </summary>
        public InvocationExpressionSyntax InvocationSyntax { get; }

        /// <summary>
        /// 获取是否为方法调用
        /// </summary>
        public bool IsMethodInvocation { get; }

        /// <summary>
        /// 获取调用的方法
        /// </summary>
        public IMethodSymbol MethodSymbol { get; }

        /// <summary>
        /// 调用上下文
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
