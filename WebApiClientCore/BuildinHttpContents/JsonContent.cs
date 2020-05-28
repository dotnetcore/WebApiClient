using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示http请求的json内容
    /// 从对象的json序列化到HttpContext发送过程为0拷贝
    /// </summary>
    class JsonContent : HttpContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/json";

        /// <summary>
        /// json内容的buffer
        /// </summary>
        private readonly BufferWriter<byte> jsonBufferWriter;

        /// <summary>
        /// http请求的json内容
        /// </summary>
        /// <param name="jsonBufferWriter">json内容的buffer</param>
        public JsonContent(BufferWriter<byte> jsonBufferWriter)
        {
            this.Headers.ContentType = new MediaTypeHeaderValue(MediaType);
            this.jsonBufferWriter = jsonBufferWriter;
        }

        /// <summary>
        /// 创建只读流
        /// </summary>
        /// <returns></returns>
        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            var buffer = this.jsonBufferWriter.GetWrittenSpan().ToArray();
            var readStream = new MemoryStream(buffer, 0, buffer.Length, writable: false);
            return Task.FromResult<Stream>(readStream);
        }

        /// <summary>
        /// 序列化到目标流中
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var memory = this.jsonBufferWriter.GetWrittenMemory();
            await stream.WriteAsync(memory).ConfigureAwait(false);
        }

        /// <summary>
        /// 计算内容长度
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        protected override bool TryComputeLength(out long length)
        {
            length = this.jsonBufferWriter.WrittenCount;
            return true;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.jsonBufferWriter.Dispose();
            base.Dispose(disposing);
        }
    }
}
