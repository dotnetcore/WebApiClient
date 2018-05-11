using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示键值对表单内容
    /// </summary>
    class UrlEncodedContent : HttpContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/x-www-form-urlencoded";

        /// <summary>
        /// 用于保存表单内容
        /// </summary>
        private readonly MemoryStream stream = new MemoryStream();

        /// <summary>
        /// 键值对表单内容
        /// </summary>
        /// <param name="content">原始表单</param>
        /// <param name="disposeContent">是否要释放原始表单</param>
        public UrlEncodedContent(HttpContent content, bool disposeContent = true)
        {
            if (content != null)
            {
                content.CopyToAsync(this.stream);
                if (disposeContent == true)
                {
                    content.Dispose();
                }
            }
            this.Headers.ContentType = new MediaTypeHeaderValue(MediaType);
        }

        /// <summary>
        /// 添加字段到内存流
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public async Task AddFormFieldAsync(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            if (keyValues == null || keyValues.Any() == false)
            {
                return;
            }

            var bytes = this.EncodedKeyValues(keyValues);
            await this.stream.WriteAsync(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 计算键值对的字节组
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        private byte[] EncodedKeyValues(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            var encoding = Encoding.UTF8;
            var parameters =
                from kv in keyValues
                let value = HttpUtility.UrlEncode(kv.Value, encoding)
                select $"{kv.Key }={value}";

            var parameterString = string.Join("&", parameters);
            if (this.stream.Length > 0)
            {
                parameterString = "&" + parameterString;
            }
            return encoding.GetBytes(parameterString);
        }

        /// <summary>
        /// 计算内容长度
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        protected override bool TryComputeLength(out long length)
        {
            length = this.stream.Length;
            return true;
        }

        /// <summary>
        /// 创建只读流
        /// </summary>
        /// <returns></returns>
        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            var buffer = this.stream.ToArray();
            var readStream = new MemoryStream(buffer, 0, buffer.Length, writable: false);
            return Task.FromResult<Stream>(readStream);
        }

        /// <summary>
        /// 序列化到目标流中
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var position = this.stream.Position;
            this.stream.Position = 0;
            await this.stream.CopyToAsync(stream);
            this.stream.Position = position;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.stream.Dispose();
            base.Dispose(disposing);
        }
    }
}
