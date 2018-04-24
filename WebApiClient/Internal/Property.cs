using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace WebApiClient
{
    /// <summary>
    /// 表示属性
    /// </summary>
    [DebuggerDisplay("{property}")]
    class Property
    {
        /// <summary>
        /// 属性信息
        /// </summary>
        private readonly PropertyInfo property;

        /// <summary>
        /// 获取器
        /// </summary>
        public PropertyGetter Getter { get; private set; }

        /// <summary>
        /// 设置器
        /// </summary>
        public PropertySetter Setter { get; private set; }

        /// <summary>
        /// 表示属性
        /// </summary>
        /// <param name="property">属性</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Property(PropertyInfo property)
        {
            this.property = property;
            this.Getter = new PropertyGetter(property);
            this.Setter = new PropertySetter(property);
        }

        /// <summary>
        /// 类型属性缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Property[]> cache = new ConcurrentDictionary<Type, Property[]>();

        /// <summary>
        /// 获取类型的属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Property[] GetProperties(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return cache.GetOrAdd(type, t
                => t
                .GetProperties()
                .Where(item => item.CanRead && item.CanWrite)
                .Select(item => new Property(item))
                .ToArray());
        }

        /// <summary>
        /// 将source的属性复制给target
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="ignoreException">是否忽略异常</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static int CopyProperties<T>(T source, T target, bool ignoreException = true)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var count = 0;
            var properties = GetProperties(source.GetType());
            foreach (var item in properties)
            {
                try
                {
                    var value = item.Getter.Invoke(source);
                    item.Setter.Invoke(target, value);
                    count = count + 1;
                }
                catch (Exception ex)
                {
                    if (ignoreException == false)
                    {
                        throw ex;
                    }
                }
            }
            return count;
        }
    }
}
