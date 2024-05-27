using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApiClientCore.Analyzers.SourceGenerator
{
    sealed class DynamicDependencyBuilder
    {
        private readonly Compilation compilation;
        private readonly IEnumerable<HttpApiCodeBuilder> codeBuilders;

        public string FileName => "WebApiClientBuilderExtensions.g.cs";
        public string ClassName => "WebApiClientBuilderExtensions_G";

        public DynamicDependencyBuilder(Compilation compilation, IEnumerable<HttpApiCodeBuilder> codeBuilders)
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
            builder.AppendLine("namespace Microsoft.Extensions.DependencyInjection");
            builder.AppendLine("{");
            builder.AppendLine("    /// <summary>IWebApiClientBuilder扩展</summary>");
            builder.AppendLine($"    public static partial class {this.ClassName}");
            builder.AppendLine("    {");

            builder.AppendLine($"""
                        /// <summary>
                        /// 注册程序集{compilation.AssemblyName}的所有动态依赖
                        /// 避免程序集在裁剪时裁剪掉由SourceGenerator生成的代理类
                        /// </summary>
                        /// <param name="builder"></param> 
                        /// <returns></returns>
                """);

            var assemblyName = GetAssemblyName(compilation);
            foreach (var codeBuilder in this.codeBuilders)
            {
                builder.AppendLine($"        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof({codeBuilder.Namespace}.{codeBuilder.ClassName}))]");
            }

            builder.AppendLine($"        public static IWebApiClientBuilder AddDynamicDependency{assemblyName}(this IWebApiClientBuilder builder)");
            builder.AppendLine("        {");
            builder.AppendLine("            return builder;");
            builder.AppendLine("        }");
             
            builder.AppendLine("    }");
            builder.AppendLine("}");
            builder.AppendLine("#endif");
            return builder.ToString();
        }

        private static string GetAssemblyName(Compilation compilation)
        {
            var assemblyName = compilation.AssemblyName ?? string.Empty;
            return new string(assemblyName.Where(IsAllowChar).ToArray());

            static bool IsAllowChar(char c)
            {
                return ('0' <= c && c <= '9') || ('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z');
            }
        }
    }
}
