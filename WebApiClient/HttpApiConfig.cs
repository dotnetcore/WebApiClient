using System;
using System.Net.Http;
using WebApiClient.Defaults;

namespace WebApiClient
{
    /// <summary>
    /// 表示Http接口的配置项
    /// </summary>
    public class HttpApiConfig : IDisposable
    {
        /// <summary>
        /// 获取默认xml格式化工具唯一实例
        /// </summary>
        public static readonly IXmlFormatter DefaultXmlFormatter = new XmlFormatter();

        /// <summary>
        /// 获取默认json格式化工具唯一实例
        /// </summary>
        public static readonly IJsonFormatter DefaultJsonFormatter = new JsonFormatter();

        /// <summary>
        /// 获取默认KeyValue格式化工具唯一实例
        /// </summary>
        public static readonly IKeyValueFormatter DefaultKeyValueFormatter = new KeyValueFormatter();


        /// <summary>
        /// 自定义数据容器
        /// </summary>
        private Tags tags;

        /// <summary>
        /// 关联的HttpClient
        /// </summary>
        private HttpClient httpClient;

        /// <summary>
        /// 与httpClient关联的IHttpHandler
        /// </summary>
        private IHttpHandler httpHandler;

        /// <summary>
        /// 同步锁
        /// </summary>
        private readonly object syncRoot = new object();


        /// <summary>
        /// 获取配置的自定义数据的存储和访问容器
        /// </summary>
        public Tags Tags
        {
            get => this.GetTagsSafeSync();
        }

        /// <summary>
        /// 获取HttpClient实例
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        public HttpClient HttpClient
        {
            get => this.GetHttpClientSafeSync();
        }

        /// <summary>
        /// 获取与HttpClient关联的IHttpHandler
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        public IHttpHandler HttpHandler
        {
            get => this.GetHttpHandlerSafeSync();
        }

        /// <summary>
        /// 获取或设置Http服务完整主机域名
        /// 例如http://www.webapiclient.com
        /// 设置了HttpHost值，HttpHostAttribute将失效  
        /// </summary>
        public Uri HttpHost { get; set; }

        /// <summary>
        /// 获取或设置是否对参数的属性值进行输入有效性验证
        /// 默认为true
        /// </summary>
        public bool UseParameterPropertyValidate { get; set; } = true;

        /// <summary>
        /// 获取或设置请求时序列化使用的默认格式   
        /// 影响JsonFormatter或KeyValueFormatter的序列化
        /// </summary>
        public FormatOptions FormatOptions { get; set; } = new FormatOptions();

        /// <summary>
        /// 获取或设置Xml格式化工具
        /// </summary>
        public IXmlFormatter XmlFormatter { get; set; } = DefaultXmlFormatter;

        /// <summary>
        /// 获取或设置Json格式化工具
        /// </summary>
        public IJsonFormatter JsonFormatter { get; set; } = DefaultJsonFormatter;

        /// <summary>
        /// 获取或设置KeyValue格式化工具
        /// </summary>
        public IKeyValueFormatter KeyValueFormatter { get; set; } = DefaultKeyValueFormatter;

        /// <summary>
        /// 获取全局过滤器集合
        /// 非线程安全类型
        /// </summary>
        public GlobalFilterCollection GlobalFilters { get; private set; } = new GlobalFilterCollection();

        /// <summary>
        /// Http接口的配置项   
        /// </summary>
        public HttpApiConfig() :
            this(new DefaultHttpClientHandler(), true)
        {
        }

        /// <summary>
        /// Http接口的配置项   
        /// </summary>
        /// <param name="handler">HTTP消息处理程序</param>
        /// <param name="disposeHandler">用Dispose方法时，是否也Dispose handler</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpApiConfig(HttpMessageHandler handler, bool disposeHandler = false)
            : this(new HttpClient(handler, disposeHandler))
        {
        }

        /// <summary>
        /// Http接口的配置项
        /// </summary>
        /// <param name="httpClient">外部HttpClient实例</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpApiConfig(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.HttpHost = httpClient.BaseAddress;

            var userAgent = httpClient.DefaultRequestHeaders.UserAgent;
            if (userAgent.Count == 0)
            {
                userAgent.Add(HttpHandlerProvider.DefaultUserAgent);
            }
        }

        /// <summary>
        /// 以同步安全方式获取Tags实例
        /// </summary>
        /// <returns></returns>
        private Tags GetTagsSafeSync()
        {
            lock (this.syncRoot)
            {
                if (this.tags == null)
                {
                    this.tags = new Tags();
                }
                return this.tags;
            }
        }

        /// <summary>
        /// 以同步安全方式获取HttpClient实例
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <returns></returns>
        private HttpClient GetHttpClientSafeSync()
        {
            lock (this.syncRoot)
            {
                if (this.IsDisposed == true)
                {
                    throw new ObjectDisposedException(this.GetType().Name);
                }
                return this.httpClient;
            }
        }


        /// <summary>
        /// 以同步安全方式获取IHttpHandler实例
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <returns></returns>
        private IHttpHandler GetHttpHandlerSafeSync()
        {
            lock (this.syncRoot)
            {
                if (this.IsDisposed == true)
                {
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                if (this.httpHandler == null)
                {
                    this.httpHandler = HttpHandlerProvider.CreateHandler(this.httpClient);
                }
                return this.httpHandler;
            }
        }

        #region IDisposable
        /// <summary>
        /// 获取对象是否已释放
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// 关闭和释放所有相关资源
        /// </summary>
        public void Dispose()
        {
            if (this.IsDisposed == false)
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
            this.IsDisposed = true;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~HttpApiConfig()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否也释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.httpClient != null)
            {
                this.httpClient.Dispose();
            }
        }
        #endregion
    }
}
