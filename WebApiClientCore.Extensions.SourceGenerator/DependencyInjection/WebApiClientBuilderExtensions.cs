using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// IWebApiClientBuilder扩展
    /// </summary>
    public static class WebApiClientBuilderExtensions
    {
        /// <summary>
        /// 编译时使用SourceGenerator生成接口的代理类型代码
        /// 运行时查找接口的代理类型并创建实例
        /// </summary> 
        /// <param name="builder"></param>
        /// <returns></returns>
        [Obsolete("SourceGenerator功能已合并到基础包并默认启用")]
        public static IWebApiClientBuilder UseSourceGeneratorHttpApiActivator(this IWebApiClientBuilder builder)
        {
            return builder;
        }
    }
}
