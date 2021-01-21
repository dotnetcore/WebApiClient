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
        /// 编译时生成接口的代理类型代码
        /// 运行时通过查找接口的代理类型并创建实例
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebApiClientBuilder AddHttpApiSourceGeneratorActivator(this IWebApiClientBuilder builder)
        {
            builder.Services.AddSingleton(typeof(IHttpApiActivator<>), typeof(HttpApiSourceGeneratorActivator<>));
            return builder;
        }
    }
}
