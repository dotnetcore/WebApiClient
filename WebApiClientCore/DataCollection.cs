using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示延时初始化的数据集合
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public class DataCollection
    {
        /// <summary>
        /// 数据字典
        /// </summary>
        private readonly Lazy<Dictionary<object, object?>> lazy;

        /// <summary>
        /// 获取集合元素的数量
        /// </summary>
        public int Count => this.lazy.IsValueCreated ? this.lazy.Value.Count : 0;

        /// <summary>
        /// 延时初始化的数据集合   
        /// </summary>
        public DataCollection()
        {
            this.lazy = new Lazy<Dictionary<object, object?>>(() => new Dictionary<object, object?>());
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Set(object key, object? value)
        {
            this.lazy.Value[key] = value;
        }

        /// <summary>
        /// 返回是否包含指定的key
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool ContainsKey(object key)
        {
            return this.lazy.IsValueCreated ? this.lazy.Value.ContainsKey(key) : false;
        }

        /// <summary>
        /// 读取指定的键并强制转换为目标类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T Get<T>(object key)
        {
#nullable disable
            if (this.TryGetValue(key, out var value))
            {
                return value == null ? default : (T)value;
            }
            return default;
#nullable enable
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool TryGetValue(object key, out object? value)
        {
            if (this.lazy.IsValueCreated == false)
            {
                value = default;
                return false;
            }

            return this.lazy.Value.TryGetValue(key, out value);
        }

        /// <summary>
        /// 尝试移除
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool TryRemove(object key, out object? value)
        {
            if (this.lazy.IsValueCreated == false)
            {
                value = default;
                return false;
            }

            return this.lazy.Value.Remove(key, out value);
        }

        /// <summary>
        /// 调试视图
        /// </summary>
        private class DebugView
        {
            /// <summary>
            /// 查看的对象
            /// </summary> 
            private readonly DataCollection target;

            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="target">查看的对象</param>
            public DebugView(DataCollection target)
            {
                this.target = target;
            }

            /// <summary>
            /// 查看的内容
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public KeyValuePair<object, object?>[] Values
            {
                get
                {
                    return this.target.lazy.IsValueCreated
                        ? this.target.lazy.Value.ToArray()
                        : Array.Empty<KeyValuePair<object, object?>>();
                }
            }
        }
    }
}
