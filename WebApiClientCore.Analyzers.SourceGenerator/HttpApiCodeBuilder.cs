﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// 拦截器变量名
        /// </summary>
        private readonly string apiInterceptorFieldName = $"apiInterceptor_{(uint)Environment.TickCount}";

        /// <summary>
        /// action执行器变量名
        /// </summary>
        private readonly string actionInvokersFieldName = $"actionInvokers_{(uint)Environment.TickCount}";

        /// <summary>
        /// using
        /// </summary>
        public IEnumerable<string> Usings
        {
            get
            {
                yield return "using System;";
                yield return "using System.Diagnostics;";
                yield return "using WebApiClientCore;";
            }
        }

        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace => $"{this.httpApi.ContainingNamespace}.SourceGenerators";

        /// <summary>
        /// 基础接口名
        /// </summary>
        public string BaseInterfaceName => this.httpApi.ToDisplayString();

        /// <summary>
        /// 代理的接口类型名称
        /// </summary>
        public string HttpApiTypeName => this.httpApi.IsGenericType ? this.httpApi.ConstructUnboundGenericType().ToDisplayString() : this.httpApi.ToDisplayString();

        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName => this.httpApi.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        /// <summary>
        /// 构造器名
        /// </summary>
        public string CtorName => this.httpApi.Name;

        /// <summary>
        /// HttpApi代码构建器
        /// </summary>
        /// <param name="httpApi"></param>
        public HttpApiCodeBuilder(INamedTypeSymbol httpApi)
        {
            this.httpApi = httpApi;
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
            foreach (var item in this.Usings)
            {
                builder.AppendLine(item);
            }
            builder.AppendLine($"namespace {this.Namespace}");
            builder.AppendLine("{");
            builder.AppendLine($"\t[HttpApiProxyClass(typeof({this.HttpApiTypeName}))]");
            builder.AppendLine($"\tpublic class {this.ClassName}:{this.BaseInterfaceName}");
            builder.AppendLine("\t{");

            builder.AppendLine($"\t\tprivate readonly IHttpApiInterceptor {this.apiInterceptorFieldName};");
            builder.AppendLine($"\t\tprivate readonly ApiActionInvoker[] {this.actionInvokersFieldName};");

            builder.AppendLine($"\t\tpublic {this.CtorName}(IHttpApiInterceptor apiInterceptor,ApiActionInvoker[] actionInvokers)");
            builder.AppendLine("\t\t{");
            builder.AppendLine($"\t\t\tthis.{this.apiInterceptorFieldName} = apiInterceptor;");
            builder.AppendLine($"\t\t\tthis.{this.actionInvokersFieldName} = actionInvokers;");
            builder.AppendLine("\t\t}");

            var index = 0;
            foreach (var method in FindApiMethods(this.httpApi))
            {
                var methodCode = this.BuildMethod(method, index);
                builder.AppendLine(methodCode);
                index += 1;
            }

            builder.AppendLine("\t}");
            builder.AppendLine("}");

            // System.Diagnostics.Debugger.Launch();
            return builder.ToString();
        }

        /// <summary>
        /// 查找接口类型及其继承的接口的所有方法
        /// </summary>
        /// <param name="httpApi">接口</param>
        /// <returns></returns>
        private static IEnumerable<IMethodSymbol> FindApiMethods(INamedTypeSymbol httpApi)
        {
            return httpApi
                .AllInterfaces
                .Append(httpApi)
                .SelectMany(item => item.GetMembers())
                .OfType<IMethodSymbol>();
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
            var parametersString = string.Join(",", method.Parameters.Select(item => $"{item.Type} {item.Name}"));
            var parameterNamesString = string.Join(",", method.Parameters.Select(item => item.Name));

            builder.AppendLine($"\t\t[HttpApiProxyMethod({index})]");
            builder.AppendLine($"\t\tpublic {method.ReturnType} {method.Name}( {parametersString} )");
            builder.AppendLine("\t\t{");
            builder.AppendLine($"\t\t\treturn ({method.ReturnType})this.{this.apiInterceptorFieldName}.Intercept(this.{this.actionInvokersFieldName}[{index}], new object[] {{ {parameterNamesString} }});");
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
            return this.HttpApiTypeName == other.HttpApiTypeName;
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
            return this.HttpApiTypeName.GetHashCode();
        }
    }
}
