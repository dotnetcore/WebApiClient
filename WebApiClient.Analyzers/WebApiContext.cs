using Microsoft.CodeAnalysis;
using System;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// 表示WebApiClient的上下文
    /// </summary>
    class WebApiContext
    {
        /// <summary>
        /// IHttpApi的类型
        /// </summary>
        private readonly Lazy<INamedTypeSymbol> ihttpApiType;

        /// <summary>
        /// AttributeCtorUsageAtribute的类型
        /// </summary>
        private readonly Lazy<INamedTypeSymbol> attributeCtorUsageAtributeType;

        /// <summary>
        /// IHttpApi的类型名称
        /// </summary>
        private const string ihttpApiTypeName = "WebApiClient.IHttpApi";

        /// <summary>
        /// AttributeCtorUsageAtribute的类型名称
        /// </summary>
        private const string attributeCtorUsageTypName = "WebApiClient.Attributes.AttributeCtorUsageAttribute";

        /// <summary>
        /// 获取IHttpApi的类型
        /// </summary>
        public INamedTypeSymbol IHttpApiType
        {
            get => this.ihttpApiType.Value;
        }

        /// <summary>
        /// 获取AttributeCtorUsageAtribute的类型
        /// </summary>
        public INamedTypeSymbol AttributeCtorUsageAtributeType
        {
            get => this.attributeCtorUsageAtributeType.Value;
        }

        /// <summary>
        /// WebApiClient的上下文
        /// </summary>
        /// <param name="compilation"></param>
        public WebApiContext(Compilation compilation)
        {
            this.ihttpApiType = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName(ihttpApiTypeName), true);
            this.attributeCtorUsageAtributeType = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName(attributeCtorUsageTypName), true);
        }
    }
}
