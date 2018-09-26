using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
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
        /// 获取响应的友好文件名称
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 获取文件的大小
        /// </summary>
        public long? FileSize { get; private set; }

        /// <summary>
        /// 获取文件类型
        /// </summary>
        public string MediaType { get; private set; }

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
                await this.SaveAsAsync(fileStream).ConfigureAwait(false);
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
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (stream.CanWrite == false)
            {
                throw new ArgumentException(nameof(stream) + " cannot be write", nameof(stream));
            }

            var length = 0;
            var buffer = new byte[8 * 1024];
            var sourceStream = await this.HttpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);

            while ((length = await sourceStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
            {
                await stream.WriteAsync(buffer, 0, length).ConfigureAwait(false);
            }
        }
    }
}
