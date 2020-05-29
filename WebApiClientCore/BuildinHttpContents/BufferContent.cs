using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示BufferWriter优化的HttpContent
    /// </summary>
    class BufferContent : HttpContent, IBufferWriter<byte>
    {
        /// <summary>
        /// buffer
        /// </summary>
        private readonly BufferWriter<byte> bufferWriter = new BufferWriter<byte>();

        /// <summary>
        /// BufferWriter优化的HttpContent
        /// </summary> 
        public BufferContent(string mediaType)
        {
            this.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        }

        /// <summary>
        /// 设置向前推进
        /// </summary>
        /// <param name="count"></param>
        public void Advance(int count)
        {
            this.bufferWriter.Advance(count);
        }

        /// <summary>
        /// 返回用于写入数据的Memory
        /// </summary>
        /// <param name="sizeHint"></param>
        /// <returns></returns>
        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            return this.bufferWriter.GetMemory(sizeHint);
        }

        /// <summary>
        /// 返回用于写入数据的Span
        /// </summary>
        /// <param name="sizeHint"></param>
        /// <returns></returns>
        public Span<byte> GetSpan(int sizeHint = 0)
        {
            return this.bufferWriter.GetSpan(sizeHint);
        }

        /// <summary>
        /// 创建只读流
        /// </summary>
        /// <returns></returns>
        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            var buffer = this.bufferWriter.GetWrittenSpan().ToArray();
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
            var memory = this.bufferWriter.GetWrittenMemory();
            await stream.WriteAsync(memory).ConfigureAwait(false);
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
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.bufferWriter.Dispose();
            base.Dispose(disposing);
        }
    }
}
