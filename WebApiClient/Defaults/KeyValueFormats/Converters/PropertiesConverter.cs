using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

            return
                from p in PropertyDescriptor.GetProperties(context.DataType)
                where p.IsSupportGet && p.IgnoreSerialized == false
                let value = p.GetValue(context.Data)
                let options = context.Options.CloneChange(p.DateTimeFormat)
                let ctx = new ConvertContext(p.Name, value, context.Depths, options)
                select ctx.ToKeyValuePair(); // 只拆解第一层属性则不用递归
        }

        /// <summary>
        /// 表示属性描述
        /// </summary>
        private class PropertyDescriptor
        {
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
            /// 获取是否声明IgnoreSerializedAttribute
            /// </summary>      
            public bool IgnoreSerialized { get; private set; }

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
                var aliasAs = property.GetAttribute<AliasAsAttribute>(true);
                this.Name = aliasAs == null ? property.Name : aliasAs.Name;

                if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                {
                    var formatAttribute = property.GetAttribute<DateTimeFormatAttribute>(true);
                    this.DateTimeFormat = formatAttribute == null ? null : formatAttribute.Format;
                }

                if (property.CanRead == true)
                {
                    this.getter = new PropertyGetter(property);
                }

                this.IgnoreSerialized = property.IsDefined(typeof(IgnoreSerializedAttribute));
                this.IsSupportGet = this.getter != null;
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
