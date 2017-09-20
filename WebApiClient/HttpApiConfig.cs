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
    /// </summary>
    public class HttpApiConfig
    {
        /// <summary>
        /// 获取或设置Http服务完整主机域名
        /// 例如http://www.webapiclient.com
        /// 设置了HttpHost值，HttpHostAttribute将失效  
        /// </summary>
        public Uri HttpHost { get; set; }

        /// <summary>
        /// 获取或设置自定义相关数据
        /// </summary>
        public object UserToken { get; set; }

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
        /// 所有属性的生命周期与Api接口代理实例一致
        /// </summary>
        public HttpApiConfig()
        {
        }

        /// <summary>
        /// 将null值的属性设置为默认
        /// </summary>
        public virtual void SetNullAsDefault()
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
    }
}
