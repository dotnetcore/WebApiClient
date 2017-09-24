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
        /// 获取或设置Http处理程序
        /// </summary>
        public HttpClientHandler HttpClientHandler { get; set; }

        /// <summary>
        /// 获取或设置与HttpClientHandler实例关联的HttpClient
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// Http接口的配置项   
        /// </summary>
        public HttpApiConfig()
        {
        }

        /// <summary>
        /// 将null值的属性设置为默认
        /// </summary>
        internal protected virtual void SetNullPropertyAsDefault()
        {
            if (this.XmlFormatter == null)
            {
                this.XmlFormatter = new DefaultXmlFormatter();
            }

            if (this.JsonFormatter == null)
            {
                this.JsonFormatter = new DefaultJsonFormatter();
            }

            if (this.HttpClientHandler == null)
            {
                this.HttpClientHandler = new DefaultHttpClientHandler(keepAlive: true);
            }

            if (this.HttpClient == null)
            {
                this.HttpClient = new HttpClient(this.HttpClientHandler);
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
            if (this.HttpClient != null)
            {
                this.HttpClient.Dispose();
            }

            if (disposing == true)
            {
                this.XmlFormatter = null;
                this.JsonFormatter = null;
                this.HttpClient = null;
                this.HttpClientHandler = null;
                this.HttpHost = null;
            }
        }
        #endregion
    }
}
