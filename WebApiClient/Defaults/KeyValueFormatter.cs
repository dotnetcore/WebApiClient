using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using WebApiClient.Contexts;
using WebApiClient.DataAnnotations;
using WebApiClient.Interfaces;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 表示默认键值对列化工具
    /// </summary>
    public class KeyValueFormatter : IKeyValueFormatter
    {
        /// <summary>
        /// 序列化参数为键值对
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> IKeyValueFormatter.Serialize(ApiParameterDescriptor parameter, FormatOptions options)
        {
            return this.Serialize(parameter.Name, parameter.Value, options);
        }

        /// <summary>
        /// 序列化对象为键值对
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="obj">对象实例</param>
        /// <param name="options">选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> IKeyValueFormatter.Serialize(string name, object obj, FormatOptions options)
        {
            return this.Serialize(name, obj, options);
        }

        /// <summary>
        /// 序列化对象为键值对
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="obj">对象实例</param>
        /// <param name="options">选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        protected virtual IEnumerable<KeyValuePair<string, string>> Serialize(string name, object obj, FormatOptions options)
        {
            if (options == null)
            {
                options = new FormatOptions();
            }

            var type = obj == null ? null : obj.GetType();
            var descriptor = TypeDescriptor.GetDescriptor(type);
            if (descriptor == null || descriptor.IsSimpleType == true)
            {
                return new[] { this.FormatAsSimple(name, obj, options) };
            }

            if (type == typeof(KeyValuePair<string, string>))
            {
                var kv = (KeyValuePair<string, string>)obj;
                return new[] { this.FormatAsSimple(kv.Key, kv.Value, options) };
            }

            if (type == typeof(KeyValuePair<string, object>))
            {
                var kv = (KeyValuePair<string, object>)obj;
                return new[] { this.FormatAsSimple(kv.Key, kv.Value, options) };
            }

            if (descriptor.IsEnumerableKeyValueOfString == true)
            {
                var dic = obj as IEnumerable<KeyValuePair<string, string>>;
                return this.FormatAsDictionary<string>(dic, options);
            }

            if (descriptor.IsEnumerableKeyValueOfObject == true)
            {
                var dic = obj as IEnumerable<KeyValuePair<string, object>>;
                return this.FormatAsDictionary<object>(dic, options);
            }

            if (descriptor.IsEnumerable == true)
            {
                var enumerable = obj as IEnumerable;
                return this.ForamtAsEnumerable(name, enumerable, options);
            }

            return this.FormatAsComplex(obj, options);
        }

        /// <summary>
        /// 数组转换为键值对
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="enumerable">值</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> ForamtAsEnumerable(string name, IEnumerable enumerable, FormatOptions options)
        {
            return from item in enumerable.Cast<object>()
                   select this.FormatAsSimple(name, item, options);
        }

        /// <summary>
        /// 复杂类型转换为键值对
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> FormatAsComplex(object instance, FormatOptions options)
        {
            return
                from p in PropertyDescriptor.GetProperties(instance.GetType())
                where p.IsSupportGet && p.IgnoreSerialized == false
                let value = p.GetValue(instance)
                let opt = options.CloneChange(p.DateTimeFormat)
                select this.FormatAsSimple(p.AliasName, value, opt);
        }

        /// <summary>
        /// 字典转换为键值对
        /// </summary>
        /// <param name="dic">字典</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> FormatAsDictionary<TValue>(IEnumerable<KeyValuePair<string, TValue>> dic, FormatOptions options)
        {
            return from kv in dic select this.FormatAsSimple(kv.Key, kv.Value, options);
        }

        /// <summary>
        /// 简单类型转换为键值对
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="options">选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        protected KeyValuePair<string, string> FormatAsSimple(string name, object value, FormatOptions options)
        {
            if (string.IsNullOrEmpty(name) == true)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (options.UseCamelCase == true)
            {
                name = FormatOptions.CamelCase(name);
            }

            if (value == null)
            {
                return new KeyValuePair<string, string>(name, null);
            }

            var isDateTime = value is DateTime;
            if (isDateTime == false)
            {
                return new KeyValuePair<string, string>(name, value.ToString());
            }

            // 时间格式转换         
            var dateTime = ((DateTime)value).ToString(options.DateTimeFormat, DateTimeFormatInfo.InvariantInfo);
            return new KeyValuePair<string, string>(name, dateTime);
        }

        /// <summary>
        /// 表示类型描述
        /// </summary>
        private class TypeDescriptor
        {
            /// <summary>
            /// 描述缓存
            /// </summary>
            private static readonly ConcurrentDictionary<Type, TypeDescriptor> descriptorCache;

            /// <summary>
            /// 获取类型是否为简单类型
            /// </summary>
            public bool IsSimpleType { get; private set; }

            /// <summary>
            /// 获取类型是否为可列举类型
            /// </summary>
            public bool IsEnumerable { get; private set; }

            /// <summary>
            /// 获取类型是否为IEnumerable(KeyValuePair(string, object))
            /// </summary>
            public bool IsEnumerableKeyValueOfObject { get; private set; }

            /// <summary>
            /// 获取类型是否为IEnumerable(KeyValuePair(string, string))
            /// </summary>
            public bool IsEnumerableKeyValueOfString { get; private set; }

            /// <summary>
            /// 类型描述
            /// </summary>
            /// <param name="type">类型</param>
            private TypeDescriptor(Type type)
            {
                this.IsSimpleType = type.IsSimple();
                this.IsEnumerable = type.IsInheritFrom<IEnumerable>();
                this.IsEnumerableKeyValueOfObject = type.IsInheritFrom<IEnumerable<KeyValuePair<string, object>>>();
                this.IsEnumerableKeyValueOfString = type.IsInheritFrom<IEnumerable<KeyValuePair<string, string>>>();
            }

            /// <summary>
            /// 静态构造器
            /// </summary>
            static TypeDescriptor()
            {
                descriptorCache = new ConcurrentDictionary<Type, TypeDescriptor>();
            }

            /// <summary>
            /// 获取类型的描述
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns></returns>
            public static TypeDescriptor GetDescriptor(Type type)
            {
                if (type == null)
                {
                    return null;
                }
                return descriptorCache.GetOrAdd(type, (t) => new TypeDescriptor(t));
            }
        }

        /// <summary>
        /// 表示属性描述
        /// </summary>
        private class PropertyDescriptor
        {
            /// <summary>
            /// 获取器
            /// </summary>
            private readonly Getter getter;

            /// <summary>
            /// 获取属性别名或名称
            /// </summary>
            public string AliasName { get; private set; }

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
                this.AliasName = aliasAs == null ? property.Name : aliasAs.Name;

                if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                {
                    var formatAttribute = property.GetAttribute<DateTimeFormatAttribute>(true);
                    this.DateTimeFormat = formatAttribute == null ? null : formatAttribute.Format;
                }

                if (property.CanRead == true)
                {
                    this.getter = Getter.Create(property);
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

            /// <summary>
            /// 表示属性的Get方法抽象类
            /// </summary>
            private abstract class Getter
            {
                /// <summary>
                /// 创建属性的Get方法
                /// </summary>
                /// <param name="property">属性</param>
                /// <returns></returns>
                public static Getter Create(PropertyInfo property)
                {
                    var getterType = typeof(GenericGetter<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
                    return Activator.CreateInstance(getterType, property) as Getter;
                }

                /// <summary>
                /// 执行Get方法
                /// </summary>
                /// <param name="instance">实例</param>
                /// <returns></returns>
                public abstract object Invoke(object instance);

                /// <summary>
                /// 表示属性的Get方法
                /// </summary>
                /// <typeparam name="TTarget">属性所在的类</typeparam>
                /// <typeparam name="TResult">属性的返回值</typeparam>
                private class GenericGetter<TTarget, TResult> : Getter
                {
                    /// <summary>
                    /// get方法的委托
                    /// </summary>
                    private readonly Func<TTarget, TResult> getFunc;

                    /// <summary>
                    /// 属性的Get方法
                    /// </summary>
                    /// <param name="property">属性</param>
                    public GenericGetter(PropertyInfo property)
                    {
                        var getMethod = property.GetGetMethod();
                        this.getFunc = (Func<TTarget, TResult>)getMethod.CreateDelegate(typeof(Func<TTarget, TResult>), null);
                    }

                    /// <summary>
                    /// 执行Get方法
                    /// </summary>
                    /// <param name="instance">实例</param>
                    /// <returns></returns>
                    public override object Invoke(object instance)
                    {
                        return this.getFunc.Invoke((TTarget)instance);
                    }
                }
            }
        }
    }
}
