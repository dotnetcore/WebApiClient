using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// Configuration items representing Http interface
    /// </summary>
    public class HttpApiConfig : Disposable
    {
        /// <summary>
        /// IHttpHandler associated with HttpClient
        /// </summary>
        private readonly Lazy<IHttpHandler> httpHandler;


        /// <summary>
        /// Directly assigned log factory
        /// </summary>
        private ILoggerFactory loggerFactory;

        /// <summary>
        /// Format options
        /// </summary>
        private FormatOptions formatOptions = new FormatOptions();

        /// <summary>
        /// xml serialization tool
        /// </summary>
        private IXmlFormatter xmlFormatter = Defaults.XmlFormatter.Instance;

        /// <summary>
        /// json serialization tool
        /// </summary>
        private IJsonFormatter jsonFormatter = Defaults.JsonFormatter.Instance;

        /// <summary>
        /// key-value serialization tool
        /// </summary>
        private IKeyValueFormatter keyValueFormatter = Defaults.KeyValueFormatter.Instance;



        /// <summary>
        /// Get HttpClient instance
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// Get configured custom data storage and access containers
        /// </summary>
        public Tags Tags { get; } = new Tags();

        /// <summary>
        /// Get the IHttpHandler associated with HttpClient
        /// </summary>
        /// <exception cref="PlatformNotSupportedException"></exception>
        public IHttpHandler HttpHandler => this.httpHandler.Value;

        /// <summary>
        /// Get global filter collection
        /// Non-thread-safe type
        /// </summary>
        public GlobalFilterCollection GlobalFilters { get; } = new GlobalFilterCollection();



        /// <summary>
        /// Gets or sets whether to perform input validity verification on the attribute value of the parameter
        /// Default is true
        /// </summary>
        public bool UseParameterPropertyValidate { get; set; } = true;

        /// <summary>
        /// Gets or sets whether input validity validation is performed on the attribute value of the returned value
        /// Default is true
        /// </summary>
        public bool UseReturnValuePropertyValidate { get; set; } = true;

        /// <summary>
        /// Gets or sets the cache provider of the API
        /// </summary>
        public IResponseCacheProvider ResponseCacheProvider { get; set; } = Defaults.ResponseCacheProvider.Instance;


        /// <summary>
        /// Get or set a service provider
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Get or set unified log factory
        /// Get instance from ServiceProvider by default 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public ILoggerFactory LoggerFactory
        {
            get => this.loggerFactory ?? (ILoggerFactory)this.ServiceProvider?.GetService(typeof(ILoggerFactory));
            set => this.loggerFactory = value ?? throw new ArgumentNullException(nameof(LoggerFactory));
        }

        /// <summary>
        /// Get or set the full host domain name of the HTTP service
        /// E.g. http://www.webapiclient.com
        /// HttpHost value is set, HttpHostAttribute will be invalid  
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public Uri HttpHost
        {
            get => this.HttpClient.BaseAddress;
            set => this.HttpClient.BaseAddress = value ?? throw new ArgumentNullException(nameof(HttpHost));
        }

        /// <summary>
        /// Gets or sets the default format used by serialization when requesting   
        /// Affects serialization of JsonFormatter or KeyValueFormatter   
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public FormatOptions FormatOptions
        {
            get => formatOptions;
            set => formatOptions = value ?? throw new ArgumentNullException(nameof(FormatOptions));
        }

        /// <summary>
        /// Get or set Xml formatting tool
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public IXmlFormatter XmlFormatter
        {
            get => xmlFormatter;
            set => xmlFormatter = value ?? throw new ArgumentNullException(nameof(XmlFormatter));
        }

        /// <summary>
        /// Get or set Json formatting tool
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public IJsonFormatter JsonFormatter
        {
            get => jsonFormatter;
            set => jsonFormatter = value ?? throw new ArgumentNullException(nameof(JsonFormatter));
        }

        /// <summary>
        /// Gets or sets the KeyValue formatting tool
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public IKeyValueFormatter KeyValueFormatter
        {
            get => keyValueFormatter;
            set => keyValueFormatter = value ?? throw new ArgumentNullException(nameof(KeyValueFormatter));
        }


        /// <summary>
        /// Configuration items of the Http interface   
        /// </summary>
        public HttpApiConfig() :
            this(new DefaultHttpClientHandler(), true)
        {
        }

        /// <summary>
        /// Configuration items of the Http interface   
        /// </summary>
        /// <param name="handler">HTTP message handler</param>
        /// <param name="disposeHandler">When using the Dispose method, is it also a Dispose handler</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpApiConfig(HttpMessageHandler handler, bool disposeHandler = false)
            : this(new HttpClient(handler, disposeHandler))
        {
        }

        /// <summary>
        /// Configuration items of the Http interface
        /// </summary>
        /// <param name="httpClient">External HttpClient instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpApiConfig(HttpClient httpClient)
        {
            this.HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.httpHandler = new Lazy<IHttpHandler>(() => HttpHandlerProvider.CreateHandler(httpClient), true);
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="disposing">Whether to release managed resources</param>
        protected override void Dispose(bool disposing)
        {
            this.HttpClient.Dispose();
        }
    }
}
