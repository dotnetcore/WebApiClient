using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示键值对表单内容
    /// </summary>
    class FormContent : HttpContent
    {
        /// <summary>
        /// 用于保存表单内容
        /// </summary>
        private readonly MemoryStream stream = new MemoryStream();

        /// <summary>
        /// 默认的http编码
        /// </summary>
        private static readonly Encoding uriEncoding = Encoding.GetEncoding(28591);

        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/x-www-form-urlencoded";


        /// <summary>
        /// 键值对表单内容
        /// </summary>
        public FormContent()
        {
            this.Headers.ContentType = new MediaTypeHeaderValue(MediaType);
        }

        /// <summary>
        /// 从HttpContent转换得到
        /// </summary>
        /// <param name="httpContent">httpContent实例</param>
        /// <param name="disposeHttpContent">是否释放httpContent</param>
        /// <returns></returns>
        public static async Task<FormContent> FromHttpContentAsync(HttpContent httpContent, bool disposeHttpContent = true)
        {
            if (httpContent == null)
            {
                return new FormContent();
            }

            if (httpContent is FormContent formContent)
            {
                return formContent;
            }

            formContent = new FormContent();
            var byteArray = await httpContent.ReadAsByteArrayAsync().ConfigureAwait(false);
            await formContent.AddRawFormAsync(byteArray).ConfigureAwait(false);

            if (disposeHttpContent == true)
            {
                httpContent.Dispose();
            }
            return formContent;
        }

        /// <summary>
        /// 添加键值对
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public async Task AddFormFieldAsync(IEnumerable<KeyValue> keyValues)
        {
            if (keyValues == null)
            {
                return;
            }

            foreach (var item in keyValues)
            {
                var key = HttpUtility.UrlEncodeToBytes(item.Key);
                var value = item.Value == null ? null : HttpUtility.UrlEncodeToBytes(item.Value);
                await this.AddRawFormAsync(key, value).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// 添加已编码的原始内容表单
        /// </summary>
        /// <param name="form">表单内容</param>
        /// <returns></returns>
        public async Task AddRawFormAsync(string? form)
        {
            if (form == null)
            {
                return;
            }

            var formBytes = uriEncoding.GetBytes(form);
            await this.AddRawFormAsync(formBytes).ConfigureAwait(false);
        }


        /// <summary>
        /// 添加已编码的二进制数据内容
        /// </summary>
        /// <param name="form">数据内容</param>
        /// <returns></returns>
        private async Task AddRawFormAsync(byte[] form)
        {
            if (form == null || form.Length == 0)
            {
                return;
            }

            if (this.stream.Length > 0)
            {
                this.stream.WriteByte((byte)'&');
            }
            await this.stream.WriteAsync(form, 0, form.Length).ConfigureAwait(false);
        }

        /// <summary>
        /// 添加已编码的二进制数据内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private async Task AddRawFormAsync(byte[] key, byte[]? value)
        {
            if (this.stream.Length > 0)
            {
                this.stream.WriteByte((byte)'&');
            }

            await this.stream.WriteAsync(key, 0, key.Length).ConfigureAwait(false);
            this.stream.WriteByte((byte)'=');
            if (value != null && value.Length > 0)
            {
                await this.stream.WriteAsync(value, 0, value.Length).ConfigureAwait(false);
            }
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
            await this.stream.CopyToAsync(stream).ConfigureAwait(false);
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
