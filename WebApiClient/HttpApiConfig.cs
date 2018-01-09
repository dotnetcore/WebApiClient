using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示Http接口的配置项
    /// 更多的配置项，可以继承此类
    /// </summary>
    public class HttpApiConfig : IDisposable
    {
        /// <summary>
        /// 获取默认xml格式化工具唯一实例
        /// </summary>
        public static readonly IStringFormatter DefaultXmlFormatter = new DefaultXmlFormatter();

        /// <summary>
        /// 获取默认json格式化工具唯一实例
        /// 如果应用程序池加载了Newtonsoft.Json，则使用Newtonsoft.Json.JsonConvert
        /// 否则使用运行库的System.Web.Script.Serialization.JavaScriptSerializer
        /// </summary>
        public static readonly IStringFormatter DefaultJsonFormatter = new DefaultJsonFormatter();

        /// <summary>
        /// 获取默认KeyValue格式化工具唯一实例
        /// </summary>
        public static readonly IKeyValueFormatter DefaultKeyValueFormatter = new DefaultKeyValueFormatter();


        /// <summary>
        /// 与HttpClientHandler实例关联的HttpClient
        /// </summary>
        private IHttpClient httpClient;


        /// <summary>
        /// 获取或设置Http服务完整主机域名
        /// 例如http://www.webapiclient.com
        /// 设置了HttpHost值，HttpHostAttribute将失效  
        /// </summary>
        public Uri HttpHost { get; set; }

        /// <summary>
        /// 获取或设置Xml格式化工具
        /// </summary>
        public IStringFormatter XmlFormatter { get; set; }

        /// <summary>
        /// 获取或设置Json格式化工具
        /// </summary>
        public IStringFormatter JsonFormatter { get; set; }

        /// <summary>
        /// 获取或设置KeyValue格式化工具
        /// </summary>
        public IKeyValueFormatter KeyValueFormatter { get; set; }

        /// <summary>
        /// 获取HttpClient实例
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        public IHttpClient HttpClient
        {
            get
            {
                if (this.IsDisposed == true)
                {
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                if (this.httpClient == null)
                {
                    this.httpClient = new DefaultHttpClient();
                }
                return this.httpClient;
            }
        }

        /// <summary>
        /// Http接口的配置项   
        /// </summary>
        public HttpApiConfig() :
            this(null)
        {
        }

        /// <summary>
        /// Http接口的配置项   
        /// </summary>
        /// <param name="client">客户端对象</param>
        public HttpApiConfig(IHttpClient client)
        {
            this.httpClient = client;
            this.XmlFormatter = HttpApiConfig.DefaultXmlFormatter;
            this.JsonFormatter = HttpApiConfig.DefaultJsonFormatter;
            this.KeyValueFormatter = HttpApiConfig.DefaultKeyValueFormatter;
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
