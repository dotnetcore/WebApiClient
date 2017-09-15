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
    /// 标准协议的multipart/form-data
    /// </summary>
    class StMultipartFormDataContent : MultipartContent
    {
        private const string formData = "form-data";

        /// <summary>
        /// 标准协议的multipart/form-data
        /// </summary>
        public StMultipartFormDataContent()
            : base(formData)
        {
        }

        /// <summary>
        /// 标准协议的multipart/form-data
        /// </summary>
        /// <param name="boundary">分隔符号</param>
        public StMultipartFormDataContent(string boundary)
            : base(formData, boundary)
        {
        }

        /// <summary>
        /// 添加内容
        /// </summary>
        /// <param name="content">http内容</param>
        public override void Add(HttpContent content)
        {
            base.Add(content);
        }

        /// <summary>
        /// 添加文本内容
        /// </summary>
        /// <param name="content">文本内容</param>
        /// <param name="name">名称</param>
        public void Add(StringContent content, string name)
        {
            if (content.Headers.ContentDisposition == null)
            {
                var disposition = new ContentDispositionHeaderValue(formData);
                disposition.Name = string.Format("\"{0}\"", name);
                content.Headers.ContentDisposition = disposition;
            }
            content.Headers.Remove("Content-Type");
            this.Add(content);
        }

        /// <summary>
        /// 添加文件内容
        /// </summary>
        /// <param name="content">文件内容</param>
        /// <param name="name">名称</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contenType">文件Mime</param>
        public void Add(StreamContent content, string name, string fileName, string contenType = "application/octet-stream")
        {
            if (content.Headers.ContentDisposition == null)
            {
                var disposition = new ContentDispositionHeaderValue("form-data");
                disposition.Name = string.Format("\"{0}\"", name);
                disposition.FileName = string.Format("\"{0}\"", fileName);
                content.Headers.ContentDisposition = disposition;
            }
            content.Headers.ContentType = new MediaTypeHeaderValue(contenType);
            this.Add(content);
        }
    }
}
