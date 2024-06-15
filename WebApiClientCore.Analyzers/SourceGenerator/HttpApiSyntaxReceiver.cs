using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore.Analyzers.SourceGenerator
{
    /// <summary>
    /// httpApi语法接收器
    /// </summary>
    sealed class HttpApiSyntaxReceiver : ISyntaxReceiver
    {
        /// <summary>
        /// 接口标记名称
        /// </summary>
        private const string IHttpApiTypeName = "WebApiClientCore.IHttpApi";
        private const string IApiAttributeTypeName = "WebApiClientCore.IApiAttribute";

        /// <summary>
        /// 接口列表
        /// </summary>
        private readonly List<InterfaceDeclarationSyntax> interfaceSyntaxList = new List<InterfaceDeclarationSyntax>();

        /// <summary>
        /// 访问语法树 
        /// </summary>
        /// <param name="syntaxNode"></param>
        void ISyntaxReceiver.OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InterfaceDeclarationSyntax syntax)
            {
                this.interfaceSyntaxList.Add(syntax);
            }
        }

        /// <summary>
        /// 获取所有HttpApi符号
        /// </summary>
        /// <param name="compilation"></param>
        /// <returns></returns>
        public IEnumerable<INamedTypeSymbol> GetHttpApiTypes(Compilation compilation)
        {
            var httpApi = compilation.GetTypeByMetadataName(IHttpApiTypeName);
            if (httpApi == null)
            {
                yield break;
            }

            var apiAttribute = compilation.GetTypeByMetadataName(IApiAttributeTypeName);
            foreach (var interfaceSyntax in this.interfaceSyntaxList)
            {
                var @interface = compilation.GetSemanticModel(interfaceSyntax.SyntaxTree).GetDeclaredSymbol(interfaceSyntax);
                if (@interface != null && IsHttpApiInterface(@interface, httpApi, apiAttribute))
                {
                    yield return @interface;
                }
            }
        }


        /// <summary>
        /// 是否为 http 接口
        /// </summary>
        /// <param name="interface"></param>
        /// <param name="httpApi"></param>
        /// <param name="apiAttribute"></param>
        /// <returns></returns>
        private static bool IsHttpApiInterface(INamedTypeSymbol @interface, INamedTypeSymbol httpApi, INamedTypeSymbol? apiAttribute)
        {
            if (@interface.AllInterfaces.Contains(httpApi))
            {
                return true;
            }

            if (apiAttribute == null)
            {
                return false;
            }

            return @interface.AllInterfaces.Append(@interface).Any(i =>
                HasAttribute(i, apiAttribute) || i.GetMembers().OfType<IMethodSymbol>().Any(m =>
                HasAttribute(m, apiAttribute) || m.Parameters.Any(p => HasAttribute(p, apiAttribute))));
        }


        /// <summary>
        /// 返回是否声明指定的特性
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private static bool HasAttribute(ISymbol symbol, INamedTypeSymbol attribute)
        {
            foreach (var attr in symbol.GetAttributes())
            {
                var attrClass = attr.AttributeClass;
                if (attrClass != null && attrClass.AllInterfaces.Contains(attribute))
                {
                    return true;
                }
            }
            return false;
        }
    }
}