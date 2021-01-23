using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore.Analyzers.DiagnosticProviders
{
    /// <summary>
    /// 表示引用传递参数诊断器
    /// </summary>
    sealed class RefParameterDiagnosticProvider : HttpApiDiagnosticProvider
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
        public RefParameterDiagnosticProvider(HttpApiContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 返回所有的报告诊断
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Diagnostic> CreateDiagnostics()
        {
            foreach (var method in this.Context.ApiMethods)
            {
                foreach (var parameter in method.Parameters)
                {
                    if (parameter.RefKind == RefKind.None)
                    {
                        continue;
                    }

                    if (parameter.DeclaringSyntaxReferences.Length == 0)
                    {
                        continue;
                    }

                    var declaringSyntax = parameter.DeclaringSyntaxReferences.First().GetSyntax();
                    if (declaringSyntax is ParameterSyntax parameterSyntax)
                    {
                        var modifier = parameterSyntax.Modifiers.FirstOrDefault();
                        if (modifier != null)
                        {
                            var location = modifier.GetLocation();
                            yield return this.CreateDiagnostic(location);
                        }
                    }
                }
            }
        }
    }
}
