using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace WebApiClient.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AttributeCtorAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                var id = "unexpected ctor";
                var title = "不支持使用的特性构造函数";
                var category = "Error";
                var messageFormat = "无效构造器无效构造器";
                var descriptor = new DiagnosticDescriptor(id, title, messageFormat, category, DiagnosticSeverity.Error, true);
                return ImmutableArray.Create(descriptor);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(symbolContext =>
            {
                //System.Diagnostics.Debugger.Launch();
                var symbol = (IMethodSymbol)symbolContext.Symbol;

                var a = symbol.GetAttributes().FirstOrDefault();
                if (a != null)
                {
                    if (a.ConstructorArguments.Length == 0)
                    {
                        Report(a.ApplicationSyntaxReference.GetSyntax().GetLocation());
                    }
                }

                foreach (var p in symbol.Parameters)
                {
                    var pa = p.GetAttributes().FirstOrDefault();
                    if (pa != null && pa.ConstructorArguments.Length > 0)
                    {
                        Report(pa.ApplicationSyntaxReference.GetSyntax().GetLocation());
                    }
                }


                void Report(Location location)
                {
                    var diagnostic = Diagnostic.Create(
                        this.SupportedDiagnostics.First(),
                        location);

                    symbolContext.ReportDiagnostic(diagnostic);
                }

            }, SymbolKind.Method);
        }
    }
}
