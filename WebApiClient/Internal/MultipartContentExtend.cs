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
    /// 提供MultipartContent的扩展
    /// </summary>
    static class MultipartContentExtend
    {
        /// <summary>
        /// 转换为MultipartContent
        /// 失败则返回MultipartContent的实例
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static MultipartContent CastOrCreateMultipartContent(this HttpContent instance)
        {
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
        /// <returns></returns>
        public static MultipartContent AddFile(this MultipartContent httpContent, Stream stream, string name, string fileName, string contentType = "application/octet-stream")
        {
            var fileContent = new MulitpartFileContent(stream, name, fileName, contentType);
            httpContent.Add(fileContent);
            return httpContent;
        }

        /// <summary>
        /// 添加文本内容
        /// </summary>     
        /// <param name="httpContent"></param>
        /// <param name="name">名称</param>
        /// <param name="value">文本</param>
        public static MultipartContent AddText(this MultipartContent httpContent, string name, string value)
        {
            var textContent = new MulitpartTextContent(name, value);
            httpContent.Add(textContent);
            return httpContent;
        }
    }
}
