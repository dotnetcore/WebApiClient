using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace WebApiClientCore.Analyzers
{
    /// <summary>
    /// 表示HttpApi上下文
    /// </summary>
    class HttpApiContext
    {
        /// <summary>
        /// IHttpApi的类型名称
        /// </summary>
        private const string ihttpApiTypeName = "WebApiClientCore.IHttpApi";

        /// <summary>
        /// UriAttribue的类型名称
        /// </summary>
        private const string uriAttributeTypeName = "WebApiClientCore.Attributes.UriAttribute";

        /// <summary>
        /// AttributeCtorUsageAttribute的类型名称
        /// </summary>
        private const string attributeCtorUsageTypName = "WebApiClientCore.AttributeCtorUsageAttribute";



        /// <summary>
        /// 获取语法节点上下文
        /// </summary>
        public SyntaxNodeAnalysisContext SyntaxNodeContext { get; }

        /// <summary>
        /// 获取接口声明语法
        /// </summary>
        public InterfaceDeclarationSyntax HttpApiSyntax { get; }

        /// <summary>
        /// 获取是否为HttpApi
        /// </summary>
        public bool IsHtttApi { get; }

        /// <summary>
        /// 获取IHttpApi的类型
        /// </summary>
        public INamedTypeSymbol IHttpApiType { get; }

        /// <summary>
        /// 获取UriAttribute的类型
        /// </summary>
        public INamedTypeSymbol UriAttributeType { get; }

        /// <summary>
        /// 获取AttributeCtorUsageAttribute的类型
        /// </summary>
        public INamedTypeSymbol AttributeCtorUsageAttributeType { get; }


        /// <summary>
        /// HttpApi上下文
        /// </summary>
        /// <param name="syntaxNodeContext"></param>
        public HttpApiContext(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            this.SyntaxNodeContext = syntaxNodeContext;
            this.HttpApiSyntax = syntaxNodeContext.Node as InterfaceDeclarationSyntax;

            this.IHttpApiType = syntaxNodeContext.Compilation.GetTypeByMetadataName(ihttpApiTypeName);
            this.UriAttributeType = syntaxNodeContext.Compilation.GetTypeByMetadataName(uriAttributeTypeName);
            this.AttributeCtorUsageAttributeType = syntaxNodeContext.Compilation.GetTypeByMetadataName(attributeCtorUsageTypName);

            this.IsHtttApi = this.IsHttpApiInterface();
        }


        /// <summary>
        /// 返回是否为HttpApi接口
        /// </summary>
        /// <returns></returns>
        private bool IsHttpApiInterface()
        {
            if (HttpApiSyntax?.BaseList == null)
            {
                return false;
            }

            foreach (var baseType in this.HttpApiSyntax.BaseList.Types)
            {
                var type = this.SyntaxNodeContext.SemanticModel.GetTypeInfo(baseType.Type).Type;
                if (type.Equals(this.IHttpApiType) == true)
                {
                    return true;
                }

                if (type.AllInterfaces.Any(item => item.Equals(this.IHttpApiType)) == true)
                {
                    return true;
                }
            }

            return false;
        }
    }
}