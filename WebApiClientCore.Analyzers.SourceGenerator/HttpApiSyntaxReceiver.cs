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
        private const string IApiActionAttributeTypeName = "WebApiClientCore.IApiActionAttribute";

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
            var ihttpApi = compilation.GetTypeByMetadataName(IHttpApiTypeName);
            if (ihttpApi == null)
            {
                yield break;
            }

            var iapiActionAttribute = compilation.GetTypeByMetadataName(IApiActionAttributeTypeName);
            foreach (var interfaceSyntax in this.interfaceSyntaxList)
            {
                var @interface = compilation.GetSemanticModel(interfaceSyntax.SyntaxTree).GetDeclaredSymbol(interfaceSyntax);
                if (@interface != null && IsHttpApiInterface(@interface, ihttpApi, iapiActionAttribute))
                {
                    yield return @interface;
                }
            }
        }

        /// <summary>
        /// 是否为http接口
        /// </summary>
        /// <param name="interface"></param>
        /// <param name="ihttpApi"></param>
        /// <param name="iapiActionAttribute"></param>
        /// <returns></returns>
        private static bool IsHttpApiInterface(INamedTypeSymbol @interface, INamedTypeSymbol ihttpApi, INamedTypeSymbol? iapiActionAttribute)
        {
            if (@interface.AllInterfaces.Contains(ihttpApi))
            {
                return true;
            }

            if (iapiActionAttribute == null)
            {
                return false;
            }

            var interfaces = @interface.AllInterfaces.Append(@interface);
            return interfaces.Any(i => HasApiActionAttribute(i, iapiActionAttribute));
        }


        /// <summary>
        /// 返回接口和其声明的方法是否包含IApiActionAttribute
        /// </summary>
        /// <param name="interface"></param>
        /// <param name="iapiActionAttribute"></param>
        /// <returns></returns>
        private static bool HasApiActionAttribute(INamedTypeSymbol @interface, INamedTypeSymbol iapiActionAttribute)
        {
            return HasAttribute(@interface, iapiActionAttribute)
                || @interface.GetMembers().Any(m => HasAttribute(m, iapiActionAttribute));
        }

        /// <summary>
        /// 返回成员是否有特性
        /// </summary>
        /// <param name="member"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private static bool HasAttribute(ISymbol member, INamedTypeSymbol attribute)
        {
            foreach (var attr in member.GetAttributes())
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