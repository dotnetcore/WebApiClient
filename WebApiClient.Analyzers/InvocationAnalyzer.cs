using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using WebApiClient.Analyzers.Invocation;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// 表示调用诊断分析器
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InvocationAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// 获取所支持的诊断
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    Descriptors.HttpApiCreateDescriptor);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context">上下文</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(syntaxContext =>
            {
                var invocationContext = new InvocationContext(syntaxContext);
                foreach (var item in this.GetInvocationDiagnostics(invocationContext))
                {
                    item.Report();
                }
            }, SyntaxKind.InvocationExpression);
        }

        /// <summary>
        /// 返回所有的调用诊断器
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IEnumerable<InvocationDiagnostic> GetInvocationDiagnostics(InvocationContext context)
        {
            yield return new HttpApiCreateDiagnostic(context);
        }
    }
}
