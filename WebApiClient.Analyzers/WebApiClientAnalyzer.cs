using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using WebApiClient.Analyzers.HttpApi;
using WebApiClient.Analyzers.Invocation;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// 表示WebApiClient诊断分析器
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WebApiClientAnalyzer : DiagnosticAnalyzer
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
                    Descriptors.NotMethodDefindedDescriptor,
                    Descriptors.GenericMethodDescriptor,
                    Descriptors.UriAttributeDescriptor,
                    Descriptors.HttpApiCreateDescriptor);
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
                if (httpApiContext.IsHtttApi == true)
                {
                    foreach (var item in this.GetHttpApiDiagnostics(httpApiContext))
                    {
                        item.Report();
                    }
                }
            }, SyntaxKind.InterfaceDeclaration);

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
        /// 返回所有HttpApi诊断器
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IEnumerable<HttpApiDiagnostic> GetHttpApiDiagnostics(HttpApiContext context)
        {
            yield return new AttributeDiagnostic(context);
            yield return new ReturnTypeDiagnostic(context);
            yield return new RefParameterDiagnostic(context);
            yield return new NotMethodDefindedDiagnostic(context);
            yield return new GenericMethodDiagnostic(context);
            yield return new UriAttributeDiagnostic(context);
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