using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示http请求的xml内容
    /// </summary>
    class XmlContent : StringContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/xml";

        /// <summary>
        /// http请求的xml内容
        /// </summary>
        /// <param name="xml">xml内容</param>
        /// <param name="encoding">编码</param>
        public XmlContent(string xml, Encoding encoding)
            : base(xml, encoding, MediaType)
        {
        }
    }
}
