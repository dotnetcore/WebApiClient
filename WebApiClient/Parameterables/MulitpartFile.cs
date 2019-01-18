using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Parameterables
{
    /// <summary>
    /// 表示将自身作为multipart/form-data的一个文件项
    /// </summary>
    [DebuggerDisplay("FileName = {FileName}")]
    public class MulitpartFile : IApiParameterable, IDisposable
    {
        /// <summary>
        /// 数据流
        /// </summary>
        private readonly Lazy<Stream> stream;

        /// <summary>
        /// 上传进度变化事件
        /// </summary>
        public event EventHandler<ProgressEventArgs> UploadProgressChanged;

        /// <summary>
        /// 获取文件好友名称
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 获取编码后的文件好友名称
        /// </summary>
        public virtual string EncodedFileName
        {
            get
            {
                return HttpUtility.UrlEncode(this.FileName, Encoding.UTF8);
            }
        }

        /// <summary>
        /// 获取或设置文件的Mime
        /// </summary>
        public string ContentType { get; set; } = "application/octet-stream";

        /// <summary>
        /// 将自身作为multipart/form-data的一个文件项
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="fileName">文件友好名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MulitpartFile(byte[] buffer, string fileName) :
            this(new MemoryStream(buffer ?? throw new ArgumentNullException(nameof(buffer))), fileName)
        {
        }

        /// <summary>
        /// 将自身作为multipart/form-data的一个文件项
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="fileName">文件友好名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MulitpartFile(Stream stream, string fileName)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            this.stream = new Lazy<Stream>(() => stream);
            this.FileName = fileName;
        }

        /// <summary>
        /// multipart/form-data的一个文件项
        /// </summary>
        /// <param name="localFilePath">本地文件路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public MulitpartFile(string localFilePath)
        {
            if (string.IsNullOrEmpty(localFilePath))
            {
                throw new ArgumentNullException(nameof(localFilePath));
            }

            if (File.Exists(localFilePath) == false)
            {
                throw new FileNotFoundException(localFilePath);
            }

            this.stream = new Lazy<Stream>(() => new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024 * 4, true));
            this.FileName = Path.GetFileName(localFilePath);
        }

        /// <summary>
        /// 执行请求前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        async Task IApiParameterable.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            await this.BeforeRequestAsync(context, parameter).ConfigureAwait(false);
        }

        /// <summary>
        /// 执行请求前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        protected virtual async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var uploadStream = this.UploadProgressChanged == null ?
                this.stream.Value :
                new UploadStream(this.stream.Value, this.OnUploadProgressChanged);

            context.RequestMessage.AddMulitpartFile(uploadStream, parameter.Name, this.EncodedFileName, this.ContentType);
            await ApiTask.CompletedTask;
        }

        /// <summary>
        /// 触发上传进度变化事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUploadProgressChanged(ProgressEventArgs e)
        {
            this.UploadProgressChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this.stream.IsValueCreated == true)
            {
                this.stream.Value.Dispose();
            }
        }

        /// <summary>
        /// 表示上传数据流
        /// </summary>
        private class UploadStream : Stream
        {
            /// <summary>
            /// 内部流
            /// </summary>
            private readonly Stream inner;

            /// <summary>
            /// 总字节数
            /// </summary>
            private readonly long? totalBytes;

            /// <summary>
            /// 进度事件处理者
            /// </summary>
            private readonly Action<ProgressEventArgs> progressChangedHandler;

            /// <summary>
            /// 记录当前字节数
            /// </summary>
            private long currentBytes = 0L;

            /// <summary>
            /// 上传数据流
            /// </summary>
            /// <param name="inner">内部流</param>
            /// <param name="progressChangedHandler">进度事件处理者</param>
            /// <exception cref="ArgumentNullException"></exception>
            public UploadStream(Stream inner, Action<ProgressEventArgs> progressChangedHandler)
            {
                this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
                this.progressChangedHandler = progressChangedHandler ?? throw new ArgumentNullException(nameof(progressChangedHandler));

                try
                {
                    this.totalBytes = inner.Length;
                }
                catch (Exception) { }
            }

            /// <summary>
            /// 获取是否可读
            /// </summary>
            public override bool CanRead => this.inner.CanRead;

            /// <summary>
            /// 获取是否可定位
            /// </summary>
            public override bool CanSeek => this.inner.CanSeek;

            /// <summary>
            /// 获取是否可写
            /// </summary>
            public override bool CanWrite => this.inner.CanWrite;

            /// <summary>
            /// 获取数据长度
            /// </summary>
            public override long Length => this.inner.Length;

            /// <summary>
            /// 获取数据指针位置
            /// </summary>
            public override long Position
            {
                get => this.inner.Position;
                set => this.inner.Position = value;
            }

            /// <summary>
            /// 冲刷
            /// </summary>
            public override void Flush()
            {
                this.inner.Flush();
            }

            /// <summary>
            /// 读取数据
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="offset"></param>
            /// <param name="count"></param>
            /// <returns></returns>
            public override int Read(byte[] buffer, int offset, int count)
            {
                var length = this.inner.Read(buffer, offset, count);
                var isCompleted = length == 0;

                this.currentBytes = this.currentBytes + length;
                var args = new ProgressEventArgs(this.currentBytes, this.totalBytes, isCompleted);
                this.progressChangedHandler.Invoke(args);

                return length;
            }

            /// <summary>
            /// 定位
            /// </summary>
            /// <param name="offset"></param>
            /// <param name="origin"></param>
            /// <returns></returns>
            public override long Seek(long offset, SeekOrigin origin)
            {
                return this.inner.Seek(offset, origin);
            }

            /// <summary>
            /// 设置长度
            /// </summary>
            /// <param name="value"></param>
            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// 写入数据
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="offset"></param>
            /// <param name="count"></param>
            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// 释放资源
            /// </summary>
            /// <param name="disposing"></param>
            protected override void Dispose(bool disposing)
            {
                this.inner.Dispose();
                base.Dispose(disposing);
            }
        }
    }
}
