using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示http请求的json内容
    /// </summary>
    class JsonContent : StringContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/json";

        /// <summary>
        /// http请求的json内容
        /// </summary>
        /// <param name="json">json内容</param>
        /// <param name="encoding">编码</param>
        public JsonContent(string json, Encoding encoding)
            : base(json, encoding, MediaType)
        {
        }
    }
}
