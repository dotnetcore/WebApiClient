using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// HttpApiClient配置项
    /// </summary>
    public class HttpApiClientConfig
    {
        /// <summary>
        /// 获取或设置Xml格式化工具
        /// </summary>
        public IStringFormatter XmlFormatter { get; set; }

        /// <summary>
        /// 获取或设置Json格式化工具
        /// </summary>
        public IStringFormatter JsonFormatter { get; set; }

        /// <summary>
        /// 获取或设置httpClient提供者
        /// </summary>
        public IHttpClientProvider HttpClientProvider { get; set; }


        /// <summary>
        /// HttpApiClient配置项
        /// </summary>
        public HttpApiClientConfig()
        {
            this.XmlFormatter = new DefaultXmlFormatter();
            this.JsonFormatter = new DefaultJsonFormatter();
            this.HttpClientProvider = new HttpClientProvider(InstanceType.SingleInstance);
        }
    }
}
