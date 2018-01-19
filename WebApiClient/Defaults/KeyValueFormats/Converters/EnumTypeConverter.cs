using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Defaults.KeyValueFormats.Converters
{
    /// <summary>
    /// 表示枚举类型转换器
    /// </summary>
    public class EnumTypeConverter : ConverterBase
    {
        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        public override IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context)
        {
            var type = context.Type;
            if (type.IsGenericType == true && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {                
                type = Nullable.GetUnderlyingType(type);
            }

            if (type.IsEnum == true)
            {
                var value = (int)context.Value;
                return new[] { base.ConvertToKeyValuePair(context.Name, value, context.Options) };
            }

            return this.Next.Invoke(context);
        }
    }
}
