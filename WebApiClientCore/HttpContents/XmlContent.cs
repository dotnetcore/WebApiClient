using System;
using System.Net.Http.Headers;
using System.Text;

namespace WebApiClientCore.HttpContents
{
    /// <summary>
    /// 表示 xml 内容
    /// </summary>
    public class XmlContent : BufferContent
    {
        private static readonly MediaTypeHeaderValue defaultMediaType = new(MediaType) { CharSet = Encoding.UTF8.WebName };

        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/xml";

        /// <summary>
        /// xml内容
        /// </summary>
        /// <param name="xml">xml内容</param>
        /// <param name="encoding">编码</param>
        public XmlContent(ReadOnlySpan<char> xml, Encoding encoding)
        {
            encoding.GetBytes(xml, this);
            this.Headers.ContentType = encoding == Encoding.UTF8
                ? defaultMediaType :
                new MediaTypeHeaderValue(MediaType) { CharSet = encoding.WebName };
        }
    }
}