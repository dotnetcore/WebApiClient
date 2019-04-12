using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using WebApiClient.Analyzers.Diagnostics;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// 表示HttpApi诊断分析器
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpApiAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// 获取所支持的诊断
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    Descriptors.AttributeDescriptor,
                    Descriptors.ReturnTypeDescriptor,
                    Descriptors.RefParameterDescriptor,
                    Descriptors.NotMethodDefindedDescriptor);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context">上下文</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(syntaxNodeContext =>
            {
                var httpApiContext = new HttpApiContext(syntaxNodeContext);
                var httpApiDiagnostics = this.GetHttpApiDiagnostics(httpApiContext);
                foreach (var item in httpApiDiagnostics)
                {
                    item.Report();
                }
            }, SyntaxKind.InterfaceDeclaration);
        }

        /// <summary>
        /// 返回所有HttpApi诊断器
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IEnumerable<IHttpApiDiagnostic> GetHttpApiDiagnostics(HttpApiContext context)
        {
            yield return new AttributeDiagnostic(context);
            yield return new ReturnTypeDiagnostic(context);
            yield return new RefParameterDiagnostic(context);
            yield return new NotMethodDefindedDiagnostic(context);
        }
    }
}