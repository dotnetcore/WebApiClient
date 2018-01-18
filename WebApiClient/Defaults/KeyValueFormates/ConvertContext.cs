using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Defaults.KeyValueFormates
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
        /// 获取或设置格式化选项
        /// </summary>
        public FormatOptions Options { get; set; }

        /// <summary>
        /// 获取或设置类型描述
        /// </summary>
        public TypeDescriptor Descriptor { get; set; }
    }
}
