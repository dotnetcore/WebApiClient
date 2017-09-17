using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将参数值作为application/xml请求
    /// </summary>
    public class XmlContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 编码方式
        /// </summary>
        private readonly Encoding encoding;

        /// <summary>
        /// 将参数体值为application/xml请求
        /// utf-8
        /// </summary>
        public XmlContentAttribute()
            : this(Encoding.UTF8)
        {
        }

        /// <summary>
        /// 将参数体作为application/xml请求
        /// </summary>
        /// <param name="codeName">编码</param>
        /// <exception cref="ArgumentNullException"></exception>
        public XmlContentAttribute(string codeName)
            : this(Encoding.GetEncoding(codeName))
        {
        }

        /// <summary>
        /// 将参数体作为application/xml请求
        /// </summary>
        /// <param name="encoding">编码</param>
        /// <exception cref="ArgumentNullException"></exception>
        public XmlContentAttribute(Encoding encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException();
            }
            this.encoding = encoding;
        }

        /// <summary>
        /// 获取http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        protected override HttpContent GetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var formater = context.HttpApiClientConfig.XmlFormatter;
            var content = base.FormatParameter(parameter, formater, this.encoding);
            return new StringContent(content, this.encoding, "application/xml");
        }
    }
}
