using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// Request message representing httpApi
    /// </summary>
    public class HttpApiRequestMessage : HttpRequestMessage
    {
        /// <summary>
        /// Assembly version information
        /// </summary>
        private static readonly AssemblyName assemblyName = typeof(HttpHandlerProvider).GetTypeInfo().Assembly.GetName();

        /// <summary>
        /// Default UserAgent
        /// </summary>
        private static readonly ProductInfoHeaderValue defaultUserAgent = new ProductInfoHeaderValue(assemblyName.Name, assemblyName.Version.ToString());

        /// <summary>
        /// httpApi request message
        /// </summary>
        public HttpApiRequestMessage()
        {
            this.Headers.ExpectContinue = false;
            this.Headers.UserAgent.Add(defaultUserAgent);
        }

        /// <summary>
        /// Append Query parameters to the request path
        /// </summary>
        /// <param name="keyValue">parameter</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddUrlQuery(IEnumerable<KeyValuePair<string, string>> keyValue)
        {
            this.AddUrlQuery(keyValue, Encoding.UTF8);
        }

        /// <summary>
        /// Append Query parameters to the request path
        /// </summary>
        /// <param name="keyValue">parameter</param>
        /// <param name="encoding">coding</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddUrlQuery(IEnumerable<KeyValuePair<string, string>> keyValue, Encoding encoding)
        {
            foreach (var kv in keyValue)
            {
                this.AddUrlQuery(kv, encoding);
            }
        }

        /// <summary>
        /// Append Query parameters to the request path
        /// </summary>
        /// <param name="keyValue">parameter</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddUrlQuery(KeyValuePair<string, string> keyValue)
        {
            this.AddUrlQuery(keyValue, Encoding.UTF8);
        }

        /// <summary>
        /// Append Query parameters to the request path
        /// </summary>
        /// <param name="keyValue">parameter</param>
        /// <param name="encoding">coding</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddUrlQuery(KeyValuePair<string, string> keyValue, Encoding encoding)
        {
            this.AddUrlQuery(keyValue.Key, keyValue.Value, encoding);
        }


        /// <summary>
        /// Append Query parameters to the request path
        /// </summary>
        /// <param name="key">parameter name</param>
        /// <param name="value">parameter value</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddUrlQuery(string key, string value)
        {
            this.AddUrlQuery(key, value, Encoding.UTF8);
        }

        /// <summary>
        /// Append Query parameters to the request path
        /// </summary>
        /// <param name="key">parameter name</param>
        /// <param name="value">parameter value</param>
        /// <param name="encoding">coding</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddUrlQuery(string key, string value, Encoding encoding)
        {
            if (this.RequestUri == null)
            {
                throw new HttpApiConfigException("未配置RequestUri，RequestUri不能为null");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            var editor = new UriEditor(this.RequestUri, encoding);
            editor.AddQuery(key, value);
            this.RequestUri = editor.Uri;
        }

        /// <summary>
        /// Add field to existing Content
        /// Requires content-type to be application / x-www-form-urlencoded
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">text</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public async Task AddFormFieldAsync(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            var kv = new KeyValuePair<string, string>(name, value);
            await this.AddFormFieldAsync(new[] { kv }).ConfigureAwait(false);
        }

        /// <summary>
        /// Add field to existing Content
        /// Requires content-type to be application / x-www-form-urlencoded
        /// </summary>
        /// <param name="keyValues">Key-value pair</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public async Task AddFormFieldAsync(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            this.EnsureNotGetOrHead();
            this.EnsureMediaTypeEqual(UrlEncodedContent.MediaType);

            if (keyValues == null)
            {
                return;
            }

            var formContent = await UrlEncodedContent.FromHttpContentAsync(this.Content).ConfigureAwait(false);
            await formContent.AddFormFieldAsync(keyValues).ConfigureAwait(false);
            this.Content = formContent;
        }

        /// <summary>
        /// Add file content to existing Content
        /// Requires content-type to be multipart / form-data
        /// </summary>
        /// <param name="stream">File stream</param>
        /// <param name="name">name</param>
        /// <param name="fileName">file name</param>
        /// <param name="contentType">File Mime</param>
        /// <exception cref="NotSupportedException"></exception>
        public void AddMulitpartFile(Stream stream, string name, string fileName, string contentType)
        {
            this.EnsureNotGetOrHead();

            var httpContent = this.CastToMultipartContent();
            var fileContent = new MulitpartFileContent(stream, name, fileName, contentType);
            httpContent.Add(fileContent);
            this.Content = httpContent;
        }

        /// <summary>
        /// Add text content to existing Content
        /// Requires content-type to be multipart / form-data
        /// </summary>     
        /// <param name="keyValues">Key-value pair</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddMulitpartText(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            this.EnsureNotGetOrHead();

            foreach (var kv in keyValues)
            {
                this.AddMulitpartTextInternal(kv.Key, kv.Value);
            }
        }

        /// <summary>
        /// Add text content to existing Content
        /// Requires content-type to be multipart / form-data
        /// </summary>     
        /// <param name="name">name</param>
        /// <param name="value">text</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddMulitpartText(string name, string value)
        {
            this.EnsureNotGetOrHead();
            this.AddMulitpartTextInternal(name, value);
        }

        /// <summary>
        /// Add text content to existing Content
        /// Requires content-type to be multipart / form-data
        /// </summary>     
        /// <param name="name">name</param>
        /// <param name="value">text</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        private void AddMulitpartTextInternal(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var httpContent = this.CastToMultipartContent();
            var textContent = new MulitpartTextContent(name, value);
            httpContent.Add(textContent);
            this.Content = httpContent;
        }


        /// <summary>
        /// Convert to MultipartContent
        /// If null returns an instance of MultipartContent
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        private MultipartContent CastToMultipartContent()
        {
            this.EnsureMediaTypeEqual(MultipartFormContent.MediaType);

            var httpContent = this.Content as MultipartContent;
            if (httpContent == null)
            {
                httpContent = new MultipartFormContent();
            }
            return httpContent;
        }

        /// <summary>
        /// Make sure the mediaType before and after is consistent
        /// </summary>
        /// <param name="newMediaType">New MediaType</param>
        /// <exception cref="NotSupportedException"></exception>
        private void EnsureMediaTypeEqual(string newMediaType)
        {
            var existsMediaType = this.Content?.Headers.ContentType?.MediaType;
            if (string.IsNullOrEmpty(existsMediaType) == true)
            {
                return;
            }

            if (string.Equals(existsMediaType, newMediaType, StringComparison.OrdinalIgnoreCase) == false)
            {
                var message = $"Content-Type must remain as {existsMediaType}";
                throw new NotSupportedException(message);
            }
        }


        /// <summary>
        /// Make sure it's not a Get or Head request
        /// Returns the associated HttpContent object
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        private void EnsureNotGetOrHead()
        {
            if (this.Method == HttpMethod.Get || this.Method == HttpMethod.Head)
            {
                var message = $"{this.Method}方法不支持使用{this.GetType().Name}";
                throw new NotSupportedException(message);
            }
        }

        /// <summary>
        /// Return request header data
        /// </summary>
        /// <returns></returns>
        public string GetHeadersString()
        {
            var builder = new StringBuilder()
               .AppendLine($"{this.Method} {this.RequestUri.PathAndQuery} HTTP/{this.Version}")
               .AppendLine($"Host: {this.RequestUri.Authority}")
               .Append(this.Headers.ToString());

            if (this.Content != null)
            {
                builder.Append(this.Content.Headers.ToString());
            }

            return builder.ToString();
        }


        /// <summary>
        /// Request data returned
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
        /// Return request header data
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.GetHeadersString();
        }
    }
}
