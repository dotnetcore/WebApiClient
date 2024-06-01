using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace WebApiClientCore.Analyzers.Providers
{
    /// <summary>
    /// 表示泛型接口诊断器
    /// </summary>
    sealed class GenericInterfaceDiagnosticProvider : HttpApiDiagnosticProvider
    {
        /// <summary>
        /// 获取诊断描述
        /// </summary>
        public override DiagnosticDescriptor Descriptor => Descriptors.GenericInterfaceDescriptor;

        /// <summary>
        /// 泛型接口诊断器
        /// </summary>
        /// <param name="context">上下文</param>
        public GenericInterfaceDiagnosticProvider(HttpApiContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 返回所有的报告诊断
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Diagnostic> CreateDiagnostics()
        {
            if (this.Context.Interface.IsGenericType)
            {
                var location = Context.Syntax.Identifier.GetLocation();
                yield return this.CreateDiagnostic(location);
            } 
        }
    }
}
