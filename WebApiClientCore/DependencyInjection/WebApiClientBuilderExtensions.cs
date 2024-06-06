using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <remarks>
        /// <para>• 尝试使用<see cref="DefaultHttpApiActivator{T}"/>，注册为<see cref="IHttpApiActivator{T}"/></para>
        /// <para>• 尝试使用<see cref="DefaultApiActionDescriptorProvider"/>，注册为<see cref="IApiActionDescriptorProvider"/></para>
        /// <para>• 尝试使用<see cref="DefaultResponseCacheProvider"/>，注册为<see cref="IResponseCacheProvider"/></para>
        /// <para>• 尝试使用<see cref="DefaultApiActionInvokerProvider"/>，注册为<see cref="IApiActionInvokerProvider"/></para>
        /// </remarks> 
        /// <param name="services"></param>
        /// <returns></returns>
        public static IWebApiClientBuilder AddWebApiClient(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddMemoryCache();
            services.TryAddTransient<IOptionsFactory<HttpApiOptions>, HttpApiOptionsFactory>();

            services.TryAddSingleton(typeof(IHttpApiActivator<>), typeof(DefaultHttpApiActivator<>));
            services.TryAddSingleton<IApiActionDescriptorProvider, DefaultApiActionDescriptorProvider>();
            services.TryAddSingleton<IApiActionInvokerProvider, DefaultApiActionInvokerProvider>();
            services.TryAddSingleton<IResponseCacheProvider, DefaultResponseCacheProvider>();

            return new WebApiClientBuilder(services);
        }


        /// <summary>
        /// 使用 <see cref="ILEmitHttpApiActivator{T}"/> 替换 <see cref="IHttpApiActivator{T}"/> 的实现
        /// </summary>
        /// <param name="builder">IWebApiClientBuilder 实例</param>
        /// <returns>返回 IWebApiClientBuilder 实例</returns>
        public static IWebApiClientBuilder UseILEmitHttpApiActivator(this IWebApiClientBuilder builder)
        {
            builder.Services.RemoveAll(typeof(IHttpApiActivator<>)).AddSingleton(typeof(IHttpApiActivator<>), typeof(ILEmitHttpApiActivator<>));
            return builder;
        }

        /// <summary>
        /// 使用 <see cref="SourceGeneratorHttpApiActivator{T}"/> 替换 <see cref="IHttpApiActivator{T}"/> 的实现
        /// </summary> 
        /// <param name="builder"></param>
        /// <returns></returns> 
        public static IWebApiClientBuilder UseSourceGeneratorHttpApiActivator(this IWebApiClientBuilder builder)
        {
            builder.Services.RemoveAll(typeof(IHttpApiActivator<>)).AddSingleton(typeof(IHttpApiActivator<>), typeof(SourceGeneratorHttpApiActivator<>));
            return builder;
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
        /// 配置<see cref="HttpApiOptions"/>的默认值
        /// </summary>
        /// <param name="builder"></param>  
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IWebApiClientBuilder ConfigureHttpApi(this IWebApiClientBuilder builder, Action<HttpApiOptions, IServiceProvider> configureOptions)
        {
            builder.Services.AddOptions<HttpApiOptions>().Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// 配置<see cref="HttpApiOptions"/>的默认值
        /// </summary>
        /// <param name="builder"></param>  
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IWebApiClientBuilder ConfigureHttpApi(this IWebApiClientBuilder builder, Action<HttpApiOptions> configureOptions)
        {
            builder.Services.AddOptions<HttpApiOptions>().Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// 配置<see cref="HttpApiOptions"/>的默认值
        /// </summary>
        /// <param name="builder"></param>  
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static IWebApiClientBuilder ConfigureHttpApi(this IWebApiClientBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddOptions<HttpApiOptions>().Bind(configuration);
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

        /// <summary>
        /// HttpApiOptions工厂
        /// </summary>
        private class HttpApiOptionsFactory : IOptionsFactory<HttpApiOptions>
        {
            private readonly IConfigureOptions<HttpApiOptions>[] _setups;
            private readonly IPostConfigureOptions<HttpApiOptions>[] _postConfigures;
            private readonly IValidateOptions<HttpApiOptions>[] _validations;

            /// <summary>
            /// HttpApiOptions工厂
            /// </summary>
            /// <param name="setups"></param>
            /// <param name="postConfigures"></param>
            /// <param name="validations"></param>
            public HttpApiOptionsFactory(IEnumerable<IConfigureOptions<HttpApiOptions>> setups, IEnumerable<IPostConfigureOptions<HttpApiOptions>> postConfigures, IEnumerable<IValidateOptions<HttpApiOptions>> validations)
            {
                _setups = setups as IConfigureOptions<HttpApiOptions>[] ?? setups.ToArray();
                _postConfigures = postConfigures as IPostConfigureOptions<HttpApiOptions>[] ?? postConfigures.ToArray();
                _validations = validations as IValidateOptions<HttpApiOptions>[] ?? validations.ToArray();
            }

            /// <summary>
            /// 创建Options
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public HttpApiOptions Create(string name)
            {
                var defaultOptions = this.Create(Options.Options.DefaultName, default);
                return this.Create(name, defaultOptions);
            }

            /// <summary>
            /// 创建Options
            /// </summary>
            /// <param name="name"></param>
            /// <param name="options">传入的实例</param>
            /// <returns></returns>
            private HttpApiOptions Create(string name, HttpApiOptions? options)
            {
                if (options == null)
                {
                    options = new HttpApiOptions();
                }

                foreach (var setup in _setups)
                {
                    if (setup is IConfigureNamedOptions<HttpApiOptions> namedSetup)
                    {
                        namedSetup.Configure(name, options);
                    }
                    else if (name == Options.Options.DefaultName)
                    {
                        setup.Configure(options);
                    }
                }

                foreach (var post in _postConfigures)
                {
                    post.PostConfigure(name, options);
                }

                if (_validations != null)
                {
                    var failures = new List<string>();
                    foreach (var validate in _validations)
                    {
                        var result = validate.Validate(name, options);
                        if (result != null && result.Failed)
                        {
                            failures.AddRange(result.Failures);
                        }
                    }
                    if (failures.Count > 0)
                    {
                        throw new OptionsValidationException(name, typeof(HttpApiOptions), failures);
                    }
                }

                return options;
            }
        }
    }
}
