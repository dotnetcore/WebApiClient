using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace WebApiClient
{
    /// <summary>
    /// 表示文本内容
    /// </summary>
    class MulitpartTextContent : StringContent
    {
        /// <summary>
        /// 文本内容
        /// </summary>     
        /// <param name="name">名称</param>
        /// <param name="value">文本</param>
        public MulitpartTextContent(string name, string value)
            : base(EncodingValue(value))
        {
            if (this.Headers.ContentDisposition == null)
            {
                var disposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = $"\"{name}\""
                };
                this.Headers.ContentDisposition = disposition;
            }
            this.Headers.Remove("Content-Type");
        }

        /// <summary>
        /// 对值进行编码
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        private static string EncodingValue(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            return HttpUtility.UrlEncode(value, Encoding.UTF8);
        }
    }
}
