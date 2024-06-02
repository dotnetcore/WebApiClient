using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Linq;
using System.Text;

namespace WebApiClientCore.Analyzers.SourceGenerator
{
    /// <summary>
    /// HttpApi代码构建器
    /// </summary>
    sealed class HttpApiCodeBuilder : IEquatable<HttpApiCodeBuilder>
    {
        /// <summary>
        /// 接口符号
        /// </summary>
        private readonly INamedTypeSymbol httpApi;
        private readonly string httpApiFullName;

        /// <summary>
        /// 拦截器变量名
        /// </summary>
        private readonly string apiInterceptorFieldName = $"apiInterceptor_{(uint)Environment.TickCount}";

        /// <summary>
        /// action执行器变量名
        /// </summary>
        private readonly string actionInvokersFieldName = $"actionInvokers_{(uint)Environment.TickCount}";

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName => this.httpApi.ToDisplayString();

        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace => $"WebApiClientCore.{this.httpApi.ContainingNamespace}";

        /// <summary>
        /// 类型名
        /// </summary>
        public string ClassName => this.httpApi.Name;

        /// <summary>
        /// HttpApi代码构建器
        /// </summary>
        /// <param name="httpApi"></param>
        public HttpApiCodeBuilder(INamedTypeSymbol httpApi)
        {
            this.httpApi = httpApi;
            this.httpApiFullName = httpApi.GetFullName();
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

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"using System;");
            builder.AppendLine($"using System.Diagnostics;");

            builder.AppendLine($"#pragma warning disable CS8601");
            builder.AppendLine($"#pragma warning disable 1591");
            builder.AppendLine($"namespace {this.Namespace}");
            builder.AppendLine("{");

            builder.AppendLine($"\t[DebuggerTypeProxy(typeof({this.httpApiFullName}))]");
            builder.AppendLine($"\t[HttpApiProxyClass(typeof({this.httpApiFullName}))]");
            builder.AppendLine($"\tpublic class {this.ClassName}:{this.httpApiFullName}");
            builder.AppendLine("\t{");

            builder.AppendLine($"\t\tprivate readonly IHttpApiInterceptor {this.apiInterceptorFieldName};");
            builder.AppendLine($"\t\tprivate readonly ApiActionInvoker[] {this.actionInvokersFieldName};");

            builder.AppendLine($"\t\tpublic {this.httpApi.Name}(IHttpApiInterceptor apiInterceptor,ApiActionInvoker[] actionInvokers)");
            builder.AppendLine("\t\t{");
            builder.AppendLine($"\t\t\tthis.{this.apiInterceptorFieldName} = apiInterceptor;");
            builder.AppendLine($"\t\t\tthis.{this.actionInvokersFieldName} = actionInvokers;");
            builder.AppendLine("\t\t}");

            var index = 0;
            foreach (var method in HttpApiMethodFinder.FindApiMethods(this.httpApi))
            {
                var methodCode = this.BuildMethod(method, index);
                builder.AppendLine(methodCode);
                index += 1;
            }

            builder.AppendLine("\t}");
            builder.AppendLine("}");
            builder.AppendLine("#pragma warning restore 1591");
            builder.AppendLine("#pragma warning restore CS8601");

            // System.Diagnostics.Debugger.Launch();
            return builder.ToString();
        }


        /// <summary>
        /// 构建方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string BuildMethod(IMethodSymbol method, int index)
        {
            var builder = new StringBuilder();
            var parametersString = string.Join(",", method.Parameters.Select(item => $"{item.Type.GetFullName()} {item.Name}"));
            var parameterNamesString = string.Join(",", method.Parameters.Select(item => item.Name));
            var paremterArrayString = string.IsNullOrEmpty(parameterNamesString)
                ? "Array.Empty<object>()"
                : $"new object[] {{ {parameterNamesString} }}";

            var returnTypeString = method.ReturnType.GetFullName();
            builder.AppendLine($"\t\t[HttpApiProxyMethod({index})]");
            builder.AppendLine($"\t\tpublic {returnTypeString} {method.Name}( {parametersString} )");
            builder.AppendLine("\t\t{");
            builder.AppendLine($"\t\t\treturn ({returnTypeString})this.{this.apiInterceptorFieldName}.Intercept(this.{this.actionInvokersFieldName}[{index}], {paremterArrayString});");
            builder.AppendLine("\t\t}");
            return builder.ToString();
        }

        /// <summary>
        /// 是否与目标相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(HttpApiCodeBuilder other)
        {
            return this.FileName == other.FileName;
        }

        /// <summary>
        /// 是否与目标相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is HttpApiCodeBuilder builder)
            {
                return this.Equals(builder);
            }
            return false;
        }

        /// <summary>
        /// 获取哈希
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.FileName.GetHashCode();
        }
    }
}
