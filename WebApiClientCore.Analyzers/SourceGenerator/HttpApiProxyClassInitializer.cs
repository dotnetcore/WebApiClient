using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WebApiClientCore.Analyzers.SourceGenerator
{
    /// <summary>
    /// HttpApiProxyClass初始化器
    /// </summary>
    static class HttpApiProxyClassInitializer
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public static string FileName => $"{nameof(HttpApiProxyClassInitializer)}.g.cs";

        /// <summary>
        /// 转换为SourceText
        /// </summary>
        /// <returns></returns>
        public static SourceText ToSourceText()
        {
            var code = $$"""
                #if NET5_0_OR_GREATER
                #pragma warning disable
                namespace WebApiClientCore
                {
                	/// <summary>动态依赖初始化器</summary>
                	[global::System.Reflection.Obfuscation(Exclude = true)]
                    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                	static partial class HttpApiProxyClassInitializer
                	{
                		/// <summary>初始化本程序集的动态依赖</summary> 
                		[global::System.Runtime.CompilerServices.ModuleInitializer]
                		[global::System.Diagnostics.CodeAnalysis.DynamicDependency(global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All, typeof(global::WebApiClientCore.{{nameof(HttpApiProxyClass)}}))]
                		public static void Initialize()
                		{
                		}
                	}
                }
                #pragma warning restore
                #endif                
                """;
            return SourceText.From(code, Encoding.UTF8);
        }
    }
}
