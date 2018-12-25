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
    public class MulitpartFile : IApiParameterable
    {
        /// <summary>
        /// 数据流
        /// </summary>
        private readonly Stream stream;

        /// <summary>
        /// 本机文件路径
        /// </summary>
        private readonly string filePath;

        /// <summary>
        /// 文件好友名称
        /// </summary>
        private readonly string fileName;

        /// <summary>
        /// 上传进度变化事件
        /// </summary>
        public event EventHandler<ProgressEventArgs> UploadProgressChanged;

        /// <summary>
        /// 获取文件好友名称
        /// </summary>
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(this.fileName))
                {
                    return this.fileName;
                }
                return HttpUtility.UrlEncode(this.fileName, Encoding.UTF8);
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
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.fileName = fileName;
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

            this.filePath = localFilePath;
            this.fileName = Path.GetFileName(localFilePath);
        }

        /// <summary>
        /// 执行请求前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        async Task IApiParameterable.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            context.RequestMessage.AddMulitpartFile(this.GetUploadStream(), parameter.Name, this.FileName, this.ContentType);
            await ApiTask.CompletedTask;
        }

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <returns></returns>
        private UploadStream GetUploadStream()
        {
            var inner = this.stream ?? new FileStream(this.filePath, FileMode.Open, FileAccess.Read);
            return new UploadStream(inner, this.RaiseUploadProgressChanged);
        }

        /// <summary>
        /// 触发上传进度变化事件
        /// </summary>
        /// <param name="e"></param>
        private void RaiseUploadProgressChanged(ProgressEventArgs e)
        {
            this.UploadProgressChanged?.Invoke(this, e);
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
            private readonly Action<ProgressEventArgs> eventArgsHandler;

            /// <summary>
            /// 记录当前字节数
            /// </summary>
            private long currentBytes = 0L;

            /// <summary>
            /// 上传数据流
            /// </summary>
            /// <param name="inner">内部流</param>
            /// <param name="eventArgsHandler">进度事件处理者</param>
            /// <exception cref="ArgumentNullException"></exception>
            public UploadStream(Stream inner, Action<ProgressEventArgs> eventArgsHandler)
            {
                this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
                this.eventArgsHandler = eventArgsHandler ?? throw new ArgumentNullException(nameof(eventArgsHandler));

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
            /// 获取数据指针公交车
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
                this.eventArgsHandler.Invoke(args);

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
                this.inner.SetLength(value);
            }

            /// <summary>
            /// 写入数据
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="offset"></param>
            /// <param name="count"></param>
            public override void Write(byte[] buffer, int offset, int count)
            {
                this.inner.Write(buffer, offset, count);
            }
        }
    }
}
