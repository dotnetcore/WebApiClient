using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 提供对 HttpResponseMessage内容保存的扩展
    /// </summary>
    public static class HttpResponseMessageExtend
    {
        /// <summary>
        /// 保存到目标文件
        /// </summary>
        /// <param name="response"></param>
        /// <param name="filePath">文件路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static async Task SaveAsync(this Task<HttpResponseMessage> response, string filePath)
        {
            var res = await response;
            await res.SaveAsync(filePath);
        }

        /// <summary>
        /// 保存到目标流
        /// </summary>
        /// <param name="response"></param>
        /// <param name="targetStream">流</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static async Task SaveAsync(this Task<HttpResponseMessage> response, Stream targetStream)
        {
            var res = await response;
            await res.SaveAsync(targetStream);
        }


        /// <summary>
        /// 保存到目标文件
        /// </summary>
        /// <param name="response"></param>
        /// <param name="filePath">文件路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static async Task SaveAsync(this ITask<HttpResponseMessage> response, string filePath)
        {
            var res = await response;
            await res.SaveAsync(filePath);
        }

        /// <summary>
        /// 保存到目标流
        /// </summary>
        /// <param name="response"></param>
        /// <param name="targetStream">流</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static async Task SaveAsync(this ITask<HttpResponseMessage> response, Stream targetStream)
        {
            var res = await response;
            await res.SaveAsync(targetStream);
        }

        /// <summary>
        /// 保存到目标文件
        /// </summary>
        /// <param name="response"></param>
        /// <param name="filePath">文件路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static async Task SaveAsync(this HttpResponseMessage response, string filePath)
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
                await response.SaveAsync(fileStream);
            }
        }

        /// <summary>
        /// 保存到目标流
        /// </summary>
        /// <param name="response"></param>
        /// <param name="targetStream">流</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static async Task SaveAsync(this HttpResponseMessage response, Stream targetStream)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (targetStream == null)
            {
                throw new ArgumentNullException(nameof(targetStream));
            }

            if (targetStream.CanWrite == false)
            {
                throw new ArgumentException(nameof(targetStream) + " cannot be write", nameof(targetStream));
            }

            var length = 0;
            var buffer = new byte[8 * 1024];
            var sourceStream = await response.Content.ReadAsStreamAsync();

            while ((length = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await targetStream.WriteAsync(buffer, 0, length);
            }
        }
    }
}
