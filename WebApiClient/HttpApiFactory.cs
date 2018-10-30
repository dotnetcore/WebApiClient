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
    public class HttpApiFactory<TInterface> : IHttpApiFactory<TInterface>, IHttpApiFactory
        where TInterface : class, IHttpApi
    {
        /// <summary>
        /// HttpApiConfig的配置委托
        /// </summary>
        private Action<HttpApiConfig> configAction;

        /// <summary>
        /// HttpMessageHandler的创建委托
        /// </summary>
        private Func<HttpMessageHandler> handlerFunc;

        /// <summary>
        /// handler的生命周期
        /// </summary>
        private TimeSpan lifeTime = TimeSpan.FromMinutes(2d);

        /// <summary>
        /// 具有生命周期的拦截器延时创建对象
        /// </summary>
        private Lazy<LifetimeInterceptor> lifeTimeInterceptorLazy;

        /// <summary>
        /// 拦截器清理器
        /// </summary>
        private readonly InterceptorCleaner interceptorCleaner = new InterceptorCleaner();

        /// <summary>
        /// 是否保持cookie容器
        /// </summary>
        private bool keepCookieContainer = HttpHandlerProvider.IsSupported;

        /// <summary>
        /// cookie容器
        /// </summary>
        private CookieContainer cookieContainer;

        /// <summary>
        /// HttpApi创建工厂
        /// </summary>
        public HttpApiFactory()
        {
            this.lifeTimeInterceptorLazy = new Lazy<LifetimeInterceptor>(
                this.CreateInterceptor,
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// 置HttpApi实例的生命周期
        /// </summary>
        /// <param name="lifeTime">生命周期</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public HttpApiFactory<TInterface> SetLifetime(TimeSpan lifeTime)
        {
            if (lifeTime <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.lifeTime = lifeTime;
            return this;
        }

        /// <summary>
        /// 获取或设置清理过期的HttpApi实例的时间间隔
        /// </summary>
        /// <param name="interval">时间间隔</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public HttpApiFactory<TInterface> SetCleanupInterval(TimeSpan interval)
        {
            if (interval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.interceptorCleaner.CleanupInterval = interval;
            return this;
        }

        /// <summary>
        /// 设置是否维护使用一个CookieContainer实例
        /// 该实例为首次创建时的CookieContainer
        /// </summary>
        /// <param name="keep">true维护使用一个CookieContainer实例</param>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <returns></returns>
        public HttpApiFactory<TInterface> SetKeepCookieContainer(bool keep)
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
        /// <param name="handlerFunc">创建委托</param>
        /// <returns></returns>
        public HttpApiFactory<TInterface> ConfigureHttpMessageHandler(Func<HttpMessageHandler> handlerFunc)
        {
            this.handlerFunc = handlerFunc;
            return this;
        }

        /// <summary>
        /// 配置HttpApiConfig
        /// </summary>
        /// <param name="configAction">配置委托</param>
        /// <returns></returns>
        public HttpApiFactory<TInterface> ConfigureHttpApiConfig(Action<HttpApiConfig> configAction)
        {
            this.configAction = configAction;
            return this;
        }

        /// <summary>
        /// 创建接口的代理实例
        /// </summary>
        /// <returns></returns>
        public TInterface CreateHttpApi()
        {
            return ((IHttpApiFactory)this).CreateHttpApi() as TInterface;
        }

        /// <summary>
        /// 创建接口的代理实例
        /// </summary>
        /// <returns></returns>
        object IHttpApiFactory.CreateHttpApi()
        {
            var interceptor = this.lifeTimeInterceptorLazy.Value;
            return HttpApiClient.Create(typeof(TInterface), interceptor);
        }

        /// <summary>
        /// 创建LifetimeInterceptor
        /// </summary>
        /// <returns></returns>
        private LifetimeInterceptor CreateInterceptor()
        {
            var handler = this.handlerFunc?.Invoke() ?? new DefaultHttpClientHandler();
            var httpApiConfig = new HttpApiConfig(handler, true);

            if (this.configAction != null)
            {
                this.configAction.Invoke(httpApiConfig);
            }

            if (this.keepCookieContainer == true)
            {
                var handlerContainer = httpApiConfig.HttpHandler.CookieContainer;
                Interlocked.CompareExchange(ref this.cookieContainer, handlerContainer, null);
                httpApiConfig.HttpHandler.CookieContainer = this.cookieContainer;
            }

            return new LifetimeInterceptor(
                httpApiConfig,
                this.lifeTime,
                this.OnInterceptorDeactivate);
        }

        /// <summary>
        /// 当有拦截器失效时
        /// </summary>
        /// <param name="interceptor">拦截器</param>
        private void OnInterceptorDeactivate(LifetimeInterceptor interceptor)
        {
            // 切换激活状态的记录的实例
            this.lifeTimeInterceptorLazy = new Lazy<LifetimeInterceptor>(
                this.CreateInterceptor,
                LazyThreadSafetyMode.ExecutionAndPublication);

            this.interceptorCleaner.Add(interceptor);
        }
    }
}