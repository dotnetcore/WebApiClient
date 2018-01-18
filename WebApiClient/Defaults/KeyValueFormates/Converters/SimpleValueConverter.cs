using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Defaults.KeyValueFormates.Converters
{
    /// <summary>
    /// 表示简单值类型转换器
    /// </summary>
    public class SimpleValueConverter : ConverterBase
    {
        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        public override IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context)
        {
            if (context.Descriptor.IsSimpleType == true)
            {
                return new[] { this.ToKeyValuePair(context) };
            }
            return this.Next.Invoke(context);
        }
    }
}
