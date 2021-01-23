using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore.Analyzers.DiagnosticProviders
{
    /// <summary>
    /// 表示返回类型诊断器
    /// </summary>
    sealed class ReturnTypeDiagnosticProvider : HttpApiDiagnosticProvider
    {
        /// <summary>   
        /// 获取诊断描述
        /// </summary>
        /// </summary>
        public override DiagnosticDescriptor Descriptor => Descriptors.ReturnTypeDescriptor;

        /// <summary>
        /// 返回类型诊断器
        /// </summary>
        /// <param name="context">上下文</param>
        public ReturnTypeDiagnosticProvider(HttpApiContext context)
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
                var name = method.ReturnType.MetadataName;
                if (name == "ITask`1" || name == "Task`1" || name == "Task")
                {
                    continue;
                }

                if (method.DeclaringSyntaxReferences.Length == 0)
                {
                    continue;
                }

                var declaringSyntax = method.DeclaringSyntaxReferences.First();
                if (declaringSyntax.GetSyntax() is MethodDeclarationSyntax methodDeclaration)
                {
                    var location = methodDeclaration.ReturnType.GetLocation();
                    yield return this.CreateDiagnostic(location);
                }
            }
        }
    }
}
