using System;

namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// 表示键命名选项
    /// </summary>
    public class KeyNamingOptions
    {
        /// <summary>
        /// 获取或设置键名的分隔符
        /// 默认为.
        /// </summary>
        public string KeyDelimiter { get; set; } = ".";

        /// <summary>
        /// 获取或设置数组索引格式化
        /// 默认为[i]
        /// </summary>
        public Func<int, string> KeyArrayIndex { get; set; } = i => $"[{i}]";

        /// <summary>
        /// 获取或设置键名风格
        /// </summary>
        public KeyNamingStyle KeyNamingStyle { get; set; } = KeyNamingStyle.ShortName;
    }
}
