using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示文件内容
    /// </summary>
    class MulitpartFileContent : StreamContent
    {
        /// <summary>
        /// 文件内容
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="name">名称</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">文件Mime</param>
        public MulitpartFileContent(Stream stream, string name, string fileName, string contentType)
            : base(stream)
        {
            if (this.Headers.ContentDisposition == null)
            {
                var disposition = new ContentDispositionHeaderValue("form-data");
                disposition.Name = string.Format("\"{0}\"", name);
                disposition.FileName = string.Format("\"{0}\"", fileName);
                this.Headers.ContentDisposition = disposition;
            }

            if (string.IsNullOrEmpty(contentType))
            {
                contentType = "application/octet-stream";
            }
            this.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }
    }
}
