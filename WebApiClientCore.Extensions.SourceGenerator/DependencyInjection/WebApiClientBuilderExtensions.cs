using Microsoft.Extensions.DependencyInjection.Extensions;
using WebApiClientCore;
using WebApiClientCore.Implementations;

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
        /// <remarks>
        /// <para>• 要求声明http接口的项目的编程语言为c#</para>
        /// <para>• 要求声明http接口的项目引用WebApiClientCore.Extensions.SourceGenerator包</para>
        /// <para>• 要求Visual Studio或msbuild工具为最新版本</para>
        /// </remarks>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebApiClientBuilder UseSourceGeneratorHttpApiActivator(this IWebApiClientBuilder builder)
        {
            builder.Services.RemoveAll(typeof(IHttpApiActivator<>));
            builder.Services.AddSingleton(typeof(IHttpApiActivator<>), typeof(SourceGeneratorHttpApiActivator<>));
            return builder;
        }
    }
}
