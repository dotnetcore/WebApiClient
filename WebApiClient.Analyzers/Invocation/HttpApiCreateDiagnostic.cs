using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebApiClient.Analyzers.Invocation
{
    /// <summary>
    /// 表示HttpApi.Create方法诊断
    /// </summary>
    class HttpApiCreateDiagnostic : InvocationDiagnostic
    {
        /// <summary>
        /// Create方法名称
        /// </summary>
        private const string createMethodName = "Create";

        /// <summary>
        /// HttpApi的类型的名称
        /// </summary>
        private const string httpApiTypeName = "WebApiClient.HttpApi";

        /// <summary>
        /// 获取诊断描述
        /// </summary>
        public override DiagnosticDescriptor Descriptor => Descriptors.HttpApiCreateDescriptor;

        /// <summary>
        /// HttpApi.Create方法诊断
        /// </summary>
        /// <param name="context">调用上下文</param>
        public HttpApiCreateDiagnostic(InvocationContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 返回诊断结果 
        /// </summary>
        /// <returns></returns>
        protected override Diagnostic GetDiagnostic()
        {
            if (this.Context.IsMethodInvocation == false)
            {
                return null;
            }

            var httpApiType = this.Context.SyntaxNodeContext.Compilation.GetTypeByMetadataName(httpApiTypeName);
            if (httpApiType == null)
            {
                return null;
            }

            var method = this.Context.MethodSymbol;
            if (method.ContainingType.Equals(httpApiType) == false || method.Name != createMethodName)
            {
                return null;
            }

            var expressionSyntax = this.Context.InvocationSyntax.Expression as MemberAccessExpressionSyntax;
            if (expressionSyntax != null)
            {
                var localtion = expressionSyntax.Name.GetLocation();
                return this.CreateDiagnostic(localtion);
            }
            return null;
        }
    }
}
