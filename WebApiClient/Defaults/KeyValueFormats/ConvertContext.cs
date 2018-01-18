using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Defaults.KeyValueFormats
{
    /// <summary>
    /// 表示要转换的上下文
    /// </summary>
    public class ConvertContext
    {
        /// <summary>
        /// 获取或设置名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 获取或设置Value对应的类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 获取或设置格式化选项
        /// </summary>
        public FormatOptions Options { get; set; }
    }
}
