using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using WebApiClientCore.Analyzers.HttpApi;

namespace WebApiClientCore.Analyzers
{
    /// <summary>
    /// 表示WebApiClient诊断分析器
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WebApiClientCoreAnalyzer : DiagnosticAnalyzer
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
                    Descriptors.ModifierDescriptor);
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
            yield return new ModifierDiagnostic(context);
        }
    }
}