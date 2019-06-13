using System;
using System.Text;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 使用XmlFormatter序列化参数值得到的xml文本作为application/xml请求
    /// </summary>
    public class XmlContentAttribute : HttpContentAttribute, IEncodingable
    {
        /// <summary>
        /// 编码方式
        /// </summary>
        private Encoding encoding = System.Text.Encoding.UTF8;

        /// <summary>
        /// 获取或设置编码名称
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public string Encoding
        {
            get => this.encoding.WebName;
            set => this.encoding = System.Text.Encoding.GetEncoding(value);
        }

        /// <summary>
        /// 序列化参数值得到的xml文本作为application/xml请求    
        /// </summary>
        public XmlContentAttribute()
        {
        }

        /// <summary>
        /// 序列化参数值得到的xml文本作为application/xml请求
        /// </summary>
        /// <param name="encoding">编码</param>
        /// <exception cref="ArgumentNullException"></exception>
        public XmlContentAttribute(string encoding)
        {
            this.Encoding = encoding;
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
