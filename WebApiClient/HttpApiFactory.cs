using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace WebApiClient
{
    /// <summary>
    /// Indicates that HttpApi creates a factory
    /// Provide HttpApi configuration registration and instance creation
    /// And automatically manage the life cycle of the instance
    /// </summary>
    public class HttpApiFactory : IHttpApiFactory
    {
        /// <summary>
        /// HttpHandler Delayed Creation Object with Life Cycle
        /// </summary>
        private Lazy<LifetimeHttpHandler> lifeTimeHttpHandlerLazy;

        /// <summary>
        /// HttpHandler cleaner
        /// </summary>
        private readonly LifetimeHttpHandlerCleaner httpHandlerCleaner = new LifetimeHttpHandlerCleaner();



        /// <summary>
        /// cookie container
        /// </summary>
        private CookieContainer cookieContainer;

        /// <summary>
        /// Whether to keep the cookie container
        /// </summary>
        private bool keepCookieContainer = HttpHandlerProvider.IsSupported;

        /// <summary>
        /// Life cycle
        /// </summary>
        private TimeSpan lifeTime = TimeSpan.FromMinutes(2d);

        /// <summary>
        /// HttpApiConfig configuration delegate
        /// </summary>
        private Action<HttpApiConfig> configOptions;

        /// <summary>
        /// HttpMessageHandler creation delegate
        /// </summary>
        private Func<HttpMessageHandler> handlerFactory;

        /// <summary>
        /// Get interface type
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        /// HttpApi create factory
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <param name="interfaceType">Interface Type</param>
        public HttpApiFactory(Type interfaceType)
        {
            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            if (interfaceType.IsInheritFrom<IHttpApi>() == false)
            {
                throw new ArgumentException($"Interface types must inherit {nameof(IHttpApi)}", nameof(interfaceType));
            }

            this.InterfaceType = interfaceType;
            this.lifeTimeHttpHandlerLazy = new Lazy<LifetimeHttpHandler>(this.CreateHttpHandler, true);
        }

        /// <summary>
        /// Create LifetimeHttpHandler
        /// </summary>
        /// <returns></returns>
        private LifetimeHttpHandler CreateHttpHandler()
        {
            var handler = this.handlerFactory?.Invoke() ?? new DefaultHttpClientHandler();
            return new LifetimeHttpHandler(handler, this.lifeTime, this.OnHttpHandlerDeactivate);
        }

        /// <summary>
        /// When httpHandler fails
        /// </summary>
        /// <param name="handler">httpHandler</param>
        private void OnHttpHandlerDeactivate(LifetimeHttpHandler handler)
        {
            // Example of a record that toggles the active state
            this.lifeTimeHttpHandlerLazy = new Lazy<LifetimeHttpHandler>(this.CreateHttpHandler, true);
            this.httpHandlerCleaner.Add(handler);
        }

        /// <summary>
        /// Setting the life cycle of an HttpApi instance
        /// </summary>
        /// <param name="lifeTime">Life cycle</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public HttpApiFactory SetLifetime(TimeSpan lifeTime)
        {
            if (lifeTime <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(lifeTime));
            }
            this.lifeTime = lifeTime;
            return this;
        }

        /// <summary>
        /// Set the interval to clean up expired HttpApi instances
        /// </summary>
        /// <param name="interval">time interval</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public HttpApiFactory SetCleanupInterval(TimeSpan interval)
        {
            if (interval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(interval));
            }
            this.httpHandlerCleaner.CleanupInterval = interval;
            return this;
        }

        /// <summary>
        /// Set whether to maintain the use of a CookieContainer instance
        /// This instance is the CookieContainer when first created
        /// </summary>
        /// <param name="keep">true maintenance uses a CookieContainer instance</param>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <returns></returns>
        public HttpApiFactory SetKeepCookieContainer(bool keep)
        {
            if (keep == true && HttpHandlerProvider.IsSupported == false)
            {
                var message = $"Unable to set KeepCookieContainer，please at {nameof(ConfigureHttpMessageHandler)} setting fixed for handler {nameof(CookieContainer)}";
                throw new PlatformNotSupportedException(message);
            }

            this.keepCookieContainer = keep;
            return this;
        }

        /// <summary>
        /// Configure the creation of HttpMessageHandler
        /// </summary>
        /// <param name="factory">Create delegate</param>
        /// <returns></returns>
        public HttpApiFactory ConfigureHttpMessageHandler(Func<HttpMessageHandler> factory)
        {
            this.handlerFactory = factory;
            return this;
        }

        /// <summary>
        /// Configure HttpApiConfig
        /// </summary>
        /// <param name="options">Configuration delegation</param>
        /// <returns></returns>
        public HttpApiFactory ConfigureHttpApiConfig(Action<HttpApiConfig> options)
        {
            this.configOptions = options;
            return this;
        }

        /// <summary>
        /// Create a proxy instance of the interface
        /// </summary>
        /// <returns></returns>
        public HttpApi CreateHttpApi()
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

        /// <summary>
        /// Create a proxy instance of the TInterface interface
        /// </summary>
        /// <param name="httpApiConfig">httpApi configuration</param>
        /// <returns></returns>
        protected virtual HttpApi CreateHttpApi(HttpApiConfig httpApiConfig)
        {
            return HttpApi.Create(this.InterfaceType, httpApiConfig);
        }


        #region Explicit interface implementation
        /// <summary>
        /// Configure the creation of HttpMessageHandler
        /// </summary>
        /// <param name="factory">Create delegate</param>
        void IHttpApiFactory.ConfigureHttpMessageHandler(Func<HttpMessageHandler> factory)
        {
            this.ConfigureHttpMessageHandler(factory);
        }

        /// <summary>
        /// Configure HttpApiConfig
        /// </summary>
        /// <param name="options">Configuration delegation</param>
        void IHttpApiFactory.ConfigureHttpApiConfig(Action<HttpApiConfig> options)
        {
            this.ConfigureHttpApiConfig(options);
        }
        #endregion
    }

    /// <summary>
    /// Indicates that HttpApi creates a factory
    /// Provide HttpApi configuration registration and instance creation
    /// and automatically manage the life cycle of the instance
    /// </summary>
    public class HttpApiFactory<TInterface> : HttpApiFactory, IHttpApiFactory<TInterface>
        where TInterface : class, IHttpApi
    {
        /// <summary>
        /// HttpApi create factory
        /// </summary>
        public HttpApiFactory()
            : base(typeof(TInterface))
        {
        }

        /// <summary>
        /// Create HttpApi proxy instance
        /// </summary>
        /// <returns></returns>
        public new TInterface CreateHttpApi()
        {
            return base.CreateHttpApi() as TInterface;
        }

        /// <summary>
        /// Setting the life cycle of an HttpApi instance
        /// </summary>
        /// <param name="lifeTime">Life cycle</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public new HttpApiFactory<TInterface> SetLifetime(TimeSpan lifeTime)
        {
            return base.SetLifetime(lifeTime) as HttpApiFactory<TInterface>;
        }

        /// <summary>
        /// Set the interval to clean up expired HttpApi instances
        /// </summary>
        /// <param name="interval">time interval</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public new HttpApiFactory<TInterface> SetCleanupInterval(TimeSpan interval)
        {
            return base.SetCleanupInterval(interval) as HttpApiFactory<TInterface>;
        }

        /// <summary>
        /// Set whether to maintain the use of a CookieContainer instance
        /// This instance is the CookieContainer when first created
        /// </summary>
        /// <param name="keep">true maintenance uses a CookieContainer instance</param>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <returns></returns>
        public new HttpApiFactory<TInterface> SetKeepCookieContainer(bool keep)
        {
            return base.SetKeepCookieContainer(keep) as HttpApiFactory<TInterface>;
        }

        /// <summary>
        /// Configure the creation of HttpMessageHandler
        /// </summary>
        /// <param name="factory">Create delegate</param>
        /// <returns></returns>
        public new HttpApiFactory<TInterface> ConfigureHttpMessageHandler(Func<HttpMessageHandler> factory)
        {
            return base.ConfigureHttpMessageHandler(factory) as HttpApiFactory<TInterface>;
        }

        /// <summary>
        /// Configure HttpApiConfig
        /// </summary>
        /// <param name="options">Configuration delegation</param>
        /// <returns></returns>
        public new HttpApiFactory<TInterface> ConfigureHttpApiConfig(Action<HttpApiConfig> options)
        {
            return base.ConfigureHttpApiConfig(options) as HttpApiFactory<TInterface>;
        }
    }
}