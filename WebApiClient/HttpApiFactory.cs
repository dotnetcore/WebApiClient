using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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
        /// 清理handler时间间隔
        /// </summary>
        private TimeSpan cleanupInterval = TimeSpan.FromSeconds(10d);

        /// <summary>
        /// 过期的拦截器
        /// </summary>
        private readonly ConcurrentQueue<ExpiredInterceptor> expiredInterceptors = new ConcurrentQueue<ExpiredInterceptor>();

        /// <summary>
        /// 当前过期的拦截器的数量
        /// </summary>
        private int expiredCount = 0;

        /// <summary>
        /// 激活的拦截器
        /// </summary>
        private Lazy<ActiveInterceptor> activeInterceptorLazy;

        /// <summary>
        /// HttpApi创建工厂
        /// </summary>
        public HttpApiFactory()
        {
            this.activeInterceptorLazy = new Lazy<ActiveInterceptor>(
                this.CreateActiveInterceptor,
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
            this.cleanupInterval = interval;
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
            var interceptor = this.activeInterceptorLazy.Value;
            return HttpApiClient.Create(typeof(TInterface), interceptor);
        }

        /// <summary>
        /// 创建激活状态的拦截器
        /// </summary>
        /// <returns></returns>
        private ActiveInterceptor CreateActiveInterceptor()
        {
            var handler = this.handlerFunc?.Invoke() ?? new DefaultHttpClientHandler();
            var httpApiConfig = new HttpApiConfig(handler, true);

            if (this.configAction != null)
            {
                this.configAction.Invoke(httpApiConfig);
            }

            return new ActiveInterceptor(
                httpApiConfig,
                this.lifeTime,
                this.OnInterceptorDeactivate);
        }

        /// <summary>
        /// 当有拦截器失效时
        /// </summary>
        /// <param name="active">激活的拦截器</param>
        private void OnInterceptorDeactivate(ActiveInterceptor active)
        {
            // 切换激活状态的记录的实例
            this.activeInterceptorLazy = new Lazy<ActiveInterceptor>(
                this.CreateActiveInterceptor,
                LazyThreadSafetyMode.ExecutionAndPublication);

            var expired = new ExpiredInterceptor(active);
            this.expiredInterceptors.Enqueue(expired);

            // 从0变为1，要启动清理作业
            if (Interlocked.Increment(ref this.expiredCount) == 1)
            {
                this.RegisteCleanup();
            }
        }


        /// <summary>
        /// 注册清理任务
        /// </summary>
        private async void RegisteCleanup()
        {
            while (this.Cleanup() == false)
            {
                await Task.Delay(this.cleanupInterval).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 清理失效的拦截器
        /// </summary>
        /// <returns>是否完全清理</returns>
        private bool Cleanup()
        {
            var cleanCount = this.expiredInterceptors.Count;
            for (var i = 0; i < cleanCount; i++)
            {
                this.expiredInterceptors.TryDequeue(out var expired);
                if (expired.CanDispose == false)
                {
                    this.expiredInterceptors.Enqueue(expired);
                }
                else
                {
                    expired.Dispose();
                    if (Interlocked.Decrement(ref this.expiredCount) == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}