using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示类型描述
    /// </summary>
    class TypeDescriptor
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
        /// 获取类型是否为IDictionaryOf(string,object)
        /// </summary>
        public bool IsDictionaryOfObject { get; private set; }

        /// <summary>
        /// 获取类型是否为IDictionaryOf(string,string)
        /// </summary>
        public bool IsDictionaryOfString { get; private set; }

        /// <summary>
        /// 类型描述
        /// </summary>
        /// <param name="type">类型</param>
        private TypeDescriptor(Type type)
        {
            this.IsSimpleType = type.IsSimple();
            this.IsEnumerable = type.IsInheritFrom<IEnumerable>();
            this.IsDictionaryOfObject = type.IsInheritFrom<IDictionary<string, object>>();
            this.IsDictionaryOfString = type.IsInheritFrom<IDictionary<string, string>>();
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
}
