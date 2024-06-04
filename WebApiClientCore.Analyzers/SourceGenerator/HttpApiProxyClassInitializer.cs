using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

namespace WebApiClientCore.Analyzers.SourceGenerator
{
    /// <summary>
    /// HttpApi代理类初始化器
    /// </summary>
    sealed class HttpApiProxyClassInitializer
    {
        private readonly Compilation compilation;
        private readonly IEnumerable<HttpApiProxyClass> proxyClasses;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName => $"{nameof(HttpApiProxyClassInitializer)}.cs";

        /// <summary>
        /// HttpApi代理类初始化器
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="proxyClasses"></param>
        public HttpApiProxyClassInitializer(Compilation compilation, IEnumerable<HttpApiProxyClass> proxyClasses)
        {
            this.compilation = compilation;
            this.proxyClasses = proxyClasses;
        }

        /// <summary>
        /// 转换为SourceText
        /// </summary>
        /// <returns></returns>
        public SourceText ToSourceText()
        {
            var code = this.ToString();
            return SourceText.From(code, Encoding.UTF8);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("#if NET5_0_OR_GREATER");
            builder.AppendLine("#pragma warning disable"); 
            builder.AppendLine($"namespace WebApiClientCore");
            builder.AppendLine("{");
            builder.AppendLine("    /// <summary>动态依赖初始化器</summary>");
            builder.AppendLine("    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]");
            builder.AppendLine($"    static partial class {nameof(HttpApiProxyClassInitializer)}");
            builder.AppendLine("    {");

            builder.AppendLine($"""
                        /// <summary>
                        /// 注册程序集{compilation.AssemblyName}的所有动态依赖
                        /// 避免程序集在裁剪时裁剪掉由SourceGenerator生成的代理类
                        /// </summary>
                """);

            builder.AppendLine("        [global::System.Runtime.CompilerServices.ModuleInitializer]");
            foreach (var item in this.proxyClasses)
            {
                builder.AppendLine($"        [global::System.Diagnostics.CodeAnalysis.DynamicDependency(global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All, typeof(global::{item.Namespace}.{item.ClassName}))]");
            }

            builder.AppendLine("        public static void Initialize()");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");
            builder.AppendLine("#pragma warning restore");
            builder.AppendLine("#endif");
            return builder.ToString();
        }

    }
}
