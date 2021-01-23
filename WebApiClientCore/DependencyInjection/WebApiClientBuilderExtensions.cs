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
        /// 添加WebApiClient全局默认配置
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IWebApiClientBuilder AddWebApiClient(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddMemoryCache();

            services.TryAddSingleton(typeof(IHttpApiActivator<>), typeof(DefaultHttpApiActivator<>));
            services.TryAddSingleton<IApiActionDescriptorProvider, DefaultApiActionDescriptorProvider>();
            services.TryAddSingleton<IApiActionInvokerProvider, DefaultApiActionInvokerProvider>();
            services.TryAddSingleton<IResponseCacheProvider, DefaultResponseCacheProvider>();

            return new WebApiClientBuilder(services);
        }

        /// <summary>
        /// 当非GET或HEAD请求的缺省参数特性声明时
        /// 为复杂参数类型的参数应用JsonContentAttribute
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebApiClientBuilder UseJsonFirstApiActionDescriptor(this IWebApiClientBuilder builder)
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
