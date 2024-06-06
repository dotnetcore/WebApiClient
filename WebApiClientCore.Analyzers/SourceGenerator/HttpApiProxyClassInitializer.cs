using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WebApiClientCore.Analyzers.SourceGenerator
{
    /// <summary>
    /// HttpApiProxyClass的静态类初始器
    /// </summary>
    static class HttpApiProxyClassInitializer
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
                    /// <summary>HttpApi代理类</summary>
                    [global::System.Reflection.Obfuscation(Exclude = true)]
                    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                    static partial class {{nameof(HttpApiProxyClass)}}
                    {
                #if NET5_0_OR_GREATER
                        /// <summary>初始化代理类</summary> 
                		[global::System.Runtime.CompilerServices.ModuleInitializer]
                		[global::System.Diagnostics.CodeAnalysis.DynamicDependency(global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All, typeof(global::WebApiClientCore.{{nameof(HttpApiProxyClass)}}))]
                		public static void Initialize()
                		{
                		}
                #endif
                    }
                }
                #pragma warning restore
                """;
            return SourceText.From(code, Encoding.UTF8);
        }
    }
}
