using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// File representing Http response
    /// Can be declared as the return type of the interface
    /// </summary>
    [DebuggerDisplay("FileName = {FileName}")]
    public class HttpResponseFile : HttpResponseWrapper
    {
        /// <summary>
        /// Download progress change event
        /// </summary>
        public event EventHandler<ProgressEventArgs> DownloadProgressChanged;

        /// <summary>
        /// Get the friendly file name of the response
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Get file size
        /// </summary>
        public long? FileSize { get; }

        /// <summary>
        /// Get file type
        /// </summary>
        public string MediaType { get; }

        /// <summary>
        /// Http response file
        /// </summary>
        /// <param name="response">Response message</param>
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
        /// Save to specified path
        /// </summary>
        /// <param name="filePath">File path and file name</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public async Task SaveAsAsync(string filePath)
        {
            await this.SaveAsAsync(filePath, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Save to specified path
        /// </summary>
        /// <param name="filePath">File path and file name</param>
        /// <param name="cancellationToken">Cancel token</param>
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
        /// Save to target stream
        /// </summary>
        /// <param name="stream">stream</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public async Task SaveAsAsync(Stream stream)
        {
            await this.SaveAsAsync(stream, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Save to target stream
        /// </summary>
        /// <param name="stream">stream</param>
        /// <param name="cancellationToken">Cancel token</param>
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
