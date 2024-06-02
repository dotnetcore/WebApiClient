using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

namespace WebApiClientCore.Analyzers.SourceGenerator
{
    sealed class HttpApiProxyClassInitializer
    {
        private readonly Compilation compilation;
        private readonly IEnumerable<HttpApiCodeBuilder> codeBuilders;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName => $"{nameof(HttpApiProxyClassInitializer)}.cs";

        public HttpApiProxyClassInitializer(Compilation compilation, IEnumerable<HttpApiCodeBuilder> codeBuilders)
        {
            this.compilation = compilation;
            this.codeBuilders = codeBuilders;
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
            builder.AppendLine("using System.Diagnostics.CodeAnalysis;");
            builder.AppendLine("using System.Runtime.CompilerServices;");
            builder.AppendLine($"namespace WebApiClientCore");
            builder.AppendLine("{");
            builder.AppendLine("    /// <summary>动态依赖初始化器</summary>");
            builder.AppendLine($"    static partial class {nameof(HttpApiProxyClassInitializer)}");
            builder.AppendLine("    {");

            builder.AppendLine($"""
                        /// <summary>
                        /// 注册程序集{compilation.AssemblyName}的所有动态依赖
                        /// 避免程序集在裁剪时裁剪掉由SourceGenerator生成的代理类
                        /// </summary>
                """);

            builder.AppendLine("        [ModuleInitializer]");
            foreach (var codeBuilder in this.codeBuilders)
            {
                builder.AppendLine($"        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof({codeBuilder.Namespace}.{codeBuilder.ClassName}))]");
            }

            builder.AppendLine("        public static void Initialize()");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");
            builder.AppendLine("#endif");
            return builder.ToString();
        }

    }
}
