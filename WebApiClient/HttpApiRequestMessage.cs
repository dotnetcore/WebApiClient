using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示http api的请求消息
    /// </summary>
    public class HttpApiRequestMessage : HttpRequestMessage
    {
        /// <summary>
        /// 获取或设置请求的超时时间
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// 追加Query参数到请求路径
        /// </summary>
        /// <param name="keyValue">参数</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddUrlQuery(IEnumerable<KeyValuePair<string, string>> keyValue)
        {
            this.AddUrlQuery(keyValue, Encoding.UTF8);
        }

        /// <summary>
        /// 追加Query参数到请求路径
        /// </summary>
        /// <param name="keyValue">参数</param>
        /// <param name="encoding">编码</param>
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
        /// 追加Query参数到请求路径
        /// </summary>
        /// <param name="keyValue">参数</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddUrlQuery(KeyValuePair<string, string> keyValue)
        {
            this.AddUrlQuery(keyValue, Encoding.UTF8);
        }

        /// <summary>
        /// 追加Query参数到请求路径
        /// </summary>
        /// <param name="keyValue">参数</param>
        /// <param name="encoding">编码</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddUrlQuery(KeyValuePair<string, string> keyValue, Encoding encoding)
        {
            this.AddUrlQuery(keyValue.Key, keyValue.Value, encoding);
        }


        /// <summary>
        /// 追加Query参数到请求路径
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="value">参数值</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddUrlQuery(string key, string value)
        {
            this.AddUrlQuery(key, value, Encoding.UTF8);
        }

        /// <summary>
        /// 追加Query参数到请求路径
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="encoding">编码</param>
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

            var valueEncoded = HttpUtility.UrlEncode(value, encoding);
            var query = string.Format("{0}={1}", key, valueEncoded);

            var url = this.RequestUri.ToString().TrimEnd('?', '&', '/');
            var concat = url.Contains('?') ? "&" : "?";

            this.RequestUri = new Uri(url + concat + query);
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
        public async Task AddFormFieldAsync(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            var kv = new KeyValuePair<string, string>(name, value);
            await this.AddFormFieldAsync(new[] { kv });
        }

        /// <summary>
        /// 添加字段到已有的Content
        /// 要求content-type为application/x-www-form-urlencoded
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public async Task AddFormFieldAsync(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            this.EnsureNotGetOrHead();

            if (keyValues == null)
            {
                return;
            }

            var formBody = default(byte[]);
            const string mediaType = "application/x-www-form-urlencoded";

            if (this.Content != null)
            {
                this.EnsureMediaTypeEqual(mediaType);
                formBody = await this.Content.ReadAsByteArrayAsync();
            }

            var bytesContent = MergeFields(formBody, keyValues);
            var byteArrayContent = new ByteArrayContent(bytesContent);
            byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            this.Content = byteArrayContent;
        }

        /// <summary>
        /// 合并内容
        /// </summary>
        /// <param name="formBody"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        private static byte[] MergeFields(byte[] formBody, IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            var encoding = Encoding.UTF8;
            var kvs = from kv in keyValues select string.Format("{0}={1}", kv.Key, HttpUtility.UrlEncode(kv.Value, encoding));
            var stringContent = string.Join("&", kvs);

            if (formBody != null && formBody.Length > 0)
            {
                stringContent = "&" + stringContent;
            }

            var byteConent = encoding.GetBytes(stringContent);
            return MergeBytes(formBody, byteConent);
        }

        /// <summary>
        /// 合并字节组
        /// </summary>
        /// <param name="formBody"></param>
        /// <param name="byteConent"></param>
        /// <returns></returns>
        private static byte[] MergeBytes(byte[] formBody, byte[] byteConent)
        {
            if (formBody == null || formBody.Length == 0 || byteConent == null)
            {
                return byteConent;
            }

            var bytes = new byte[formBody.Length + byteConent.Length];
            formBody.CopyTo(bytes, 0);
            byteConent.CopyTo(bytes, formBody.Length);
            return bytes;
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
        public void AddMulitpartFile(Stream stream, string name, string fileName, string contentType)
        {
            this.EnsureNotGetOrHead();

            var httpContent = this.CastOrCreateMultipartContent();
            var fileContent = new MulitpartFileContent(stream, name, fileName, contentType);
            httpContent.Add(fileContent);
            this.Content = httpContent;
        }

        /// <summary>
        /// 添加文本内容到已有的Content
        /// 要求content-type为multipart/form-data
        /// </summary>     
        /// <param name="keyValues">键值对</param>
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
        /// 添加文本内容到已有的Content
        /// 要求content-type为multipart/form-data
        /// </summary>     
        /// <param name="name">名称</param>
        /// <param name="value">文本</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddMulitpartText(string name, string value)
        {
            this.EnsureNotGetOrHead();
            this.AddMulitpartTextInternal(name, value);
        }

        /// <summary>
        /// 添加文本内容到已有的Content
        /// 要求content-type为multipart/form-data
        /// </summary>     
        /// <param name="name">名称</param>
        /// <param name="value">文本</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        private void AddMulitpartTextInternal(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var httpContent = this.CastOrCreateMultipartContent();
            var textContent = new MulitpartTextContent(name, value);
            httpContent.Add(textContent);
            this.Content = httpContent;
        }

        /// <summary>
        /// 设置Cookie值到请求头
        /// 当HttpClientHandler.UseCookies == false才会提交到服务端
        /// </summary>
        /// <param name="cookieValues">cookie值，会自动进行URL编码，eg：key1=value1; key2=value2</param>
        /// <exception cref="CookieException"></exception>
        /// <returns></returns>
        public bool SetCookie(string cookieValues)
        {
            return this.SetCookie(cookieValues, true);
        }

        /// <summary>
        /// 设置Cookie值到请求头
        /// 当HttpClientHandler.UseCookies == false才会提交到服务端
        /// </summary>
        /// <param name="cookieValues">cookie值，不进行URL编码，eg：key1=value1; key2=value2</param>
        /// <exception cref="CookieException"></exception>
        /// <returns></returns>
        public bool SetRawCookie(string cookieValues)
        {
            return this.SetCookie(cookieValues, false);
        }

        /// <summary>
        /// 设置Cookie值到请求头
        /// 当HttpClientHandler.UseCookies == false才会提交到服务端
        /// </summary>
        /// <param name="cookieValues">cookie值，eg：key1=value1; key2=value2</param>
        /// <param name="useUrlEncode">是否使用Url编码</param>
        /// <exception cref="CookieException"></exception>
        /// <returns></returns>
        private bool SetCookie(string cookieValues, bool useUrlEncode)
        {
            const string cookieName = "Cookie";
            this.Headers.Remove(cookieName);

            if (cookieValues == null)
            {
                return false;
            }

            if (useUrlEncode == true)
            {
                const string separator = "; ";
                var cookieItems = from item in HttpUtility.ParseCookie(cookieValues, true)
                                  select string.Format("{0}={1}", item.Name, item.Value);
                cookieValues = string.Join(separator, cookieItems);
            }

            return this.Headers.TryAddWithoutValidation(cookieName, cookieValues);
        }

        /// <summary>
        /// 转换为MultipartContent
        /// 为null则返回MultipartContent的实例
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        private MultipartContent CastOrCreateMultipartContent()
        {
            if (this.Content != null)
            {
                this.EnsureMediaTypeEqual("multipart/form-data");
            }

            var httpContent = this.Content as MultipartContent;
            if (httpContent == null)
            {
                var boundary = Guid.NewGuid().ToString();
                var parameter = new NameValueHeaderValue("boundary", boundary);
                httpContent = new MultipartContent("form-data", boundary);

                httpContent.Headers.ContentType.Parameters.Clear();
                httpContent.Headers.ContentType.Parameters.Add(parameter);
            }
            return httpContent;
        }

        /// <summary>
        /// 确保前后的mediaType一致
        /// </summary>
        /// <param name="newMediaType">新的MediaType</param>
        /// <exception cref="NotSupportedException"></exception>
        private void EnsureMediaTypeEqual(string newMediaType)
        {
            var contentType = this.Content.Headers.ContentType;
            if (contentType == null)
            {
                return;
            }

            var existsMediaType = contentType.MediaType;
            if (string.Equals(existsMediaType, newMediaType, StringComparison.OrdinalIgnoreCase) == false)
            {
                var message = string.Format("Content-Type必须保持为{0}", existsMediaType);
                throw new NotSupportedException(message);
            }
        }


        /// <summary>
        /// 确保不是Get或Head请求
        /// 返回关联的HttpContent对象
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        private void EnsureNotGetOrHead()
        {
            if (this.Method == HttpMethod.Get || this.Method == HttpMethod.Head)
            {
                var message = string.Format("{0}方法不支持使用{1}", this.Method, this.GetType().Name);
                throw new NotSupportedException(message);
            }
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder()
                .Append(this.Method).Append(" ")
                .Append(this.RequestUri.PathAndQuery)
                .Append(" HTTP/").Append(this.Version).AppendLine()
                .Append("Host: ").Append(this.RequestUri.Authority).AppendLine();

            foreach (var item in this.Headers)
            {
                builder.AppendFormat("{0}: {1}", item.Key, string.Join(",", item.Value)).AppendLine();
            }

            if (this.Content != null)
            {
                foreach (var item in this.Content.Headers)
                {
                    builder.AppendFormat("{0}: {1}", item.Key, string.Join(",", item.Value)).AppendLine();
                }
            }

            builder.AppendLine();
            return builder.ToString();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public async Task<string> ToStringAsync()
        {
            var builder = new StringBuilder()
                .Append(this.ToString());

            if (this.Content != null)
            {
                var content = await this.Content.ReadAsStringAsync();
                builder.Append(content);
            }

            return builder.ToString();
        }
    }
}
