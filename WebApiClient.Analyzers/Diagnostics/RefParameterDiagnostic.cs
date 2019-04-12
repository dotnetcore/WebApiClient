using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClient.Analyzers.Diagnostics
{
    /// <summary>
    /// 表示引用传递参数诊断器
    /// </summary>
    class RefParameterDiagnostic : HttpApiDiagnostic
    {
        /// <summary>
        /// /// <summary>
        /// 获取诊断描述
        /// </summary>
        /// </summary>
        public override DiagnosticDescriptor Descriptor => Descriptors.RefParameterDescriptor;

        /// <summary>
        /// 引用传递参数诊断器
        /// </summary>
        /// <param name="context">上下文</param>
        public RefParameterDiagnostic(HttpApiContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 返回所有的报告诊断
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Diagnostic> GetDiagnostics()
        {
            foreach (var method in this.GetApiMethodSymbols())
            {
                foreach (var parameter in method.Parameters)
                {
                    if (parameter.RefKind != RefKind.None)
                    {
                        var parameterSyntax = parameter.DeclaringSyntaxReferences.First().GetSyntax() as ParameterSyntax;
                        var location = parameterSyntax.Modifiers.First().GetLocation();
                        yield return this.CreateDiagnostic(location);
                    }
                }
            }
        }

    }
}
