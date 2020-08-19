using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;
using WebApiClientCore.HttpContents;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示httpApi的请求消息
    /// </summary>
    public class HttpApiRequestMessage : HttpRequestMessage
    {
        /// <summary>
        /// 程序集版本信息
        /// </summary>
        private static readonly AssemblyName assemblyName = typeof(HttpApiRequestMessage).Assembly.GetName();

        /// <summary>
        /// 请求头的默认UserAgent
        /// </summary>
        private readonly static ProductInfoHeaderValue defaultUserAgent = new ProductInfoHeaderValue(assemblyName.Name, assemblyName.Version?.ToString());


        /// <summary>
        /// httpApi的请求消息
        /// </summary>
        public HttpApiRequestMessage()
            : this(null)
        {
        }

        /// <summary>
        /// httpApi的请求消息
        /// </summary>
        /// <param name="requestUri">请求uri</param>
        public HttpApiRequestMessage(Uri? requestUri)
            : this(requestUri, useDefaultUserAgent: false)
        {
        }

        /// <summary>
        /// httpApi的请求消息
        /// </summary>
        /// <param name="requestUri">请求uri</param>
        /// <param name="useDefaultUserAgent">请求头是否包含默认的UserAgent</param>
        public HttpApiRequestMessage(Uri? requestUri, bool useDefaultUserAgent)
        {
            this.RequestUri = requestUri;
            if (useDefaultUserAgent == true)
            {
                this.Headers.UserAgent.Add(defaultUserAgent);
            }
        }

        /// <summary>
        /// 追加Query参数到请求路径
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="value">参数值</param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddUrlQuery(string key, string? value)
        {
            if (this.RequestUri == null)
            {
                throw new ApiInvalidConfigException(Resx.required_RequestUri);
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var editor = new UriEditor(this.RequestUri);
            editor.AddQuery(key, value);
            this.RequestUri = editor.Uri;
        }

        /// <summary>
        /// 添加字段到已有的Content
        /// 要求content-type为application/x-www-form-urlencoded
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public async Task AddFormFieldAsync(string name, string? value)
        {
            var keyValue = new KeyValue(name, value);
            await this.AddFormFieldAsync(new[] { keyValue }).ConfigureAwait(false);
        }

        /// <summary>
        /// 添加字段到已有的Content
        /// 要求content-type为application/x-www-form-urlencoded
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public async Task AddFormFieldAsync(IEnumerable<KeyValue> keyValues)
        {
            this.EnsureNotGetOrHead();
            this.EnsureMediaTypeEqual(FormContent.MediaType);

            var formContent = await FormContent.ParseAsync(this.Content).ConfigureAwait(false);
            formContent.AddFormField(keyValues);
            this.Content = formContent;
        }


        /// <summary>
        /// 添加文本内容到已有的Content
        /// 要求content-type为multipart/form-data
        /// </summary>     
        /// <param name="name">名称</param>
        /// <param name="value">文本</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddFormDataText(string name, string? value)
        {
            var keyValue = new KeyValue(name, value);
            this.AddFormDataText(new[] { keyValue });
        }

        /// <summary>
        /// 添加文本内容到已有的Content
        /// 要求content-type为multipart/form-data
        /// </summary>     
        /// <param name="keyValues">键值对</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddFormDataText(IEnumerable<KeyValue> keyValues)
        {
            this.EnsureNotGetOrHead();
            this.EnsureMediaTypeEqual(FormDataContent.MediaType);

            if (!(this.Content is MultipartContent httpContent))
            {
                httpContent = new FormDataContent();
            }

            foreach (var keyValue in keyValues)
            {
                var textContent = new FormDataTextContent(keyValue);
                httpContent.Add(textContent);
                this.Content = httpContent;
            }
        }


        /// <summary>
        /// 添加文件内容到已有的Content
        /// 要求content-type为multipart/form-data
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="name">名称</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">文件Mime</param>
        /// <exception cref="NotSupportedException"></exception>
        public void AddFormDataFile(Stream stream, string name, string? fileName, string? contentType)
        {
            this.EnsureNotGetOrHead();
            this.EnsureMediaTypeEqual(FormDataContent.MediaType);

            if (!(this.Content is MultipartContent httpContent))
            {
                httpContent = new FormDataContent();
            }

            var fileContent = new FormDataFileContent(stream, name, fileName, contentType);
            httpContent.Add(fileContent);
            this.Content = httpContent;
        }

        /// <summary>
        /// 确保不是Get或Head请求
        /// 返回关联的HttpContent对象
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureNotGetOrHead()
        {
            if (this.Method == HttpMethod.Get || this.Method == HttpMethod.Head)
            {
                var message = Resx.unsupported_HttpContent.Format(this.Method, this.GetType().Name);
                throw new NotSupportedException(message);
            }
        }

        /// <summary>
        /// 确保前后的mediaType一致
        /// </summary>
        /// <param name="newMediaType">新的MediaType</param>
        /// <exception cref="NotSupportedException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureMediaTypeEqual(string newMediaType)
        {
            var existsMediaType = this.Content?.Headers.ContentType?.MediaType;
            if (string.IsNullOrEmpty(existsMediaType) == true)
            {
                return;
            }

            if (string.Equals(existsMediaType, newMediaType, StringComparison.OrdinalIgnoreCase) == false)
            {
                var message = Resx.contenType_RemainAs.Format(existsMediaType);
                throw new NotSupportedException(message);
            }
        }

        /// <summary>
        /// 读取请求头
        /// </summary>
        /// <returns></returns>
        public string GetHeadersString()
        {
            const string host = "Host";
            var builder = new StringBuilder()
               .AppendLine($"{this.Method} {this.RequestUri.PathAndQuery} HTTP/{this.Version}");

            if (this.Headers.Contains(host) == false)
            {
                builder.AppendLine($"{host}: {this.RequestUri.Authority}");
            }

            builder.Append(this.Headers.ToString());
            if (this.Content != null)
            {
                builder.Append(this.Content.Headers.ToString());
            }

            return builder.ToString();
        }


        /// <summary>
        /// 返回请求数据
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetRequestStringAsync()
        {
            var builder = new StringBuilder(this.GetHeadersString());
            if (this.Content != null)
            {
                var content = await this.Content.ReadAsStringAsync().ConfigureAwait(false);
                builder.Append(content);
            }
            return builder.ToString();
        }


        /// <summary>
        /// 返回请求头数据
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.GetHeadersString();
        }
    }
}
