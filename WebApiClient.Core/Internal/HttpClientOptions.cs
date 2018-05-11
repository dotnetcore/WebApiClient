using System;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpClient默认选项
    /// </summary>
    static class HttpClientOptions
    {
        /// <summary>
        /// 获取或设置一个站点内的默认连接数限制
        /// 这个值在初始化HttpClientHandler时使用
        /// 默认值为128
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ConnectionLimit { get; set; } = 128;
    }
}
