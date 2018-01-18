using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Defaults.KeyValueFormates.Converters
{
    /// <summary>
    /// 表示字典转换器
    /// </summary>
    public class DictionaryConverter : ConverterBase
    {
        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        public override IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context)
        {
            if (context.Descriptor.IsEnumerableKeyValueOfString == true)
            {
                var dic = context.Value as IEnumerable<KeyValuePair<string, string>>;
                return dic.Select(item => this.ToKeyValuePair(item.Key, item.Value, context.Options));
            }

            if (context.Descriptor.IsEnumerableKeyValueOfObject == true)
            {
                var dic = context.Value as IEnumerable<KeyValuePair<string, object>>;
                return dic.Select(item => this.ToKeyValuePair(item.Key, item.Value, context.Options));
            }

            return this.Next.Invoke(context);
        }
    }
}
