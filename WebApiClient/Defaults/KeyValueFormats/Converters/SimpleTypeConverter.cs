using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Defaults.KeyValueFormats.Converters
{
    /// <summary>
    /// 表示简单类型转换器
    /// </summary>
    public class SimpleTypeConverter : ConverterBase
    {
        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        public override IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context)
        {
            if (this.IsSupported(context.Type) == true)
            {
                return new[] { base.ConvertToKeyValuePair(context) };
            }
            return this.Next.Invoke(context);
        }

        /// <summary>
        /// 获取类型是否为简单类型
        /// 从而使用ToString()方法值为字符串
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        protected virtual bool IsSupported(Type type)
        {
            if (type.IsGenericType == true && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
            }

            if (type.IsPrimitive || type.IsEnum)
            {
                return true;
            }

            return type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(Guid)
                || type == typeof(Uri)
                || type == typeof(Version);
        }
    }
}
