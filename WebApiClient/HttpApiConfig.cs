using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// 表示Http接口的配置项
    /// </summary>
    public class HttpApiConfig : Disposable
    {
        /// <summary>
        /// 与HttpClient关联的IHttpHandler
        /// </summary>
        private readonly Lazy<IHttpHandler> httpHandler;


        /// <summary>
        /// 直接赋值的日志工厂
        /// </summary>
        private ILoggerFactory loggerFactory;

        /// <summary>
        /// 格式选项
        /// </summary>
        private FormatOptions formatOptions = new FormatOptions();

        /// <summary>
        /// xml序列化工具
        /// </summary>
        private IXmlFormatter xmlFormatter = Defaults.XmlFormatter.Instance;

        /// <summary>
        /// json序列化工具
        /// </summary>
        private IJsonFormatter jsonFormatter = Defaults.JsonFormatter.Instance;

        /// <summary>
        /// key-value序列化工具
        /// </summary>
        private IKeyValueFormatter keyValueFormatter = Defaults.KeyValueFormatter.Instance;



        /// <summary>
        /// 获取HttpClient实例
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// 获取配置的自定义数据的存储和访问容器
        /// </summary>
        public Tags Tags { get; } = new Tags();

        /// <summary>
        /// 获取与HttpClient关联的IHttpHandler
        /// </summary>
        /// <exception cref="PlatformNotSupportedException"></exception>
        public IHttpHandler HttpHandler => this.httpHandler.Value;

        /// <summary>
        /// 获取全局过滤器集合
        /// 非线程安全类型
        /// </summary>
        public GlobalFilterCollection GlobalFilters { get; } = new GlobalFilterCollection();



        /// <summary>
        /// 获取或设置是否对参数的属性值进行输入有效性验证
        /// 默认为true
        /// </summary>
        public bool UseParameterPropertyValidate { get; set; } = true;

        /// <summary>
        /// 获取或设置是否对返回值的属性值进行输入有效性验证
        /// 默认为true
        /// </summary>
        public bool UseReturnValuePropertyValidate { get; set; } = true;

        /// <summary>
        /// 获取或设置Api的缓存提供者
        /// </summary>
        public IResponseCacheProvider ResponseCacheProvider { get; set; } = Defaults.ResponseCacheProvider.Instance;


        /// <summary>
        /// 获取或设置服务提供者
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// 获取或设置统一日志工厂
        /// 默认从ServiceProvider获取实例 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public ILoggerFactory LoggerFactory
        {
            get => this.loggerFactory ?? (ILoggerFactory)this.ServiceProvider?.GetService(typeof(ILoggerFactory));
            set => this.loggerFactory = value ?? throw new ArgumentNullException(nameof(LoggerFactory));
        }

        /// <summary>
        /// 获取或设置Http服务完整主机域名
        /// 例如http://www.webapiclient.com
        /// 设置了HttpHost值，HttpHostAttribute将失效  
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public Uri HttpHost
        {
            get => this.HttpClient.BaseAddress;
            set => this.HttpClient.BaseAddress = value ?? throw new ArgumentNullException(nameof(HttpHost));
        }

        /// <summary>
        /// 获取或设置请求时序列化使用的默认格式   
        /// 影响JsonFormatter或KeyValueFormatter的序列化   
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public FormatOptions FormatOptions
        {
            get => formatOptions;
            set => formatOptions = value ?? throw new ArgumentNullException(nameof(FormatOptions));
        }

        /// <summary>
        /// 获取或设置Xml格式化工具
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public IXmlFormatter XmlFormatter
        {
            get => xmlFormatter;
            set => xmlFormatter = value ?? throw new ArgumentNullException(nameof(XmlFormatter));
        }

        /// <summary>
        /// 获取或设置Json格式化工具
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public IJsonFormatter JsonFormatter
        {
            get => jsonFormatter;
            set => jsonFormatter = value ?? throw new ArgumentNullException(nameof(JsonFormatter));
        }

        /// <summary>
        /// 获取或设置KeyValue格式化工具
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public IKeyValueFormatter KeyValueFormatter
        {
            get => keyValueFormatter;
            set => keyValueFormatter = value ?? throw new ArgumentNullException(nameof(KeyValueFormatter));
        }


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
            this.HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.httpHandler = new Lazy<IHttpHandler>(() => HttpHandlerProvider.CreateHandler(httpClient), true);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否也释放托管资源</param>
        protected override void Dispose(bool disposing)
        {
            this.HttpClient.Dispose();
        }
    }
}
