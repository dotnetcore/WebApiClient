using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace WebApiClientCore.Analyzers
{
    /// <summary>
    /// 表示HttpApi诊断器抽象类
    /// </summary>
    abstract class HttpApiDiagnostic
    {
        /// <summary>
        /// 获取上下文
        /// </summary>
        protected HttpApiContext Context { get; }

        /// <summary>
        /// 获取诊断描述
        /// </summary>
        public abstract DiagnosticDescriptor Descriptor { get; }

        /// <summary>
        /// HttpApi诊断器
        /// </summary>
        /// <param name="context">上下文</param>
        public HttpApiDiagnostic(HttpApiContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// 创建诊断结果
        /// </summary>
        /// <param name="location"></param>
        /// <param name="messageArgs"></param>
        /// <returns></returns>
        protected Diagnostic CreateDiagnostic(Location location, params object[] messageArgs)
        {
            return location == null ? null : Diagnostic.Create(this.Descriptor, location, messageArgs);
        }

        /// <summary>
        /// 返回HttpApi的所有方法
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<IMethodSymbol> GetApiMethodSymbols()
        {
            foreach (var member in this.Context.HttpApiSyntax.Members)
            {
                if (member.Kind() != SyntaxKind.MethodDeclaration)
                {
                    continue;
                }

                if (this.Context.SyntaxNodeContext.SemanticModel.GetDeclaredSymbol(member) is IMethodSymbol symbol)
                {
                    yield return symbol;
                }
            }
        }

        /// <summary>
        /// 报告诊断结果
        /// </summary>
        public void Report()
        {
            if (this.Context.IsHtttApi == false)
            {
                return;
            }

            foreach (var item in this.GetDiagnostics())
            {
                if (item != null)
                {
                    this.Context.SyntaxNodeContext.ReportDiagnostic(item);
                }
            }
        }

        /// <summary>
        /// 返回所有的报告诊断
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<Diagnostic> GetDiagnostics();
    }
}
