using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApiClient创建工厂
    /// </summary>
    public partial class HttpApiClientFactory : IHttpApiClientFactory
    {
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
        private readonly ConcurrentQueue<ExpiredHandlerEntry> expiredEntries;

        /// <summary>
        /// 激活记录的创建工厂
        /// </summary>
        private readonly Func<Type, Lazy<ActiveHandlerEntry>> activeEntryFactory;

        /// <summary>
        /// http接口客户端选项
        /// </summary>
        private readonly ConcurrentDictionary<Type, TypedClientOption> typedClientOptions;

        /// <summary>
        /// 激活的记录
        /// </summary>
        private readonly ConcurrentDictionary<Type, Lazy<ActiveHandlerEntry>> activeEntries;


        /// <summary>
        /// 获取或设置HttpMessageHandler的生命周期
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public TimeSpan Lifetime
        {
            get
            {
                return this.lifeTime;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this.lifeTime = value;
            }
        }

        /// <summary>
        /// 获取或设置清理过期的HttpMessageHandler的时间间隔
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public TimeSpan CleanupInterval
        {
            get
            {
                return this.cleanupInterval;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this.cleanupInterval = value;
            }
        }

        /// <summary>
        /// HttpApiClient创建工厂
        /// </summary>
        public HttpApiClientFactory()
        {
            this.expiredEntries = new ConcurrentQueue<ExpiredHandlerEntry>();
            this.activeEntries = new ConcurrentDictionary<Type, Lazy<ActiveHandlerEntry>>();
            this.typedClientOptions = new ConcurrentDictionary<Type, TypedClientOption>();
            this.activeEntryFactory = apiType => new Lazy<ActiveHandlerEntry>(() => this.CreateActiveEntry(apiType), LazyThreadSafetyMode.ExecutionAndPublication);

            this.RegisteCleanup();
        }

        /// <summary>
        /// 注册HttpApiClient对应的http接口
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="config">HttpApiConfig的配置</param>
        /// <param name="handlerFactory">HttpMessageHandler创建委托</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddTypedClient<TInterface>(Action<HttpApiConfig> config, Func<HttpMessageHandler> handlerFactory) where TInterface : class, IHttpApi
        {
            if (handlerFactory == null)
            {
                handlerFactory = () => new DefaultHttpClientHandler();
            }

            var options = new TypedClientOption
            {
                ConfigAction = config,
                HandlerFactory = handlerFactory
            };

            var state = this.typedClientOptions.TryAdd(typeof(TInterface), options);
            if (state == false)
            {
                throw new InvalidOperationException($"接口{typeof(TInterface)}不能重复注册");
            }
        }

        /// <summary>
        /// 创建实现了指定接口的HttpApiClient实例
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        public TInterface CreateTypedClient<TInterface>() where TInterface : class, IHttpApi
        {
            var apiType = typeof(TInterface);
            var entry = this.activeEntries.GetOrAdd(apiType, this.activeEntryFactory).Value;
            return HttpApiClient.Create<TInterface>(entry.HttpApiConfig);
        }

        /// <summary>
        /// 创建激活状态的Handler记录
        /// </summary>
        /// <param name="apiType">http接口类型</param>
        /// <returns></returns>
        private ActiveHandlerEntry CreateActiveEntry(Type apiType)
        {
            if (this.typedClientOptions.TryGetValue(apiType, out var option) == false)
            {
                throw new ArgumentException($"未注册的接口类型{apiType}");
            }

            var innder = option.HandlerFactory.Invoke();
            var handler = new LifeTimeTrackingHandler(innder);
            var httpApiConfig = new HttpApiConfig(handler, false);

            if (option.ConfigAction != null)
            {
                option.ConfigAction.Invoke(httpApiConfig);
            }

            return new ActiveHandlerEntry(this)
            {
                ApiType = apiType,
                Disposable = innder,
                HttpApiConfig = httpApiConfig
            };
        }

        /// <summary>
        /// 当有记录失效时
        /// </summary>
        /// <param name="active">激活的记录</param>
        void IHttpApiClientFactory.OnEntryDeactivate(ActiveHandlerEntry active)
        {
            this.activeEntries.TryRemove(active.ApiType, out var _);
            var expired = new ExpiredHandlerEntry(active);
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
