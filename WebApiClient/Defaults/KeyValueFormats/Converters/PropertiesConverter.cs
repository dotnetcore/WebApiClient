using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebApiClient.DataAnnotations;

namespace WebApiClient.Defaults.KeyValueFormats.Converters
{
    /// <summary>
    /// 表示属性拆解转换器
    /// </summary>
    public class PropertiesConverter : ConverterBase
    {
        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        public override IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context)
        {
            // 无条件解析为属性
            // 因为其它转换器都无法解析此类型
            // 只拆解第一层属性则不用递归
            return from ctx in this.GetPropertiesContexts(context)
                   select ctx.ToKeyValuePair();
        }


        /// <summary>
        /// 解析context的Data的属性
        /// 返回多个属性组成的ConvertContext
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        protected IEnumerable<ConvertContext> GetPropertiesContexts(ConvertContext context)
        {
            return
                from p in PropertyDescriptor.GetProperties(context.DataType)
                where p.IsSupportGet && p.IgnoreSerialized == false
                let value = p.GetValue(context.Data)
                where (p.IgnoreWhenNull && value == null) == false
                let options = context.Options.CloneChange(p.DateTimeFormat)
                select new ConvertContext(p.Name, value, context.Depths, options);
        }

        /// <summary>
        /// 表示属性描述
        /// </summary>
        private class PropertyDescriptor
        {
            /// <summary>
            /// 序列化范围
            /// </summary>
            private const FormatScope keyValueFormatScope = FormatScope.KeyValueFormat;

            /// <summary>
            /// 获取器
            /// </summary>
            private readonly PropertyGetter getter;

            /// <summary>
            /// 获取属性名称
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// 获取声明的DateTimeFormatAttribute的Format
            /// </summary>
            public string DateTimeFormat { get; private set; }

            /// <summary>
            /// 获取是否忽略序列化
            /// </summary>      
            public bool IgnoreSerialized { get; private set; }

            /// <summary>
            /// 获取当值为null时是否忽略序列化
            /// </summary>
            public bool IgnoreWhenNull { get; private set; }

            /// <summary>
            /// 获取是否支持Get操作
            /// </summary>
            public bool IsSupportGet { get; private set; }

            /// <summary>
            /// 属性
            /// </summary>
            /// <param name="property">属性信息</param>
            private PropertyDescriptor(PropertyInfo property)
            {
                var aliasAsAttribute = property.GetAttribute<AliasAsAttribute>(true);
                if (aliasAsAttribute != null && aliasAsAttribute.IsDefinedScope(keyValueFormatScope))
                {
                    this.Name = aliasAsAttribute.Name;
                }
                else
                {
                    this.Name = property.Name;
                }

                var datetimeFormatAttribute = property.GetCustomAttribute<DateTimeFormatAttribute>();
                if (datetimeFormatAttribute != null && datetimeFormatAttribute.IsDefinedScope(keyValueFormatScope))
                {
                    this.DateTimeFormat = datetimeFormatAttribute.Format;
                }

                if (property.CanRead == true)
                {
                    this.getter = new PropertyGetter(property);
                    this.IsSupportGet = true;
                }

                this.IgnoreSerialized = property.IsDefinedFormatScope<IgnoreSerializedAttribute>(keyValueFormatScope);
                this.IgnoreWhenNull = property.IsDefinedFormatScope<IgnoreWhenNullAttribute>(keyValueFormatScope);
            }

            /// <summary>
            /// 获取属性的值
            /// </summary>
            /// <param name="instance">实例</param>
            /// <exception cref="NotSupportedException"></exception>
            /// <returns></returns>
            public object GetValue(object instance)
            {
                if (this.IsSupportGet == false)
                {
                    throw new NotSupportedException();
                }
                return this.getter.Invoke(instance);
            }

            /// <summary>
            /// 类型的属性描述缓存
            /// </summary>
            private static readonly ConcurrentDictionary<Type, PropertyDescriptor[]> propertyCached = new ConcurrentDictionary<Type, PropertyDescriptor[]>();

            /// <summary>
            /// 从类型的属性获取属性
            /// </summary>
            /// <param name="classType">类型</param>
            /// <returns></returns>
            public static PropertyDescriptor[] GetProperties(Type classType)
            {
                return propertyCached.GetOrAdd(classType, t => t.GetProperties().Select(p => new PropertyDescriptor(p)).ToArray());
            }
        }
    }
}
