using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 提供HttpContent转换为MultipartContent
    /// </summary>
    static class HttpContentConvert
    {
        /// <summary>
        /// 转换为MultipartContent
        /// 失败则返回MultipartContent的实例
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static MultipartContent CastMultipartContent(this HttpContent instance)
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
    }
}
