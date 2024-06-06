using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WebApiClientCore.Analyzers.SourceGenerator
{
    /// <summary>
    /// HttpApiProxyClass的静态类
    /// </summary>
    static class HttpApiProxyClassStatic
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public static string FileName => $"{nameof(HttpApiProxyClass)}.g.cs";

        /// <summary>
        /// 转换为SourceText
        /// </summary>
        /// <returns></returns>
        public static SourceText ToSourceText()
        {
            var code = $$"""
                #pragma warning disable
                using System;
                namespace WebApiClientCore
                {
                    [global::System.Reflection.Obfuscation(Exclude = true)]
                    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                    static partial class {{nameof(HttpApiProxyClass)}}
                    {
                    }
                }
                #pragma warning restore
                """;
            return SourceText.From(code, Encoding.UTF8);
        }
    }
}
