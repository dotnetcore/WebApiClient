using System.Net.Http;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示缓存结果
    /// </summary>
    class ApiCacheValue
    {
        /// <summary>
        /// 获取缓存的键
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 获取响应信息
        /// </summary>
        public HttpResponseMessage Value { get; set; }

        /// <summary>
        /// 缓存结果
        /// </summary>
        /// <param name="key">缓存的键</param>
        /// <param name="value">响应信息</param>
        public ApiCacheValue(string key, HttpResponseMessage value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
