using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace WebApiClientCore.Analyzers
{
    /// <summary>
    /// 表示HttpApi诊断器提供者抽象类
    /// </summary>
    abstract class HttpApiDiagnosticProvider
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
        public HttpApiDiagnosticProvider(HttpApiContext context)
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
            return Diagnostic.Create(this.Descriptor, location, messageArgs);
        }

        /// <summary>
        /// 创建所有的诊断结果
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Diagnostic> CreateDiagnostics();
    }
}
