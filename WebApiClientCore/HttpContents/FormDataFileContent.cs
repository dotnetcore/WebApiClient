using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApiClientCore.HttpContents
{
    /// <summary>
    /// 表示form-data文件内容
    /// </summary>
    public class FormDataFileContent : StreamContent, ICustomHttpContentConvertable
    {
        /// <summary>
        /// form-data文件内容
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="name">名称</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">文件Mime</param>
        public FormDataFileContent(Stream stream, string name, string? fileName, string? contentType)
            : base(stream)
        {
            if (this.Headers.ContentDisposition == null)
            {
                var disposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = $"\"{name}\"",
                    FileName = $"\"{fileName}\""
                };
                this.Headers.ContentDisposition = disposition;
            }

            if (string.IsNullOrEmpty(contentType) == true)
            {
                contentType = "application/octet-stream";
            }
            this.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }

        /// <summary>
        /// 转换为省略内容的HttpContent
        /// </summary>
        /// <returns></returns>
        public HttpContent ToCustomHttpContext()
        {
            return new EllipsisContent(this);
        }

        /// <summary>
        /// 表示省略内容的文件请求内容
        /// </summary>
        private class EllipsisContent : ByteArrayContent
        {
            /// <summary>
            /// 省略号内容
            /// </summary>
            private static readonly byte[] content = new[] { (byte)'.', (byte)'.', (byte)'.' };

            /// <summary>
            /// 省略内容的文件请求内容
            /// </summary>
            /// <param name="fileContent">文件内容</param>
            public EllipsisContent(FormDataFileContent fileContent)
                : base(content)
            {
                this.Headers.ContentDisposition = fileContent.Headers.ContentDisposition;
                this.Headers.ContentType = fileContent.Headers.ContentType;
            }
        }
    }
}
