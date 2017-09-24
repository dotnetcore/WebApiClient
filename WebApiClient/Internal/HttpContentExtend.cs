using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 提供HttpContent的扩展
    /// </summary>
    static class HttpContentExtend
    {
        /// <summary>
        /// 将httpContent与keyValues合并
        /// 返回合并后得到的新HttpContent
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="nameValues">键值对</param>
        /// <returns></returns>
        public static async Task<HttpContent> MergeKeyValuesAsync(this HttpContent instance, IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            if (keyValues == null)
            {
                return instance;
            }

            var formBody = default(byte[]);
            const string mediaType = "application/x-www-form-urlencoded";

            if (instance != null)
            {
                instance.Headers.ContentType.EnsureMediaTypeEqual(mediaType);
                formBody = await instance.ReadAsByteArrayAsync();
            }

            var bytesContent = Merge(formBody, keyValues);
            var byteArrayContent = new ByteArrayContent(bytesContent);
            byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            return byteArrayContent;
        }

        /// <summary>
        /// 合并内容
        /// </summary>
        /// <param name="formBody"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        private static byte[] Merge(byte[] formBody, IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            var encoding = Encoding.UTF8;
            var kvs = from kv in keyValues select string.Format("{0}={1}", kv.Key, HttpUtility.UrlEncode(kv.Value, encoding));
            var stringContent = string.Join("&", kvs);

            if (formBody != null && formBody.Length > 0)
            {
                stringContent = "&" + stringContent;
            }

            var byteConent = encoding.GetBytes(stringContent);
            return Merge(formBody, byteConent);
        }

        /// <summary>
        /// 合并字节组
        /// </summary>
        /// <param name="formBody"></param>
        /// <param name="byteConent"></param>
        /// <returns></returns>
        private static byte[] Merge(byte[] formBody, byte[] byteConent)
        {
            if (formBody == null || formBody.Length == 0)
            {
                return byteConent;
            }

            var bytes = new byte[formBody.Length + byteConent.Length];
            formBody.CopyTo(bytes, 0);
            byteConent.CopyTo(bytes, formBody.Length);
            return bytes;
        }


        /// <summary>
        /// 转换为MultipartContent
        /// 为null则返回MultipartContent的实例
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static MultipartContent CastOrCreateMultipartContent(this HttpContent instance)
        {
            if (instance != null)
            {
                instance.Headers.ContentType.EnsureMediaTypeEqual("multipart/form-data");
            }

            var httpContent = instance as MultipartContent;
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
        /// 添加文件内容
        /// </summary>
        /// <param name="httpContent"></param>
        /// <param name="stream">文件流</param>
        /// <param name="name">名称</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">文件Mime</param>
        public static void AddFile(this MultipartContent httpContent, Stream stream, string name, string fileName, string contentType)
        {
            var fileContent = new MulitpartFileContent(stream, name, fileName, contentType);
            httpContent.Add(fileContent);
        }

        /// <summary>
        /// 添加文本内容
        /// </summary>     
        /// <param name="httpContent"></param>
        /// <param name="keyValues">键值对</param>
        public static void AddText(this MultipartContent httpContent, IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            foreach (var kv in keyValues)
            {
                httpContent.AddText(kv.Key, kv.Value);
            }
        }

        /// <summary>
        /// 添加文本内容
        /// </summary>     
        /// <param name="httpContent"></param>
        /// <param name="name">名称</param>
        /// <param name="value">文本</param>
        public static void AddText(this MultipartContent httpContent, string name, string value)
        {
            var textContent = new MulitpartTextContent(name, value);
            httpContent.Add(textContent);
        }

        /// <summary>
        /// 确保前后的mediaType一致
        /// </summary>
        /// <param name="contentType">已有的ContentType</param>
        /// <param name="newMediaType">新的MediaType</param>
        /// <exception cref="NotSupportedException"></exception>
        private static void EnsureMediaTypeEqual(this MediaTypeHeaderValue contentType, string newMediaType)
        {
            if (contentType == null)
            {
                return;
            }

            var oldMediaType = contentType.MediaType;
            if (string.Equals(oldMediaType, newMediaType, StringComparison.OrdinalIgnoreCase) == false)
            {
                var message = string.Format("Content-Type必须保持为{0}", oldMediaType);
                throw new NotSupportedException(message);
            }
        }
    }
}
