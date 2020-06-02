using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore.Parameters
{
    /// <summary>
    /// 表示form-data的一个文件项
    /// </summary>
    [DebuggerDisplay("FileName = {FileName}")]
    public class FormDataFile : IApiParameter
    {
        /// <summary>
        /// 数据流创建委托
        /// </summary>
        private readonly Func<Stream> streamFactory;

        /// <summary>
        /// 获取文件好友名称
        /// </summary>
        public string? FileName { get; }

        /// <summary>
        /// 获取或设置文件的Mime
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// 获取编码后的文件好友名称
        /// </summary>
        public virtual string? EncodedFileName => HttpUtility.UrlEncode(this.FileName, Encoding.UTF8);

        /// <summary>
        /// form-data的一个文件项
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public FormDataFile(string filePath)
            : this(new FileInfo(filePath))
        {
        }

        /// <summary>
        /// form-data的一个文件项
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        public FormDataFile(FileInfo fileInfo)
            : this(() => fileInfo.OpenRead(), fileInfo.Name)
        {
        }

        /// <summary>
        /// form-data的一个文件项
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="fileName">文件友好名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FormDataFile(byte[] buffer, string? fileName) :
            this(() => new MemoryStream(buffer), fileName)
        {
        }

        /// <summary>
        /// form-data的一个文件项        
        /// 不支持多线程并发请求
        /// 如果多次请求则要求数据流必须支持倒带读取
        /// </summary>
        /// <param name="seekableStream">数据流</param>
        /// <param name="fileName">文件友好名称</param>
        /// <returns></returns>
        public FormDataFile(Stream seekableStream, string? fileName)
            : this(() => new AutoRewindStream(seekableStream), fileName)
        {
        }

        /// <summary>
        /// form-data的一个文件项
        /// </summary>
        /// <param name="streamFactory">数据流的创建委托</param>
        /// <param name="fileName">文件友好名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FormDataFile(Func<Stream> streamFactory, string? fileName)
        {
            this.streamFactory = streamFactory ?? throw new ArgumentNullException(nameof(streamFactory));
            this.FileName = fileName;
        }

        /// <summary>
        /// 执行请求前
        /// </summary>
        /// <param name="context">上下文</param>
        public Task OnRequestAsync(ApiParameterContext context)
        {
            var stream = this.streamFactory();
            context.HttpContext.RequestMessage.AddFormDataFile(stream, context.Parameter.Name, this.EncodedFileName, this.ContentType);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 表示发送后自动倒带的流
        /// </summary>
        private class AutoRewindStream : Stream
        {
            private readonly Stream stream;
            private readonly long? defaultPosition;

            /// <summary>
            /// 获取是否可读
            /// </summary>
            public override bool CanRead => this.stream.CanRead;

            /// <summary>
            /// 获取是否可定位
            /// </summary>
            public override bool CanSeek => this.stream.CanSeek;

            /// <summary>
            /// 获取是否可写
            /// </summary>
            public override bool CanWrite => this.stream.CanWrite;

            /// <summary>
            /// 获取长度
            /// </summary>
            public override long Length => this.stream.Length;

            /// <summary>
            /// 获取位置
            /// </summary>
            public override long Position
            {
                get => this.stream.Position;
                set => this.stream.Position = value;
            }

            /// <summary>
            /// 发送后自动倒带的流
            /// </summary>
            /// <param name="seekableStream">包装的流</param>
            /// <exception cref="ArgumentNullException"></exception>
            public AutoRewindStream(Stream seekableStream)
            {
                this.stream = seekableStream ?? throw new ArgumentNullException(nameof(seekableStream));
                if (seekableStream.CanSeek == true)
                {
                    this.defaultPosition = seekableStream.Position;
                }
            }

            /// <summary>
            /// 刷新
            /// </summary>
            public override void Flush()
            {
                this.stream.Flush();
            }

            /// <summary>
            /// 读取
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="offset"></param>
            /// <param name="count"></param>
            /// <returns></returns>
            public override int Read(byte[] buffer, int offset, int count)
            {
                return this.stream.Read(buffer, offset, count);
            }

            /// <summary>
            /// 定位
            /// </summary>
            /// <param name="offset"></param>
            /// <param name="origin"></param>
            /// <returns></returns>
            public override long Seek(long offset, SeekOrigin origin)
            {
                return this.stream.Seek(offset, origin);
            }

            /// <summary>
            /// 设置长度
            /// </summary>
            /// <param name="value"></param>
            public override void SetLength(long value)
            {
                this.stream.SetLength(value);
            }

            /// <summary>
            /// 写入
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="offset"></param>
            /// <param name="count"></param>
            public override void Write(byte[] buffer, int offset, int count)
            {
                this.stream.Write(buffer, offset, count);
            }

            /// <summary>
            /// 异步复制
            /// </summary>
            /// <param name="destination"></param>
            /// <param name="bufferSize"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
            {
                return this.stream.CopyToAsync(destination, bufferSize, cancellationToken);
            }

            /// <summary>
            /// 不释放资源
            /// 而是尝试倒带内置的stream以支持重新读取
            /// </summary>
            /// <param name="disposing"></param>
            protected override void Dispose(bool disposing)
            {
                if (this.defaultPosition != null)
                {
                    this.stream.Position = this.defaultPosition.Value;
                }
            }
        }
    }
}
