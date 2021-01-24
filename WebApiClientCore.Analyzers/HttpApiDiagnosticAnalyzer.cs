using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using WebApiClientCore.Analyzers.Providers;

namespace WebApiClientCore.Analyzers
{
    /// <summary>
    /// 表示WebApiClient诊断分析器
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpApiDiagnosticAnalyzer : DiagnosticAnalyzer
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
                if (HttpApiContext.TryParse(syntaxNodeContext, out var httpApiContext))
                {
                    if (httpApiContext != null)
                    {
                        var diagnostics = this
                            .GetDiagnosticProviders(httpApiContext)
                            .SelectMany(d => d.CreateDiagnostics());

                        foreach (var item in diagnostics)
                        {
                            syntaxNodeContext.ReportDiagnostic(item);
                        }
                    }
                }
            }, SyntaxKind.InterfaceDeclaration);
        }

        /// <summary>
        /// 返回所有HttpApi诊断器
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IEnumerable<HttpApiDiagnosticProvider> GetDiagnosticProviders(HttpApiContext context)
        {
            yield return new CtorAttributeDiagnosticProvider(context);
            yield return new ReturnTypeDiagnosticProvider(context);
            yield return new RefParameterDiagnosticProvider(context);
            yield return new NotMethodDefindedDiagnosticProvider(context);
            yield return new GenericMethodDiagnosticProvider(context);
            yield return new UriAttributeDiagnosticProvider(context);
            yield return new ModifierDiagnosticProvider(context);
        }
    }
}