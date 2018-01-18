using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Defaults.KeyValueFormats.Converters
{
    /// <summary>
    /// 表示KeyValuePair类型转换器
    /// </summary>
    public class KeyValuePairConverter : ConverterBase
    {
        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        public override IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context)
        {
            if (context.Type == typeof(KeyValuePair<string, string>))
            {
                var kv = (KeyValuePair<string, string>)context.Value;
                return new[] { this.GetKeyValuePair(kv.Key, kv.Value, context.Options) };
            }

            if (context.Type == typeof(KeyValuePair<string, object>))
            {
                var kv = (KeyValuePair<string, object>)context.Value;
                return new[] { this.GetKeyValuePair(kv.Key, kv.Value, context.Options) };
            }

            return this.Next.Invoke(context);
        }
    }
}
