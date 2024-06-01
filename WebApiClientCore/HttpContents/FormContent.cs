using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Internals;
using WebApiClientCore.Serialization;

namespace WebApiClientCore.HttpContents
{
    /// <summary>
    /// 表示键值对表单内容
    /// </summary>
    public class FormContent : HttpContent
    {
        /// <summary>
        /// buffer写入器
        /// </summary>
        private readonly RecyclableBufferWriter<byte> bufferWriter = new();

        /// <summary>
        /// 默认的http编码
        /// </summary>
        private static readonly Encoding httpEncoding = Encoding.GetEncoding(28591);

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
        /// 键值对表单内容
        /// </summary>
        /// <param name="keyValues">键值对</param> 
        public FormContent(IEnumerable<KeyValue> keyValues)
        {
            this.AddFormField(keyValues);
            this.Headers.ContentType = new MediaTypeHeaderValue(MediaType);
        }

        /// <summary>
        /// 键值对表单内容
        /// </summary>
        /// <param name="value">模型对象值</param>
        /// <param name="options">序列化选项</param>
        public FormContent(object? value, KeyValueSerializerOptions? options)
        {
            if (value != null)
            {
                var keyValues = KeyValueSerializer.Serialize(nameof(value), value, options);
                this.AddFormField(keyValues);
            }
            this.Headers.ContentType = new MediaTypeHeaderValue(MediaType);
        }

        /// <summary>
        /// 添加键值对
        /// </summary>
        /// <param name="keyValues">键值对</param>
        public void AddFormField(KeyValue keyValues)
        {
            this.AddFormField(Enumerable.Repeat(keyValues, 1));
        }

        /// <summary>
        /// 添加键值对
        /// </summary>
        /// <param name="keyValues">键值对</param>
        public void AddFormField(IEnumerable<KeyValue> keyValues)
        {
            this.EnsureNotBuffered();

            if (keyValues == null)
            {
                return;
            }

            foreach (var item in keyValues)
            {
                if (this.bufferWriter.WrittenCount > 0)
                {
                    this.bufferWriter.Write((byte)'&');
                }

                HttpUtil.UrlEncode(item.Key, this.bufferWriter);
                this.bufferWriter.Write((byte)'=');
                HttpUtil.UrlEncode(item.Value, this.bufferWriter);
            }
        }

        /// <summary>
        /// 添加已编码的原始内容表单
        /// </summary>
        /// <param name="encodedForm">表单内容</param>
        public void AddForm(string? encodedForm)
        {
            if (encodedForm == null)
            {
                return;
            }

            var formBytes = httpEncoding.GetBytes(encodedForm);
            this.AddForm(formBytes);
        }

        /// <summary>
        /// 添加已编码的二进制数据内容
        /// </summary>
        /// <param name="encodedForm">数据内容</param>
        public void AddForm(ReadOnlySpan<byte> encodedForm)
        {
            this.EnsureNotBuffered();

            if (encodedForm.IsEmpty == true)
            {
                return;
            }

            if (this.bufferWriter.WrittenCount > 0)
            {
                this.bufferWriter.Write((byte)'&');
            }
            this.bufferWriter.Write(encodedForm);
        }

        /// <summary>
        /// 计算内容长度
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        protected override bool TryComputeLength(out long length)
        {
            length = this.bufferWriter.WrittenCount;
            return true;
        }

        /// <summary>
        /// 创建只读流
        /// </summary>
        /// <returns></returns>
        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            var segment = this.bufferWriter.WrittenSegment;
            var readStream = new MemoryStream(segment.Array!, segment.Offset, segment.Count, writable: false);
            return Task.FromResult<Stream>(readStream);
        }

        /// <summary>
        /// 序列化到目标流中
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            var memory = this.bufferWriter.WrittenMemory;
            return stream.WriteAsync(memory).AsTask();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.bufferWriter.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// 从HttpContent转换得到
        /// </summary>
        /// <param name="httpContent">httpContent实例</param>
        /// <param name="disposeHttpContent">是否释放httpContent</param>
        /// <returns></returns>
        public static async Task<FormContent> ParseAsync(HttpContent? httpContent, bool disposeHttpContent = true)
        {
            if (httpContent == null)
            {
                return new FormContent();
            }

            if (httpContent is FormContent formContent && formContent.IsBuffered() == false)
            {
                return formContent;
            }

            formContent = new FormContent();
            var byteArray = await httpContent.ReadAsByteArrayAsync().ConfigureAwait(false);
            formContent.AddForm(byteArray);

            if (disposeHttpContent == true)
            {
                httpContent.Dispose();
            }
            return formContent;
        }
    }
}
