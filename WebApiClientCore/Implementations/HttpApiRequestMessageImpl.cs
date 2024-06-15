using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;
using WebApiClientCore.HttpContents;
using WebApiClientCore.Internals;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示HttpApi的请求消息
    /// </summary>
    sealed class HttpApiRequestMessageImpl : HttpApiRequestMessage
    {
        /// <summary>
        /// 程序集版本信息
        /// </summary>
        private static readonly AssemblyName assemblyName = typeof(HttpApiRequestMessageImpl).Assembly.GetName();

        /// <summary>
        /// 请求头的默认UserAgent
        /// </summary>
        private readonly static ProductInfoHeaderValue defaultUserAgent = new(assemblyName.Name ?? "WebApiClientCore", assemblyName.Version?.ToString());

        /// <summary>
        /// httpApi的请求消息
        /// </summary> 
        public HttpApiRequestMessageImpl()
            : this(null, true)
        {
        }

        /// <summary>
        /// httpApi的请求消息
        /// </summary>
        /// <param name="requestUri">请求 uri</param>
        /// <param name="useDefaultUserAgent">请求头是否包含默认的UserAgent</param>
        public HttpApiRequestMessageImpl(Uri? requestUri, bool useDefaultUserAgent)
        {
            this.RequestUri = requestUri;
            if (useDefaultUserAgent == true)
            {
                this.Headers.UserAgent.Add(defaultUserAgent);
            }
        }

        /// <summary>
        /// 返回使用 uri 值合成的请求URL
        /// </summary> 
        /// <param name="uri">uri值</param> 
        /// <returns></returns>
        public override Uri MakeRequestUri(Uri uri)
        {
            var baseUri = this.RequestUri;
            if (uri.IsAbsoluteUri == false)
            {
                return CreateUriByRelative(baseUri, uri);
            }

            if (uri.AbsolutePath == "/")
            {
                return CreateUriByAbsolute(uri, baseUri);
            }

            return uri;
        }

        /// <summary>
        /// 创建 uri
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="relative"></param>
        /// <returns></returns>
        private static Uri CreateUriByRelative(Uri? baseUri, Uri relative)
        {
            if (baseUri == null)
            {
                return relative;
            }

            if (baseUri.IsAbsoluteUri == true)
            {
                return new Uri(baseUri, relative);
            }

            return relative;
        }

        /// <summary>
        /// 创建 uri
        /// 参数值的 uri 是绝对 uir，且只有根路径
        /// </summary>
        /// <param name="absolute"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static Uri CreateUriByAbsolute(Uri absolute, Uri? uri)
        {
            if (uri == null)
            {
                return absolute;
            }

            var relative = GetRelativeUri(uri);
            return relative == "/" ? absolute : new Uri(absolute, relative);
        }

        /// <summary>
        /// 返回相对 uri
        /// </summary>
        /// <param name="uri">uri</param> 
        /// <returns></returns>
        private static string GetRelativeUri(Uri uri)
        {
            if (uri.IsAbsoluteUri == false)
            {
                return uri.OriginalString;
            }

            var path = uri.OriginalString.AsSpan()[(uri.Scheme.Length + 3)..];
            var index = path.IndexOf('/');
            if (index < 0)
            {
                return "/";
            }

            return path[index..].ToString();
        }

        /// <summary>
        /// 追加Query参数到请求路径
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="value">参数值</param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public override void AddUrlQuery(string key, string? value)
        {
            var uri = this.RequestUri;
            if (uri == null)
            {
                throw new ApiInvalidConfigException(Resx.required_RequestUri);
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            this.RequestUri = new UriValue(uri).AddQuery(key, value).ToUri();
        }


        /// <summary>
        /// 添加字段到已有的Content
        /// 要求 content-type 为 application/x-www-form-urlencoded
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public override async Task AddFormFieldAsync(IEnumerable<KeyValue> keyValues)
        {
            this.EnsureMediaTypeEqual(FormContent.MediaType);

            var formContent = await FormContent.ParseAsync(this.Content).ConfigureAwait(false);
            formContent.AddFormField(keyValues);
            this.Content = formContent;
        }

        /// <summary>
        /// 添加文本内容到已有的Content
        /// 要求 content-type 为 multipart/form-data
        /// </summary>     
        /// <param name="keyValues">键值对</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public override void AddFormDataText(IEnumerable<KeyValue> keyValues)
        {
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
        /// 要求 content-type 为 multipart/form-data
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="name">名称</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">文件Mime</param>
        /// <exception cref="NotSupportedException"></exception>
        public override void AddFormDataFile(Stream stream, string name, string? fileName, string? contentType)
        {
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
        /// 确保前后的 mediaType 一致
        /// </summary>
        /// <param name="newMediaType">新的MediaType</param>
        /// <exception cref="NotSupportedException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureMediaTypeEqual(string newMediaType)
        {
            if (this.Content == null)
            {
                return;
            }

            var contentType = this.Content.Headers.ContentType;
            if (contentType == null)
            {
                return;
            }

            var oldMediaType = contentType.MediaType;
            if (newMediaType.Equals(oldMediaType, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var message = Resx.contenType_RemainAs.Format(oldMediaType);
            throw new NotSupportedException(message);
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
