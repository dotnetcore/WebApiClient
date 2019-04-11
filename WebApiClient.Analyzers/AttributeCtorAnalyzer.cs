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
        /// 诊断描述器
        /// </summary>
        private readonly DiagnosticDescriptor diagnosticDescriptor;

        /// <summary>
        /// 特性的构造函数分析器
        /// </summary>
        public AttributeCtorAnalyzer()
        {
            const string id = "AC1001";
            const string title = "特性构造函数不匹配";
            const string category = "Error";
            const string messageFormat = "不支持特性的此构造函数，请使用其它构造函数";
            const string helpLinkUri = "https://github.com/dotnetcore/WebApiClient/wiki/WebApiClient%E5%9F%BA%E7%A1%80";
            this.diagnosticDescriptor = new DiagnosticDescriptor(id, title, messageFormat, category, DiagnosticSeverity.Error, true, helpLinkUri: helpLinkUri);
        }

        /// <summary>
        /// 获取所支持的诊断
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get => ImmutableArray.Create(this.diagnosticDescriptor);
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
                var diagnosticAttributes = httpApi.GetDiagnosticAttributes();

                foreach (var item in diagnosticAttributes)
                {
                    var location = item.ApplicationSyntaxReference.GetSyntax().GetLocation();
                    var diagnostic = Diagnostic.Create(this.diagnosticDescriptor, location);
                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                }
            }, SyntaxKind.InterfaceDeclaration);
        }
    }
}