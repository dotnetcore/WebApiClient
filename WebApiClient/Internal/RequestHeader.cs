using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace WebApiClient
{
    /// <summary>
    /// 提供请求头枚举到名称的转换
    /// </summary>
    static class RequestHeader
    {
        /// <summary>
        /// 请求头枚举和名称的缓存
        /// </summary>
        private static readonly Dictionary<HttpRequestHeader, string> cache = new Dictionary<HttpRequestHeader, string>();

        /// <summary>
        /// 请求头枚举到名称的转换
        /// </summary>
        static RequestHeader()
        {
            var headerNames =
                from header in Enum.GetValues(typeof(HttpRequestHeader)).Cast<HttpRequestHeader>()
                let name = Regex.Replace(header.ToString(), "[A-Z][^A-Z]", (m) => m.Index == 0 ? m.Value : "-" + m.Value)
                select new { header, name };

            foreach (var item in headerNames)
            {
                cache.Add(item.header, item.name);
            }
        }

        /// <summary>
        /// 获取请求头名称
        /// </summary>
        /// <param name="header">请求头枚举</param>
        /// <returns></returns>
        public static string GetName(HttpRequestHeader header)
        {
            cache.TryGetValue(header, out string name);
            return name;
        }
    }
}
