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
        private readonly IEnumerable<HttpApiProxyClass> proxyClasses;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName => $"{nameof(HttpApiProxyClassInitializer)}.g.cs";

        /// <summary>
        /// HttpApi代理类初始化器
        /// </summary> 
        /// <param name="proxyClasses"></param>
        public HttpApiProxyClassInitializer(IEnumerable<HttpApiProxyClass> proxyClasses)
        {
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
            builder.AppendLine("\t/// <summary>动态依赖初始化器</summary>");
            builder.AppendLine("\t[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]");
            builder.AppendLine($"\tstatic partial class {nameof(HttpApiProxyClassInitializer)}");
            builder.AppendLine("\t{");

            builder.AppendLine($"\t\t/// <summary>初始化本程序集的动态依赖</summary> ");
            builder.AppendLine("\t\t[global::System.Runtime.CompilerServices.ModuleInitializer]");
            foreach (var item in this.proxyClasses)
            {
                builder.AppendLine($"\t\t[global::System.Diagnostics.CodeAnalysis.DynamicDependency(global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All, typeof(global::{item.Namespace}.{item.ClassName}))]");
            }

            builder.AppendLine("\t\tpublic static void Initialize()");
            builder.AppendLine("\t\t{");
            builder.AppendLine("\t\t}");
            builder.AppendLine("\t}");
            builder.AppendLine("}");
            builder.AppendLine("#pragma warning restore");
            builder.AppendLine("#endif");
            return builder.ToString();
        }

    }
}
