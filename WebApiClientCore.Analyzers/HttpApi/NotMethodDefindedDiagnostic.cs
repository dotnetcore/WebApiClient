using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace WebApiClientCore.Analyzers.HttpApi
{
    /// <summary>
    /// 表示非方法声明诊断器
    /// </summary>
    class NotMethodDefindedDiagnostic : HttpApiDiagnostic
    {
        /// <summary>
        /// /// <summary>
        /// 获取诊断描述
        /// </summary>
        /// </summary>
        public override DiagnosticDescriptor Descriptor => Descriptors.NotMethodDefindedDescriptor;

        /// <summary>
        /// 非方法声明诊断器
        /// </summary>
        /// <param name="context">上下文</param>
        public NotMethodDefindedDiagnostic(HttpApiContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 返回所有的报告诊断
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Diagnostic> GetDiagnostics()
        {
            foreach (var member in this.Context.HttpApiSyntax.Members)
            {
                if (member.Kind() != SyntaxKind.MethodDeclaration)
                {
                    var location = member.GetLocation();
                    yield return this.CreateDiagnostic(location);
                }
            }
        }
    }
}
