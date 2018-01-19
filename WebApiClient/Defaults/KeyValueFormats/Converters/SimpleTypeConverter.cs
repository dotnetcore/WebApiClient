using System;
using System.Collections.Generic;

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
        public sealed override IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context)
        {
            if (this.IsSupported(context.DataType) == true)
            {
                return context.ToKeyValuePairs();
            }
            return this.Next.Invoke(context);
        }

        /// <summary>
        /// 获取类型是否为简单类型
        /// 从而直接调用context.ToKeyValuePair()
        /// </summary>
        /// <param name="dataType">类型</param>
        /// <returns></returns>
        protected virtual bool IsSupported(Type dataType)
        {
            if (dataType.IsGenericType == true && dataType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                dataType = Nullable.GetUnderlyingType(dataType);
            }

            return dataType.IsPrimitive
                || dataType.IsEnum
                || dataType == typeof(string)
                || dataType == typeof(decimal)
                || dataType == typeof(DateTime)
                || dataType == typeof(Guid)
                || dataType == typeof(Uri)
                || dataType == typeof(Version);
        }
    }
}
