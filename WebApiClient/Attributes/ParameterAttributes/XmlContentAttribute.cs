using System;
using System.Net.Http;
using System.Text;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 使用XmlFormatter序列化参数值得到的json文本作为application/xml请求
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
            : this(Encoding.GetEncoding(codeName ?? throw new ArgumentNullException(nameof(codeName))))
        {
        }

        /// <summary>
        /// 将参数体作为application/xml请求
        /// </summary>
        /// <param name="encoding">编码</param>
        /// <exception cref="ArgumentNullException"></exception>
        private XmlContentAttribute(Encoding encoding)
        {
            this.encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }


        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        protected override void SetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var formatter = context.HttpApiConfig.XmlFormatter;
            var xml = formatter.Serialize(parameter.Value, this.encoding);

            context.RequestMessage.Content = new XmlContent(xml, this.encoding);
        }
    }
}
