using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示Http响应的文件
    /// 可以声明为接口的返回类型
    /// </summary>
    [DebuggerDisplay("FileName = {FileName}")]
    public class HttpResponseFile : HttpResponseWrapper
    {
        /// <summary>
        /// 下载进度变化事件
        /// </summary>
        public event EventHandler<ProgressEventArgs> DownloadProgressChanged;

        /// <summary>
        /// 获取响应的友好文件名称
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// 获取文件的大小
        /// </summary>
        public long? FileSize { get; }

        /// <summary>
        /// 获取文件类型
        /// </summary>
        public string MediaType { get; }

        /// <summary>
        /// Http响应的文件
        /// </summary>
        /// <param name="response">响应消息</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpResponseFile(HttpResponseMessage response)
            : base(response)
        {
            var headers = response.Content.Headers;
            this.FileSize = headers.ContentLength;
            this.FileName = headers.ContentDisposition?.FileName;
            this.MediaType = headers.ContentType?.MediaType;
        }
        /// <summary>
        /// 保存到指定路径
        /// </summary>
        /// <param name="filePath">文件路径和文件名</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public async Task SaveAsAsync(string filePath)
        {
            await this.SaveAsAsync(filePath, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// 保存到指定路径
        /// </summary>
        /// <param name="filePath">文件路径和文件名</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns></returns>
        public async Task SaveAsAsync(string filePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(filePath) == true)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var dir = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                await this.SaveAsAsync(fileStream, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 保存到目标流
        /// </summary>
        /// <param name="stream">流</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public async Task SaveAsAsync(Stream stream)
        {
            await this.SaveAsAsync(stream, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// 保存到目标流
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns></returns>
        public async Task SaveAsAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (stream.CanWrite == false)
            {
                throw new ArgumentException(nameof(stream) + " cannot be write", nameof(stream));
            }

            var current = 0L;
            var buffer = new byte[8 * 1024];
            var sourceStream = await this.HttpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);

            while (true)
            {
                var length = await sourceStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                var isCompleted = length == 0;

                current = current + length;
                var total = this.FileSize;
                if (isCompleted == true && total == null)
                {
                    total = current;
                }

                var args = new ProgressEventArgs(current, total, isCompleted);
                this.DownloadProgressChanged?.Invoke(this, args);

                if (isCompleted == true)
                {
                    break;
                }
                await stream.WriteAsync(buffer, 0, length, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
