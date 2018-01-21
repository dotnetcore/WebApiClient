using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.DataAnnotations
{
    /// <summary>
    /// 表示注解特性的使用范围
    /// </summary>
    [Flags]
    public enum AnnotateScope
    {
        /// <summary>
        /// 适用于Json序列化
        /// </summary>
        JsonFormat = 0x1,

        /// <summary>
        /// 适用于KeyValue序列化
        /// </summary>
        KeyValueFormat = 0x2
    }
}
