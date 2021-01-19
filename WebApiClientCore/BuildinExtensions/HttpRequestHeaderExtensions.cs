using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供请求头枚举到名称的转换
    /// </summary>
    static class HttpRequestHeaderExtensions
    {
        /// <summary>
        /// HttpRequestHeader的类型
        /// </summary>
        private static readonly Type httpRequestHeaderType = typeof(HttpRequestHeader);

        /// <summary>
        /// 请求头枚举和名称的缓存
        /// </summary>
        private static readonly Dictionary<HttpRequestHeader, string> cache = new Dictionary<HttpRequestHeader, string>();

        /// <summary>
        /// 请求头枚举到名称的转换
        /// </summary>
        static HttpRequestHeaderExtensions()
        {
            foreach (var header in Enum.GetValues(httpRequestHeaderType).Cast<HttpRequestHeader>())
            {
                cache.Add(header, header.GetHeaderName());
            }
        }

        /// <summary>
        /// 返回枚举的DisplayName
        /// </summary>
        /// <param name="header">请求头枚举</param>
        /// <returns></returns>
        private static string GetHeaderName(this HttpRequestHeader header)
        {
            return httpRequestHeaderType
                .GetField(header.ToString())?
                .GetCustomAttribute<DisplayAttribute>()?
                .Name ?? header.ToString();
        }

        /// <summary>
        /// 转换为header名
        /// </summary>
        /// <param name="header">请求头枚举</param>
        /// <returns></returns>
        public static string ToHeaderName(this HttpRequestHeader header)
        {
            if (cache.TryGetValue(header, out var name))
            {
                return name;
            }
            return header.ToString();
        }
    }
}