using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// Represents the HttpApi context
    /// </summary>
    class HttpApiContext
    {
        /// <summary>
        /// IHttpApi's type name
        /// </summary>
        private const string ihttpApiTypeName = "WebApiClient.IHttpApi";

        /// <summary>
        /// UriAttribue's type name
        /// </summary>
        private const string uriAttributeTypeName = "WebApiClient.Attributes.UriAttribute";

        /// <summary>
        /// Type name of AttributeCtorUsageAttribute
        /// </summary>
        private const string attributeCtorUsageTypName = "WebApiClient.Attributes.AttributeCtorUsageAttribute";



        /// <summary>
        /// Get syntax node context
        /// </summary>
        public SyntaxNodeAnalysisContext SyntaxNodeContext { get; }

        /// <summary>
        /// Get interface declaration syntax
        /// </summary>
        public InterfaceDeclarationSyntax HttpApiSyntax { get; }

        /// <summary>
        /// Gets whether it is HttpApi
        /// </summary>
        public bool IsHtttApi { get; }

        /// <summary>
        /// Get the type of IHttpApi
        /// </summary>
        public INamedTypeSymbol IHttpApiType { get; }

        /// <summary>
        /// Get the type of UriAttribute
        /// </summary>
        public INamedTypeSymbol UriAttributeType { get; }

        /// <summary>
        /// Get the type of AttributeCtorUsageAttribute
        /// </summary>
        public INamedTypeSymbol AttributeCtorUsageAttributeType { get; }


        /// <summary>
        /// HttpApi context
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
        /// Returns whether it is an HttpApi interface
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