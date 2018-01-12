using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebApiClient
{
    /// <summary>
    /// 表示属性
    /// </summary>
    class KeyValueProperty
    {
        /// <summary>
        /// 获取器
        /// </summary>
        private readonly Getter getter;

        /// <summary>
        /// 获取属性别名或名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取是否支持Get操作
        /// </summary>
        public bool IsSupportGet { get; private set; }

        /// <summary>
        /// 获取是否声明IgnoreSerializedAttribute
        /// </summary>      
        public bool IgnoreSerialized { get; private set; }

        /// <summary>
        /// 获取声明的DateTimeFormatAttribute的Format
        /// </summary>
        public string DateTimeFormat { get; private set; }

        /// <summary>
        /// 属性
        /// </summary>
        /// <param name="property">属性信息</param>
        private KeyValueProperty(PropertyInfo property)
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
                this.getter = Getter.Create(property);
            }

            this.IsSupportGet = this.getter != null;
            this.IgnoreSerialized = property.IsDefined(typeof(IgnoreSerializedAttribute));
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
        /// 类型属性的Setter缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, KeyValueProperty[]> cached = new ConcurrentDictionary<Type, KeyValueProperty[]>();

        /// <summary>
        /// 从类型的属性获取属性
        /// </summary>
        /// <param name="classType">类型</param>
        /// <returns></returns>
        public static KeyValueProperty[] GetProperties(Type classType)
        {
            return cached.GetOrAdd(classType, t =>
              t.GetProperties().Select(p => new KeyValueProperty(p)).ToArray()
           );
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