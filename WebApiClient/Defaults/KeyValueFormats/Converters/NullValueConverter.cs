using System.Collections.Generic;

namespace WebApiClient.Defaults.KeyValueFormats.Converters
{
    /// <summary>
    /// 表示Null值转换器
    /// </summary>
    public class NullValueConverter : ConverterBase
    {
        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        public override IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context)
        {
            if (context.Data == null)
            {
                return context.ToKeyValuePairs();
            }
            return this.Next.Invoke(context);
        }
    }
}
