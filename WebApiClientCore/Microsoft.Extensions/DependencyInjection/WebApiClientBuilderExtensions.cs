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
        /// 创建WebApiClient全局配置的Builder
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IWebApiClientBuilder AddWebApiClient(this IServiceCollection services)
        {
            return new WebApiClientBuilder(services);
        }

        /// <summary>
        /// 使用Emit在运行时动态创建接口的代理类
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebApiClientBuilder AddHttpApiEmitActivator(this IWebApiClientBuilder builder)
        {
            builder.Services.AddSingleton(typeof(IHttpApiActivator<>), typeof(HttpApiEmitActivator<>));
            return builder;
        }

        /// <summary>
        /// 当非GET或HEAD请求的缺省参数特性声明时
        /// 为复杂参数类型的参数应用JsonContentAttribute
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebApiClientBuilder AddJsonFirstApiActionDescriptorProvider(this IWebApiClientBuilder builder)
        {
            builder.Services.AddSingleton<IApiActionDescriptorProvider, JsonFirstApiActionDescriptorProvider>();
            return builder;
        }

        /// <summary>
        /// WebApiClient全局配置的Builder
        /// </summary>
        private class WebApiClientBuilder : IWebApiClientBuilder
        {
            /// <summary>
            /// 获取服务集合
            /// </summary>
            public IServiceCollection Services { get; }

            public WebApiClientBuilder(IServiceCollection services)
            {
                this.Services = services;
            }
        }
    }
}
