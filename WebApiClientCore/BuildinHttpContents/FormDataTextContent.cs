using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示文本内容
    /// </summary>
    class FormDataTextContent : StringContent
    {
        /// <summary>
        /// 文本内容
        /// </summary>     
        /// <param name="name">名称</param>
        /// <param name="value">文本</param>
        public FormDataTextContent(string name, string? value)
            : base(value ?? string.Empty)
        {
            this.Headers.ContentType = null;
            if (this.Headers.ContentDisposition == null)
            {
                var disposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = $"\"{name}\""
                };
                this.Headers.ContentDisposition = disposition;
            }
        }
    }
}
