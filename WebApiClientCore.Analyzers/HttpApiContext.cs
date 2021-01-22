using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore.Analyzers
{
    /// <summary>
    /// 表示HttpApi上下文
    /// </summary>
    sealed class HttpApiContext
    {
        /// <summary>
        /// IHttpApi的类型名称
        /// </summary>
        private const string IHttpApiTypeName = "WebApiClientCore.IHttpApi";

        /// <summary>
        /// UriAttribue的类型名称
        /// </summary>
        private const string UriAttributeTypeName = "WebApiClientCore.Attributes.UriAttribute";

        /// <summary>
        /// AttributeCtorUsageAttribute的类型名称
        /// </summary>
        private const string AttributeCtorUsageTypName = "WebApiClientCore.AttributeCtorUsageAttribute";



        /// <summary>
        /// 获取语法节点上下文
        /// </summary>
        public SyntaxNodeAnalysisContext SyntaxNodeContext { get; }

        /// <summary>
        /// 获取接口声明语法
        /// </summary>
        public InterfaceDeclarationSyntax? InterfaceSyntax { get; }

        /// <summary>
        /// 获取接
        /// </summary>
        public INamedTypeSymbol? @Interface { get; }

        /// <summary>
        /// 获取是否为HttpApi
        /// </summary>
        public bool IsHtttApi { get; }

        /// <summary>
        /// 获取IHttpApi的类型
        /// </summary>
        public INamedTypeSymbol? IHttpApi { get; }

        /// <summary>
        /// 获取UriAttribute的类型
        /// </summary>
        public INamedTypeSymbol? UriAttribute { get; }

        /// <summary>
        /// 获取AttributeCtorUsageAttribute的类型
        /// </summary>
        public INamedTypeSymbol? AttributeCtorUsageAttribute { get; }

        /// <summary>
        /// 获取声明的Api方法
        /// </summary>
        public IReadOnlyList<IMethodSymbol> ApiMethods { get; }

        /// <summary>
        /// HttpApi上下文
        /// </summary>
        /// <param name="syntaxNodeContext"></param>
        public HttpApiContext(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            this.SyntaxNodeContext = syntaxNodeContext;
            this.InterfaceSyntax = syntaxNodeContext.Node as InterfaceDeclarationSyntax;

            this.IHttpApi = syntaxNodeContext.Compilation.GetTypeByMetadataName(IHttpApiTypeName);
            this.UriAttribute = syntaxNodeContext.Compilation.GetTypeByMetadataName(UriAttributeTypeName);
            this.AttributeCtorUsageAttribute = syntaxNodeContext.Compilation.GetTypeByMetadataName(AttributeCtorUsageTypName);
           
            if (this.InterfaceSyntax != null)
            {
                this.Interface = syntaxNodeContext.Compilation.GetSemanticModel(this.InterfaceSyntax.SyntaxTree).GetDeclaredSymbol(this.InterfaceSyntax);
            }

            if (this.Interface != null)
            {
                if (this.IHttpApi != null)
                {
                    this.IsHtttApi = this.Interface.AllInterfaces.Contains(this.IHttpApi);
                }
                this.ApiMethods = this.Interface.GetMembers().OfType<IMethodSymbol>().ToList().AsReadOnly();
            }
            else
            {
                this.ApiMethods = new List<IMethodSymbol>().AsReadOnly();
            }
        }
    }
}