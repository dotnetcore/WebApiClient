using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApiClientCore.Analyzers.SourceGenerator
{
    /// <summary>
    /// HttpApi代理类生成器
    /// </summary>
    sealed class HttpApiProxyClass : IEquatable<HttpApiProxyClass>
    {
        private readonly INamedTypeSymbol httpApi;
        private readonly string httpApiFullName;
        private readonly string proxyClassName;

        private const string ApiInterceptorFieldName = "_apiInterceptor";
        private const string ActionInvokersFieldName = "_actionInvokers";

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// HttpApi代理类生成器
        /// </summary>
        /// <param name="httpApi"></param>
        public HttpApiProxyClass(INamedTypeSymbol httpApi)
        {
            this.httpApi = httpApi;
            this.httpApiFullName = GetFullName(httpApi);
            this.proxyClassName = httpApi.ToDisplayString().Replace(".", "_");
            this.FileName = $"{nameof(HttpApiProxyClass)}.{proxyClassName}.g.cs";
        }

        /// <summary>
        /// 获取完整名称
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private static string GetFullName(ISymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
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
            builder.AppendLine("#pragma warning disable");
            builder.AppendLine($"namespace WebApiClientCore");
            builder.AppendLine("{");
            builder.AppendLine($"\tpartial class {nameof(HttpApiProxyClass)}");
            builder.AppendLine("\t{");
            builder.AppendLine($"\t\t[global::WebApiClientCore.HttpApiProxyClass(typeof({this.httpApiFullName}))]");
            builder.AppendLine($"\t\t[global::System.Diagnostics.DebuggerTypeProxy(typeof({this.httpApiFullName}))]");
            builder.AppendLine($"\t\tsealed partial class {this.proxyClassName} : {this.httpApiFullName}");
            builder.AppendLine("\t\t{");

            builder.AppendLine($"\t\t\tprivate readonly global::WebApiClientCore.IHttpApiInterceptor {ApiInterceptorFieldName};");
            builder.AppendLine($"\t\t\tprivate readonly global::WebApiClientCore.ApiActionInvoker[] {ActionInvokersFieldName};");
            builder.AppendLine();
            builder.AppendLine($"\t\t\tpublic {this.proxyClassName}(global::WebApiClientCore.IHttpApiInterceptor apiInterceptor, global::WebApiClientCore.ApiActionInvoker[] actionInvokers)");
            builder.AppendLine("\t\t\t{");
            builder.AppendLine($"\t\t\t\tthis.{ApiInterceptorFieldName} = apiInterceptor;");
            builder.AppendLine($"\t\t\t\tthis.{ActionInvokersFieldName} = actionInvokers;");
            builder.AppendLine("\t\t\t}");
            builder.AppendLine();

            var index = 0;
            foreach (var declaringType in this.httpApi.AllInterfaces.Append(httpApi))
            {
                foreach (var method in declaringType.GetMembers().OfType<IMethodSymbol>())
                {
                    var methodCode = this.BuildMethod(declaringType, method, index);
                    builder.AppendLine(methodCode);
                    index += 1;
                }
            }

            builder.AppendLine("\t\t}");
            builder.AppendLine("\t}");
            builder.AppendLine("}");
            builder.AppendLine("#pragma warning restore");

            return builder.ToString();
        }

        /// <summary>
        /// 构建方法
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="method"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string BuildMethod(INamedTypeSymbol declaringType, IMethodSymbol method, int index)
        {
            var builder = new StringBuilder();
            var parametersString = string.Join(", ", method.Parameters.Select((item, i) => $"{GetFullName(item.Type)} p{i}"));
            var parameterNamesString = string.Join(", ", method.Parameters.Select((item, i) => $"p{i}"));
            var parameterArrayString = string.IsNullOrEmpty(parameterNamesString)
                ? "global::System.Array.Empty<global::System.Object>()"
                : $"new global::System.Object[] {{ {parameterNamesString} }}";

            var returnTypeString = GetFullName(method.ReturnType);
            var declaringTypeString = GetFullName(declaringType);

            builder.AppendLine($"\t\t\t[global::WebApiClientCore.HttpApiProxyMethod({index}, \"{method.Name}\", typeof({declaringTypeString}))]");
            builder.AppendLine($"\t\t\t{returnTypeString} {declaringTypeString}.{method.Name}({parametersString})");
            builder.AppendLine("\t\t\t{");
            builder.AppendLine($"\t\t\t\treturn ({returnTypeString})this.{ApiInterceptorFieldName}.Intercept(this.{ActionInvokersFieldName}[{index}], {parameterArrayString});");
            builder.AppendLine("\t\t\t}");
            return builder.ToString();
        }

        /// <summary>
        /// 是否与目标相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(HttpApiProxyClass? other)
        {
            return other != null && this.FileName == other.FileName;
        }

        /// <summary>
        /// 是否与目标相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            return obj is HttpApiProxyClass other && this.Equals(other);
        }

        /// <summary>
        /// 获取哈希
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.FileName.GetHashCode();
        }


        /// <summary>
        /// 表示MethodInfo的相等比较器
        /// </summary>
        private class MethodEqualityComparer : IEqualityComparer<IMethodSymbol>
        {
            public static MethodEqualityComparer Default { get; } = new MethodEqualityComparer();

            public bool Equals(IMethodSymbol x, IMethodSymbol y)
            {
                if (x.Name != y.Name || !x.ReturnType.Equals(y.ReturnType, SymbolEqualityComparer.Default))
                {
                    return false;
                }

                var xParameterTypes = x.Parameters.Select(p => p.Type);
                var yParameterTypes = y.Parameters.Select(p => p.Type);
                return xParameterTypes.SequenceEqual(yParameterTypes, SymbolEqualityComparer.Default);
            }

            public int GetHashCode(IMethodSymbol obj)
            {
#pragma warning disable RS1024
                var hashCode = 0;
                hashCode ^= obj.Name.GetHashCode();
                hashCode ^= obj.ReturnType.GetHashCode();
                foreach (var parameter in obj.Parameters)
                {
                    hashCode ^= parameter.Type.GetHashCode();
                }
                return hashCode;
#pragma warning restore RS1024
            }
        }
    }
}
