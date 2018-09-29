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
    public partial class HttpApiFactory : IHttpApiFactory, _IHttpApiFactory
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
        private readonly ConcurrentQueue<ExpiredEntry> expiredEntries;

        /// <summary>
        /// 激活记录的创建工厂
        /// </summary>
        private readonly Func<Type, Lazy<ActiveEntry>> activeEntryFactory;

        /// <summary>
        /// http接口代理创建选项
        /// </summary>
        private readonly ConcurrentDictionary<Type, HttpApiCreateOption> httpApiCreateOptions;

        /// <summary>
        /// 激活的记录
        /// </summary>
        private readonly ConcurrentDictionary<Type, Lazy<ActiveEntry>> activeEntries;

        /// <summary>
        /// 获取已过期但还未释放的HttpApi实例数量
        /// </summary>
        public int ExpiredCount
        {
            get => this.expiredEntries.Count;
        }

        /// <summary>
        /// 获取或设置HttpApi实例的生命周期
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
        /// 获取或设置清理过期的HttpApi实例的时间间隔
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
        /// HttpApi创建工厂
        /// </summary>
        public HttpApiFactory()
        {
            this.expiredEntries = new ConcurrentQueue<ExpiredEntry>();
            this.activeEntries = new ConcurrentDictionary<Type, Lazy<ActiveEntry>>();
            this.httpApiCreateOptions = new ConcurrentDictionary<Type, HttpApiCreateOption>();
            this.activeEntryFactory = apiType => new Lazy<ActiveEntry>(() => this.CreateActiveEntry(apiType), LazyThreadSafetyMode.ExecutionAndPublication);

            this.RegisteCleanup();
        }

        /// <summary>
        /// 注册http接口
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="config">HttpApiConfig的配置</param>
        /// <param name="handlerFactory">HttpMessageHandler创建委托</param>
        /// <returns></returns>
        public bool AddHttpApi<TInterface>(Action<HttpApiConfig> config, Func<HttpMessageHandler> handlerFactory) where TInterface : class, IHttpApi
        {
            if (handlerFactory == null)
            {
                handlerFactory = () => new DefaultHttpClientHandler();
            }

            var options = new HttpApiCreateOption
            {
                ConfigAction = config,
                HandlerFactory = handlerFactory
            };

            return this.httpApiCreateOptions.TryAdd(typeof(TInterface), options);
        }

        /// <summary>
        /// 创建指定接口的代理实例
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public TInterface CreateHttpApi<TInterface>() where TInterface : class, IHttpApi
        {
            var apiType = typeof(TInterface);
            var entry = this.activeEntries.GetOrAdd(apiType, this.activeEntryFactory).Value;
            return HttpApiClient.Create(apiType, entry.Interceptor) as TInterface;
        }

        /// <summary>
        /// 创建激活状态的记录
        /// </summary>
        /// <param name="apiType">http接口类型</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        private ActiveEntry CreateActiveEntry(Type apiType)
        {
            if (this.httpApiCreateOptions.TryGetValue(apiType, out var option) == false)
            {
                throw new ArgumentException($"未注册的接口类型{apiType}");
            }

            var handler = option.HandlerFactory.Invoke();
            var httpApiConfig = new HttpApiConfig(handler, false);
            var interceptor = new LifeTimeTrackingInterceptor(httpApiConfig);

            if (option.ConfigAction != null)
            {
                option.ConfigAction.Invoke(httpApiConfig);
            }

            return new ActiveEntry(this)
            {
                ApiType = apiType,
                Disposable = httpApiConfig,
                Interceptor = interceptor
            };
        }

        /// <summary>
        /// 当有记录失效时
        /// </summary>
        /// <param name="active">激活的记录</param>
        void _IHttpApiFactory.OnEntryDeactivate(ActiveEntry active)
        {
            this.activeEntries.TryRemove(active.ApiType, out var _);
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
