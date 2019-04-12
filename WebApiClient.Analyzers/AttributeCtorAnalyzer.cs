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
        /// 特性诊断描述器
        /// </summary>
        private static readonly DiagnosticDescriptor attributeDescriptor =
            Rule("AC1001", "特性构造函数不匹配", "不支持特性的此构造函数，请使用其它构造函数");

        /// <summary>
        /// 方法返回类型诊断描述器
        /// </summary>
        private static readonly DiagnosticDescriptor returnDescriptor =
            Rule("RT1001", "不支持的返回类型", "返回类型必须为ITask<>或Task<T>");


        /// <summary>
        /// 创建诊断描述器
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="helpLinkUri"></param>
        /// <returns></returns>
        private static DiagnosticDescriptor Rule(string id, string title, string message, string helpLinkUri = null)
        {
            const string category = "Error";
            if (string.IsNullOrEmpty(helpLinkUri) == true)
            {
                helpLinkUri = "https://github.com/dotnetcore/WebApiClient/wiki/WebApiClient%E5%9F%BA%E7%A1%80";
            }
            return new DiagnosticDescriptor(id, title, message, category, DiagnosticSeverity.Error, true, helpLinkUri: helpLinkUri);
        }

        /// <summary>
        /// 获取所支持的诊断
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get => ImmutableArray.Create(attributeDescriptor, returnDescriptor);
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
                var diagnosticReturnSyntaxs = httpApi.GetDiagnosticReturnSyntaxs();

                foreach (var item in diagnosticReturnSyntaxs)
                {
                    var location = item.GetLocation();
                    var diagnostic = Diagnostic.Create(returnDescriptor, location);
                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                }

                var diagnosticAttributes = httpApi.GetDiagnosticAttributes();
                foreach (var item in diagnosticAttributes)
                {
                    var location = item.ApplicationSyntaxReference.GetSyntax().GetLocation();
                    var diagnostic = Diagnostic.Create(attributeDescriptor, location);
                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                }
            }, SyntaxKind.InterfaceDeclaration);
        }
    }
}