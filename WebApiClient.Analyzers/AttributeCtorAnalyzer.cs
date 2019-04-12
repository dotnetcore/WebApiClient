using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// 表示特性的构造函数分析器
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AttributeCtorAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// 获取所支持的诊断
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AttributeDescriptor,
                    DiagnosticDescriptors.ReturnTypeDescriptor,
                    DiagnosticDescriptors.RefParameterDescriptor,
                    DiagnosticDescriptors.NotMethodDefindDescriptor);
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
                var httpApi = new WebApiClientHtttApi(syntaxNodeContext);
                httpApi.ReportDiagnostic();
            }, SyntaxKind.InterfaceDeclaration);
        }
    }
}