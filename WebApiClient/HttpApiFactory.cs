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
        /// 过期的记录
        /// </summary>
        private readonly ConcurrentQueue<ExpiredEntry> expiredEntries = new ConcurrentQueue<ExpiredEntry>();

        /// <summary>
        /// 激活的记录
        /// </summary>
        private volatile Lazy<ActiveEntry> activeEntryLazy;


        /// <summary>
        /// 获取生命周期
        /// </summary>
        TimeSpan IHttpApiFactory.Lifetime
        {
            get => this.lifeTime;
        }

        /// <summary>
        /// HttpApi创建工厂
        /// </summary>
        public HttpApiFactory()
        {
            this.activeEntryLazy = new Lazy<ActiveEntry>(
                this.CreateActiveEntry,
                LazyThreadSafetyMode.ExecutionAndPublication);

            this.RegisteCleanup();
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
            var interceptor = this.activeEntryLazy.Value.Interceptor;
            return HttpApiClient.Create(typeof(TInterface), interceptor);
        }

        /// <summary>
        /// 创建激活状态的记录
        /// </summary>
        /// <returns></returns>
        private ActiveEntry CreateActiveEntry()
        {
            var handler = this.handlerFunc?.Invoke() ?? new DefaultHttpClientHandler();
            var httpApiConfig = new HttpApiConfig(handler, true);
            var interceptor = new LifeTimeTrackingInterceptor(httpApiConfig);

            if (this.configAction != null)
            {
                this.configAction.Invoke(httpApiConfig);
            }

            return new ActiveEntry(this)
            {
                Interceptor = interceptor,
                Disposable = httpApiConfig
            };
        }

        /// <summary>
        /// 当有记录失效时
        /// </summary>
        /// <param name="active">激活的记录</param>
        void IHttpApiFactory.OnEntryDeactivate(ActiveEntry active)
        {
            // 切换激活状态的记录的实例
            this.activeEntryLazy = new Lazy<ActiveEntry>(
                this.CreateActiveEntry,
                LazyThreadSafetyMode.ExecutionAndPublication);

            var expired = new ExpiredEntry(active);
            this.expiredEntries.Enqueue(expired);
        }


        /// <summary>
        /// 注册清理任务
        /// </summary>
        private void RegisteCleanup()
        {
            Task.Delay(this.cleanupInterval)
                .ConfigureAwait(false)
                .GetAwaiter()
                .OnCompleted(this.CleanupCallback);
        }

        /// <summary>
        /// 清理任务回调
        /// </summary>
        private void CleanupCallback()
        {
            var count = this.expiredEntries.Count;
            for (var i = 0; i < count; i++)
            {
                this.expiredEntries.TryDequeue(out var entry);
                if (entry.CanDispose == true)
                {
                    entry.Dispose();
                }
                else
                {
                    this.expiredEntries.Enqueue(entry);
                }
            }


            this.RegisteCleanup();
        }
    }
}
