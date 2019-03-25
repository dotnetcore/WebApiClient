using System;

namespace WebApiClient.DataAnnotations
{
    /// <summary>
    /// 表示适用的序列化的范围
    /// </summary>
    [Flags]
    public enum FormatScope
    {
        /// <summary>
        /// 适用于全部序列化
        /// </summary>
        All = 0,

        /// <summary>
        /// 适用于Json序列化
        /// </summary>
        JsonFormat = 0x1,

        /// <summary>
        /// 适用于KeyValue序列化
        /// </summary>
        KeyValueFormat = 0x2,

        /// <summary>
        /// 适用于Bson序列化
        /// </summary>
        BsonFormat = 0x4,
    }
}
