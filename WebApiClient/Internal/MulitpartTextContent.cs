using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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
            : base(value == null ? string.Empty : value)
        {
            if (this.Headers.ContentDisposition == null)
            {
                var disposition = new ContentDispositionHeaderValue("form-data");
                disposition.Name = string.Format("\"{0}\"", name);
                this.Headers.ContentDisposition = disposition;
            }
            this.Headers.Remove("Content-Type");
        }
    }
}
