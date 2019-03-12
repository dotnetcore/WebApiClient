using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Parameterables
{
    /// <summary>
    /// 表示将自身作为multipart/form-data的一个文件项
    /// </summary>
    [DebuggerDisplay("FileName = {FileName}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public class MulitpartFile : Stream, IApiParameterable
    {
        /// <summary>
        /// 数据流
        /// </summary>
        private readonly Stream innerStream;

        /// <summary>
        /// 指示是否可以dispose传入的stream
        /// </summary>
        private readonly bool disposeInnerStream;

        /// <summary>
        /// 总字节数
        /// </summary>
        private readonly long? totalBytes;

        /// <summary>
        /// 记录当前字节数
        /// </summary>
        private long currentBytes = 0L;


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
        /// 返回是否可读
        /// </summary>
        public override bool CanRead
        {
            get => this.innerStream.CanRead;
        }

        /// <summary>
        /// 返回是否可探索位置
        /// </summary>
        public override bool CanSeek
        {
            get => this.innerStream.CanSeek;
        }

        /// <summary>
        /// 返回是否可写
        /// </summary>
        public override bool CanWrite
        {
            get => this.innerStream.CanWrite;
        }

        /// <summary>
        /// 返回数据流长度
        /// </summary>
        public override long Length
        {
            get => this.innerStream.Length;
        }

        /// <summary>
        /// 返回数据流当前的指针位置
        /// </summary>
        public override long Position
        {
            get => this.innerStream.Position;
            set => this.innerStream.Position = value;
        }

        /// <summary>
        /// multipart/form-data的一个文件项
        /// </summary>
        /// <param name="localFilePath">本地文件路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public MulitpartFile(string localFilePath)
            : this(new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4 * 1024, true),
                  Path.GetFileName(localFilePath), true)
        {
        }

        /// <summary>
        /// 将自身作为multipart/form-data的一个文件项
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="fileName">文件友好名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MulitpartFile(byte[] buffer, string fileName) :
            this(new MemoryStream(buffer ?? throw new ArgumentNullException(nameof(buffer))), fileName, true)
        {
        }

        /// <summary>
        /// 将自身作为multipart/form-data的一个文件项
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="fileName">文件友好名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MulitpartFile(Stream stream, string fileName)
            : this(stream, fileName, false)
        {
        }

        /// <summary>
        /// 将自身作为multipart/form-data的一个文件项
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="fileName">文件友好名称</param>
        /// <param name="disposeStream">指示是否可以dispose传入的stream</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MulitpartFile(Stream stream, string fileName, bool disposeStream)
        {
            this.innerStream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.disposeInnerStream = disposeStream;
            this.FileName = fileName;

            try
            {
                this.totalBytes = this.Length;
            }
            catch (Exception) { }
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
            context.RequestMessage.AddMulitpartFile(this, parameter.Name, this.EncodedFileName, this.ContentType);
            await ApiTask.CompletedTask;
        }


        /// <summary>
        /// 冲刷缓冲
        /// </summary>
        public sealed override void Flush()
        {
            this.innerStream.Flush();
        }

        /// <summary>
        /// 冲刷缓冲
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public sealed override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await this.innerStream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 定位到指定指针位置
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <param name="origin">定位源</param>
        /// <returns></returns>
        public sealed override long Seek(long offset, SeekOrigin origin)
        {
            return this.innerStream.Seek(offset, origin);
        }

        /// <summary>
        /// 设置流的长度
        /// </summary>
        /// <param name="value">长度值</param>
        public sealed override void SetLength(long value)
        {
            this.innerStream.SetLength(value);
        }

#if !NETSTANDARD1_3
        /// <summary>
        /// 开始读取数据流
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        /// <param name="offset">缓冲区偏移量</param>
        /// <param name="count">读取的大小</param>
        /// <param name="callback">回调</param>
        /// <param name="state">用户状态数据</param>
        /// <returns></returns>
        public sealed override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return this.innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// 读取完成
        /// </summary>
        /// <param name="asyncResult">异步结果</param>
        /// <returns></returns>
        public sealed override int EndRead(IAsyncResult asyncResult)
        {
            var length = this.innerStream.EndRead(asyncResult);
            this.OnRead(length);
            return length;
        }

        /// <summary>
        /// 关闭流
        /// </summary>
        public sealed override void Close()
        {
            this.innerStream.Close();
        }
#endif

        /// <summary>
        /// 读取流到缓冲区
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        /// <param name="offset">偏移量</param>
        /// <param name="count">读取的大小</param>
        /// <returns></returns>
        public sealed override int Read(byte[] buffer, int offset, int count)
        {
            var length = this.innerStream.Read(buffer, offset, count);
            this.OnRead(length);
            return length;
        }

        /// <summary>
        /// 读取流到缓冲区
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        /// <param name="offset">偏移量</param>
        /// <param name="count">读取的大小</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public sealed override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var length = await this.innerStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
            this.OnRead(length);
            return length;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="length">数据长度</param>
        protected virtual void OnRead(int length)
        {
            var isCompleted = length == 0;
            this.currentBytes = this.currentBytes + length;

            var args = new ProgressEventArgs(this.currentBytes, this.totalBytes, isCompleted);
            this.OnUploadProgressChanged(args);
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
        /// 写入数据到流
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移量</param>
        /// <param name="count">写入的长度</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.innerStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.disposeInnerStream)
            {
                this.innerStream.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 调试视图
        /// </summary>
        private class DebugView
        {
            /// <summary>
            /// 查看的对象
            /// </summary>
            private readonly MulitpartFile target;

            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="target">查看的对象</param>
            public DebugView(MulitpartFile target)
            {
                this.target = target;
            }

            /// <summary>
            /// 获取文件好友名称
            /// </summary>
            public string FileName
            {
                get => this.target.FileName;
            }

            /// <summary>
            /// 获取编码后的文件好友名称
            /// </summary>
            public virtual string EncodedFileName
            {
                get => this.target.EncodedFileName;
            }

            /// <summary>
            /// 获取或设置文件的Mime
            /// </summary>
            public string ContentType
            {
                get => this.target.ContentType;
            }

            /// <summary>
            /// 返回数据流长度
            /// </summary>
            public long Length
            {
                get => this.target.Length;
            } 
        }
    }
}
