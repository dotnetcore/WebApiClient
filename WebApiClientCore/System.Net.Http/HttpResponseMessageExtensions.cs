using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    /// <summary>
    /// HttpResponseMessage扩展
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// 读取响应头
        /// </summary>
        /// <param name="response">响应信息</param>
        /// <returns></returns>
        public static string GetHeadersString(this HttpResponseMessage response)
        {
            var builder = new StringBuilder()
                .AppendLine($"HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}")
                .Append(response.Headers.ToString());

            if (response.Content != null)
            {
                builder.Append(response.Content.Headers.ToString());
            }
            return builder.ToString();
        }

        /// <summary>
        /// 保存到指定路径
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <param name="filePath">文件路径和文件名</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns></returns>
        public static async Task SaveAsAsync(this HttpResponseMessage httpResponse, string filePath, CancellationToken cancellationToken = default)
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

            using var fileStream = File.OpenWrite(filePath);
            await httpResponse.SaveAsAsync(fileStream, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 保存到目标流
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <param name="destination">目标流</param>
        /// <param name="cancellationToken">取消令牌</param> 
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns></returns>
        public static async Task SaveAsAsync(this HttpResponseMessage httpResponse, Stream destination, CancellationToken cancellationToken = default)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            var source = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            await source.CopyToAsync(destination, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 保存到目标流
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <param name="destination">目标流</param>
        /// <param name="progressChanged">进度变化</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns></returns>
        public static async Task SaveAsAsync(this HttpResponseMessage httpResponse, Stream destination, Action<HttpProgress> progressChanged, CancellationToken cancellationToken = default)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            var recvSize = 0L;
            var isCompleted = false;
            var fileSize = httpResponse.Content.Headers.ContentLength;

            var buffer = new byte[8 * 1024];
            var source = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);

            while (isCompleted == false && cancellationToken.IsCancellationRequested == false)
            {
                var length = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                if (length == 0)
                {
                    if (fileSize == null)
                    {
                        fileSize = recvSize;
                    }
                    isCompleted = true;
                }
                else
                {
                    recvSize += length;
                    await destination.WriteAsync(buffer, 0, length, cancellationToken).ConfigureAwait(false);
                    await destination.FlushAsync(cancellationToken).ConfigureAwait(false);
                }

                progressChanged.Invoke(new HttpProgress(fileSize, recvSize, isCompleted));
            }
        }
    }
}
