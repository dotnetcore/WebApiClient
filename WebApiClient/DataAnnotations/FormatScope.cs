using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.DataAnnotations
{
    /// <summary>
    /// 表示适用的序列化的范围
    /// </summary>
    [Flags]
    public enum FormatScope
    {
        /// <summary>
        /// 适用于Json序列化
        /// </summary>
        JsonFormat = 0x1,

        /// <summary>
        /// 适用于KeyValue序列化
        /// </summary>
        KeyValueFormat = 0x2,

        /// <summary>
        /// 适用于Json序列化和KeyValue序列化
        /// </summary>
        All = JsonFormat | KeyValueFormat
    }
}
