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
        private readonly Method geter;

        /// <summary>
        /// 获取属性别名或名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取是否支持Get操作
        /// </summary>
        public bool IsSupportGet { get; private set; }

        /// <summary>
        /// 获取是否声明KeyValueIgnoreAttribute
        /// </summary>
        public bool IsKeyValueIgnore { get; private set; }

        /// <summary>
        /// 属性
        /// </summary>
        /// <param name="property">属性信息</param>
        public KeyValueProperty(PropertyInfo property)
        {
            var keyAlias = property.GetAttribute<KeyAliasAttribute>(true);
            this.Name = keyAlias == null ? property.Name : keyAlias.Alias;

            var getMethod = property.GetGetMethod();
            if (getMethod != null)
            {
                this.geter = new Method(getMethod);
            }

            this.IsSupportGet = this.geter != null;
            this.IsKeyValueIgnore = property.IsDefined(typeof(KeyValueIgnoreAttribute));
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
            return this.geter.Invoke(instance, null);
        }

        /// <summary>
        /// 类型属性的Setter缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, KeyValueProperty[]> cached = new ConcurrentDictionary<Type, KeyValueProperty[]>();

        /// <summary>
        /// 从类型的属性获取属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static KeyValueProperty[] GetProperties(Type type)
        {
            return cached.GetOrAdd(type, t =>
              t.GetProperties().Select(p => new KeyValueProperty(p)).ToArray()
           );
        }
    }
}