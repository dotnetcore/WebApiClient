using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApi创建工厂
    /// 提供HttpApi的配置注册和实例创建
    /// 并对实例的生命周期进行自动管理
    /// </summary>
    public partial class HttpApiFactory : IHttpApiFactory
    {
        /// <summary>
        /// 具有生命周期的httpHandler延时创建对象
        /// </summary>
        private Lazy<LifetimeHttpHandler> lifeTimeHttpHandlerLazy;

        /// <summary>
        /// HttpHandler清理器
        /// </summary>
        private readonly LifetimeHttpHandlerCleaner httpHandlerCleaner = new LifetimeHttpHandlerCleaner();



        /// <summary>
        /// cookie容器
        /// </summary>
        private CookieContainer cookieContainer;

        /// <summary>
        /// 是否保持cookie容器
        /// </summary>
        private bool keepCookieContainer = HttpHandlerProvider.IsSupported;

        /// <summary>
        /// 生命周期
        /// </summary>
        private TimeSpan lifeTime = TimeSpan.FromMinutes(2d);

        /// <summary>
        /// HttpApiConfig的配置委托
        /// </summary>
        private Action<HttpApiConfig> configOptions;

        /// <summary>
        /// HttpMessageHandler的创建委托
        /// </summary>
        private Func<HttpMessageHandler> handlerFactory;

        /// <summary>
        /// 获取接口类型
        /// </summary>
        protected Type InterfaceType { get; private set; }

        /// <summary>
        /// HttpApi创建工厂
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <param name="interfaceType">接口类型</param>
        public HttpApiFactory(Type interfaceType)
        {
            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            if (interfaceType.IsInheritFrom<IHttpApi>() == false)
            {
                throw new ArgumentException($"接口类型必须继承{nameof(IHttpApi)}", nameof(interfaceType));
            }

            this.InterfaceType = interfaceType;
            this.lifeTimeHttpHandlerLazy = new Lazy<LifetimeHttpHandler>(this.CreateHttpHandler, true);
        }

        /// <summary>
        /// 创建LifetimeHttpHandler
        /// </summary>
        /// <returns></returns>
        private LifetimeHttpHandler CreateHttpHandler()
        {
            var handler = this.handlerFactory?.Invoke() ?? new DefaultHttpClientHandler();
            return new LifetimeHttpHandler(handler, this.lifeTime, this.OnHttpHandlerDeactivate);
        }

        /// <summary>
        /// 当有httpHandler失效时
        /// </summary>
        /// <param name="handler">httpHandler</param>
        private void OnHttpHandlerDeactivate(LifetimeHttpHandler handler)
        {
            // 切换激活状态的记录的实例
            this.lifeTimeHttpHandlerLazy = new Lazy<LifetimeHttpHandler>(this.CreateHttpHandler, true);
            this.httpHandlerCleaner.Add(handler);
        }

        /// <summary>
        /// 设置HttpApi实例的生命周期
        /// </summary>
        /// <param name="lifeTime">生命周期</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public HttpApiFactory SetLifetime(TimeSpan lifeTime)
        {
            if (lifeTime <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.lifeTime = lifeTime;
            return this;
        }

        /// <summary>
        /// 设置清理过期的HttpApi实例的时间间隔
        /// </summary>
        /// <param name="interval">时间间隔</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public HttpApiFactory SetCleanupInterval(TimeSpan interval)
        {
            if (interval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.httpHandlerCleaner.CleanupInterval = interval;
            return this;
        }

        /// <summary>
        /// 设置是否维护使用一个CookieContainer实例
        /// 该实例为首次创建时的CookieContainer
        /// </summary>
        /// <param name="keep">true维护使用一个CookieContainer实例</param>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <returns></returns>
        public HttpApiFactory SetKeepCookieContainer(bool keep)
        {
            if (keep == true && HttpHandlerProvider.IsSupported == false)
            {
                var message = $"无法设置KeepCookieContainer，请在{nameof(ConfigureHttpMessageHandler)}为Handler设置固定的{nameof(CookieContainer)}";
                throw new PlatformNotSupportedException(message);
            }

            this.keepCookieContainer = keep;
            return this;
        }

        /// <summary>
        /// 配置HttpMessageHandler的创建
        /// </summary>
        /// <param name="factory">创建委托</param>
        /// <returns></returns>
        public HttpApiFactory ConfigureHttpMessageHandler(Func<HttpMessageHandler> factory)
        {
            this.handlerFactory = factory;
            return this;
        }

        /// <summary>
        /// 配置HttpApiConfig
        /// </summary>
        /// <param name="options">配置委托</param>
        /// <returns></returns>
        public HttpApiFactory ConfigureHttpApiConfig(Action<HttpApiConfig> options)
        {
            this.configOptions = options;
            return this;
        }

        /// <summary>
        /// 创建TInterface接口的代理实例
        /// </summary>
        /// <param name="httpApiConfig">httpApi配置</param>
        /// <returns></returns>
        protected virtual HttpApiClient CreateHttpApi(HttpApiConfig httpApiConfig)
        {
            return HttpApiClient.Create(this.InterfaceType, httpApiConfig);
        }


        #region 接口显式实现
        /// <summary>
        /// 配置HttpMessageHandler的创建
        /// </summary>
        /// <param name="factory">创建委托</param>
        void IHttpApiFactory.ConfigureHttpMessageHandler(Func<HttpMessageHandler> factory)
        {
            this.ConfigureHttpMessageHandler(factory);
        }

        /// <summary>
        /// 配置HttpApiConfig
        /// </summary>
        /// <param name="options">配置委托</param>
        void IHttpApiFactory.ConfigureHttpApiConfig(Action<HttpApiConfig> options)
        {
            this.ConfigureHttpApiConfig(options);
        }

        /// <summary>
        /// 创建接口的代理实例
        /// </summary>
        /// <returns></returns>
        HttpApiClient IHttpApiFactory.CreateHttpApi()
        {
            var handler = this.lifeTimeHttpHandlerLazy.Value;
            var httpApiConfig = new LifetimeHttpApiConfig(handler);

            if (this.configOptions != null)
            {
                this.configOptions.Invoke(httpApiConfig);
            }

            if (this.keepCookieContainer == true)
            {
                Interlocked.CompareExchange(ref this.cookieContainer, httpApiConfig.HttpHandler.CookieContainer, null);
                if (object.ReferenceEquals(httpApiConfig.HttpHandler.CookieContainer, this.cookieContainer) == false)
                {
                    httpApiConfig.HttpHandler.CookieContainer = this.cookieContainer;
                }
            }

            return this.CreateHttpApi(httpApiConfig);
        }
        #endregion
    }

    /// <summary>
    /// 表示HttpApi创建工厂
    /// 提供HttpApi的配置注册和实例创建
    /// 并对实例的生命周期进行自动管理
    /// </summary>
    public class HttpApiFactory<TInterface> : HttpApiFactory, IHttpApiFactory<TInterface>
        where TInterface : class, IHttpApi
    {
        /// <summary>
        /// HttpApi创建工厂
        /// </summary>
        public HttpApiFactory()
            : base(typeof(TInterface))
        {
        }

        /// <summary>
        /// 创建HttpApi代理实例
        /// </summary>
        /// <returns></returns>
        public TInterface CreateHttpApi()
        {
            return ((IHttpApiFactory)this).CreateHttpApi() as TInterface;
        }


        #region new覆盖
        /// <summary>
        /// 设置HttpApi实例的生命周期
        /// </summary>
        /// <param name="lifeTime">生命周期</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public new HttpApiFactory<TInterface> SetLifetime(TimeSpan lifeTime)
        {
            return base.SetLifetime(lifeTime) as HttpApiFactory<TInterface>;
        }

        /// <summary>
        /// 设置清理过期的HttpApi实例的时间间隔
        /// </summary>
        /// <param name="interval">时间间隔</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public new HttpApiFactory<TInterface> SetCleanupInterval(TimeSpan interval)
        {
            return base.SetCleanupInterval(interval) as HttpApiFactory<TInterface>;
        }

        /// <summary>
        /// 设置是否维护使用一个CookieContainer实例
        /// 该实例为首次创建时的CookieContainer
        /// </summary>
        /// <param name="keep">true维护使用一个CookieContainer实例</param>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <returns></returns>
        public new HttpApiFactory<TInterface> SetKeepCookieContainer(bool keep)
        {
            return base.SetKeepCookieContainer(keep) as HttpApiFactory<TInterface>;
        }

        /// <summary>
        /// 配置HttpMessageHandler的创建
        /// </summary>
        /// <param name="factory">创建委托</param>
        /// <returns></returns>
        public new HttpApiFactory<TInterface> ConfigureHttpMessageHandler(Func<HttpMessageHandler> factory)
        {
            return base.ConfigureHttpMessageHandler(factory) as HttpApiFactory<TInterface>;
        }

        /// <summary>
        /// 配置HttpApiConfig
        /// </summary>
        /// <param name="options">配置委托</param>
        /// <returns></returns>
        public new HttpApiFactory<TInterface> ConfigureHttpApiConfig(Action<HttpApiConfig> options)
        {
            return base.ConfigureHttpApiConfig(options) as HttpApiFactory<TInterface>;
        }
        #endregion
    }
}