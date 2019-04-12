using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApiClient.Analyzers.Diagnostics
{
    /// <summary>
    /// 表示泛型方法诊断器
    /// </summary>
    class GenericMethodDiagnostic : HttpApiDiagnostic
    {
        /// <summary>
        /// 获取诊断描述
        /// </summary>
        public override DiagnosticDescriptor Descriptor => Descriptors.GenericMethodDescriptor;

        /// <summary>
        /// 泛型方法诊断器
        /// </summary>
        /// <param name="context">上下文</param>
        public GenericMethodDiagnostic(HttpApiContext context)
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
                if (method.IsGenericMethod == true)
                {
                    var location = method.DeclaringSyntaxReferences.First().GetSyntax().GetLocation();
                    yield return this.CreateDiagnostic(location);
                }
            }
        }
    }
}
