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
        /// 获取Xml格式化工具
        /// </summary>
        public IStringFormatter XmlFormatter { get; private set; }

        /// <summary>
        /// 获取Json格式化工具
        /// </summary>
        public IStringFormatter JsonFormatter { get; private set; }

        /// <summary>
        /// 获取HttpClient上下文提供者
        /// </summary>
        public IHttpClientContextProvider HttpClientContextProvider { get; private set; }


        /// <summary>
        /// HttpApiClient配置项
        /// </summary>
        public HttpApiClientConfig()
        {
            this.XmlFormatter = new DefaultXmlFormatter();
            this.JsonFormatter = new DefaultJsonFormatter();
            this.HttpClientContextProvider = new DefaultHttpClientContextProvider();
        }

        /// <summary>
        /// 使用Xml格式化工具
        /// </summary>
        /// <param name="xmlFormatter">Xml格式化工具</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void UseXmlFormatter(IStringFormatter xmlFormatter)
        {
            if (xmlFormatter == null)
            {
                throw new ArgumentNullException();
            }
            this.XmlFormatter = xmlFormatter;
        }

        /// <summary>
        /// 使用Json格式化工具
        /// </summary>
        /// <param name="jsonFormatter">Json格式化工具</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void UseJsonFormatter(IStringFormatter jsonFormatter)
        {
            if (jsonFormatter == null)
            {
                throw new ArgumentNullException();
            }
            this.JsonFormatter = jsonFormatter;
        }

        /// <summary>
        /// HttpClient上下文提供者
        /// </summary>
        /// <param name="provider">提供者</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void UseHttpClientContextProvider(IHttpClientContextProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException();
            }
            this.HttpClientContextProvider = provider;
        }
    }
}
