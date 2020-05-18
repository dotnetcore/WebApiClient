using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore.Analyzers.HttpApi
{
    /// <summary>
    /// 表示UriAttribute诊断器
    /// </summary>
    class UriAttributeDiagnostic : HttpApiDiagnostic
    {
        /// <summary>   
        /// 获取诊断描述
        /// </summary>
        /// </summary>
        public override DiagnosticDescriptor Descriptor => Descriptors.UriAttributeDescriptor;

        /// <summary>
        /// UriAttribute诊断器
        /// </summary>
        /// <param name="context">上下文</param>
        public UriAttributeDiagnostic(HttpApiContext context)
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
                for (var i = 1; i < method.Parameters.Length; i++)
                {
                    var parameter = method.Parameters[i];
                    var uriAttribute = parameter.GetAttributes().FirstOrDefault(item => item.AttributeClass.Equals(this.Context.UriAttributeType));
                    if (uriAttribute != null)
                    {
                        var location = uriAttribute.ApplicationSyntaxReference?.GetSyntax()?.GetLocation();
                        yield return this.CreateDiagnostic(location);
                    }
                }
            }
        }
    }
}
