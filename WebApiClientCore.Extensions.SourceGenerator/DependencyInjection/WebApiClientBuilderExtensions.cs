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
        /// <para>• 要求Visual Studio或msbuild工具为最新版本才支持SourceGenerator技术</para>
        /// <para>• 要求声明http接口的项目必须引用WebApiClientCore.Extensions.SourceGenerator包</para>
        /// <para>• 目前SourceGenerator技术不支持partial分散在多个cs文件的单个http接口</para>
        /// <para>• 目前SourceGenerator技术只支持c#语言</para>
        /// <para>• 使用SourceGenerator相比Emit创建代理类并不能带来性能提升</para>
        /// </remarks>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebApiClientBuilder UseSourceGeneratorHttpApiActivator(this IWebApiClientBuilder builder)
        {
            builder.Services.AddSingleton(typeof(IHttpApiActivator<>), typeof(SourceGeneratorHttpApiActivator<>));
            return builder;
        }
    }
}
