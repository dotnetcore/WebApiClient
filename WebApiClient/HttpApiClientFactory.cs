using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApiClient创建工厂
    /// </summary>
    public class HttpApiClientFactory : IHttpApiClientFactory
    {
        /// <summary>
        /// 获取默认的实例
        /// </summary>
        public static readonly HttpApiClientFactory Default = new HttpApiClientFactory();

        private readonly TimeSpan defaultCleanupInterval = TimeSpan.FromSeconds(10d);

        private readonly ConcurrentQueue<ExpiredHandlerEntry> expiredEntries;

        private readonly Func<Type, Lazy<ActiveHandlerEntry>> activeEntryFactory;

        private readonly ConcurrentDictionary<Type, Action<HttpApiConfig>> configs;

        private readonly ConcurrentDictionary<Type, Lazy<ActiveHandlerEntry>> activeEntries;


        public TimeSpan Lifetime { get; set; } = TimeSpan.FromMinutes(2d);


        public HttpApiClientFactory()
        {
            this.expiredEntries = new ConcurrentQueue<ExpiredHandlerEntry>();
            this.configs = new ConcurrentDictionary<Type, Action<HttpApiConfig>>();
            this.activeEntries = new ConcurrentDictionary<Type, Lazy<ActiveHandlerEntry>>();
            this.activeEntryFactory = apiType => new Lazy<ActiveHandlerEntry>(() => this.CreateActiveEntry(apiType), LazyThreadSafetyMode.ExecutionAndPublication);

            this.RegisteCleanup();
        }

        /// <summary>
        /// 注册HttpApiClient对应的http接口
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="config">配置</param>
        /// <returns></returns>
        public bool AddHttpApiClient<TInterface>(Action<HttpApiConfig> config) where TInterface : class, IHttpApi
        {
            return this.configs.TryAdd(typeof(TInterface), config);
        }

        /// <summary>
        /// 创建实现了指定接口的HttpApiClient实例
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        public TInterface CreateHttpApiClient<TInterface>() where TInterface : class, IHttpApi
        {
            var apiType = typeof(TInterface);
            var entry = this.activeEntries.GetOrAdd(apiType, this.activeEntryFactory).Value;
            return HttpApiClient.Create<TInterface>(entry.HttpApiConfig);
        }

        private ActiveHandlerEntry CreateActiveEntry(Type apiType)
        {
            var handler = new LifeTimeTrackingHandler(new DefaultHttpClientHandler());
            var httpApiConfig = new HttpApiConfig(handler, false);

            if (this.configs.TryGetValue(apiType, out Action<HttpApiConfig> config) == false)
            {
                throw new ArgumentException($"未注册的接口类型{apiType}");
            }
            else if (config != null)
            {
                config.Invoke(httpApiConfig);
            }

            return new ActiveHandlerEntry(this)
            {
                ApiType = apiType,
                HttpApiConfig = httpApiConfig,
                InnerHandler = handler.InnerHandler
            };
        }

        void IHttpApiClientFactory.OnEntryDeactivate(ActiveHandlerEntry active)
        {
            this.activeEntries.TryRemove(active.ApiType, out var _);
            var expired = new ExpiredHandlerEntry(active);
            this.expiredEntries.Enqueue(expired);
        }

        private void RegisteCleanup()
        {
            Task.Delay(this.defaultCleanupInterval)
                .ConfigureAwait(false)
                .GetAwaiter()
                .OnCompleted(this.CleanupCallback);
        }

        private void CleanupCallback()
        {
            var count = this.expiredEntries.Count;
            for (var i = 0; i < count; i++)
            {
                this.expiredEntries.TryDequeue(out var entry);
                if (entry.CanDispose() == true)
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
