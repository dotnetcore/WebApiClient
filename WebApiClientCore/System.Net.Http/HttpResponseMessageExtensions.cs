using System.IO;
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
        /// <param name="stream">流</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns></returns>
        public static async Task SaveAsAsync(this HttpResponseMessage httpResponse, Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var source = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            await source.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
        }
    }
}
