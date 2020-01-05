# if NETCOREAPP2_1

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace WebApiClient
{
    /// <summary>
    /// 提供项目相关扩展
    /// </summary>
    public static partial class Extension
    {
        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi<THttpApi>(this IServiceCollection services) where THttpApi : class, IHttpApi
        {
            return services.AddHttpApi<THttpApi>((o, s) => { });
        }

        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions<THttpApi>> configureOptions) where THttpApi : class, IHttpApi
        {
            return services.AddHttpApi<THttpApi>((o, s) => configureOptions(o));
        }

        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions<THttpApi>, IServiceProvider> configureOptions) where THttpApi : class, IHttpApi
        {
            services
                .AddOptions<HttpApiOptions<THttpApi>>()
                .Configure(configureOptions);

            return services
                .AddHttpClient(typeof(THttpApi).FullName)
                .AddTypedClient((httpClient, serviceProvider) =>
                {
                    var httpApiConfig = new HttpApiConfig(httpClient)
                    {
                        ServiceProvider = serviceProvider
                    };
                    var httpApiOptions = serviceProvider.GetRequiredService<IOptions<HttpApiOptions<THttpApi>>>().Value;
                    MergeOptions(httpApiConfig, httpApiOptions);

                    return HttpApi.Create<THttpApi>(httpApiConfig);
                })
                .ConfigurePrimaryHttpMessageHandler(() => new DefaultHttpClientHandler());
        }

        /// <summary>
        /// 合并选项到配置
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="httpApiConfig"></param>
        /// <param name="httpApiOptions"></param>
        private static void MergeOptions<THttpApi>(HttpApiConfig httpApiConfig, HttpApiOptions<THttpApi> httpApiOptions) where THttpApi : class, IHttpApi
        {
            if (httpApiOptions.UseParameterPropertyValidate != null)
            {
                httpApiConfig.UseParameterPropertyValidate = httpApiOptions.UseParameterPropertyValidate.Value;
            }
            if (httpApiOptions.UseReturnValuePropertyValidate != null)
            {
                httpApiConfig.UseReturnValuePropertyValidate = httpApiOptions.UseReturnValuePropertyValidate.Value;
            }
            if (httpApiOptions.FormatOptions != null)
            {
                httpApiConfig.FormatOptions = httpApiOptions.FormatOptions;
            }
            if (httpApiOptions.HttpHost != null)
            {
                httpApiConfig.HttpHost = httpApiOptions.HttpHost;
            }
            if (httpApiOptions.JsonFormatter != null)
            {
                httpApiConfig.JsonFormatter = httpApiOptions.JsonFormatter;
            }
            if (httpApiOptions.KeyValueFormatter != null)
            {
                httpApiConfig.KeyValueFormatter = httpApiOptions.KeyValueFormatter;
            }
            if (httpApiOptions.XmlFormatter != null)
            {
                httpApiConfig.XmlFormatter = httpApiOptions.XmlFormatter;
            }
            if (httpApiOptions.ResponseCacheProvider != null)
            {
                httpApiConfig.ResponseCacheProvider = httpApiOptions.ResponseCacheProvider;
            }
        }

        /// <summary>
        /// 配置HttpApi
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions<THttpApi>> configureOptions) where THttpApi : class, IHttpApi
        {
            return services.Configure(configureOptions);
        }

        /// <summary>
        /// 配置HttpApi
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi<THttpApi>(this IServiceCollection services, IConfiguration configureOptions) where THttpApi : class, IHttpApi
        {
            return services.Configure<HttpApiOptions<THttpApi>>(configureOptions);
        }
    }
}

#endif