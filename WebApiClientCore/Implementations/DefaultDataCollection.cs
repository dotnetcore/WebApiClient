using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示数据集合
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public class DefaultDataCollection : IDataCollection
    {
        /// <summary>
        /// 数据字典
        /// </summary>
        private Dictionary<object, object?>? dictionary;

        /// <summary>
        /// 获取集合元素的数量
        /// </summary>
        public int Count => this.dictionary == null ? 0 : this.dictionary.Count;

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Set(object key, object? value)
        {
            if (this.dictionary == null)
            {
                this.dictionary = new Dictionary<object, object?>();
            }
            this.dictionary[key] = value;
        }

        /// <summary>
        /// 返回是否包含指定的key
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool ContainsKey(object key)
        {
            return this.dictionary != null && this.dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 读取指定的键并尝试转换为目标类型
        /// 失败则返回目标类型的default值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        [return: MaybeNull]
        public TValue Get<TValue>(object key)
        {
            this.TryGetValue<TValue>(key, out var value);
            return value;
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool TryGetValue<TValue>(object key, [MaybeNullWhen(false)] out TValue value)
        {
            if (this.TryGetValue(key, out var objValue) && objValue is TValue tValue)
            {
                value = tValue;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool TryGetValue(object key, out object? value)
        {
            if (this.dictionary == null)
            {
                value = default;
                return false;
            }
            else
            {
                return this.dictionary.TryGetValue(key, out value);
            }
        }

        /// <summary>
        /// 尝试移除
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool TryRemove(object key, out object? value)
        {
            if (this.dictionary == null)
            {
                value = default;
                return false;
            }
            else
            {
                return this.dictionary.Remove(key, out value);
            }
        }

        /// <summary>
        /// 调试视图
        /// </summary>
        private class DebugView
        {
            /// <summary>
            /// 查看的对象
            /// </summary> 
            private readonly Dictionary<object, object?>? dictionary;

            /// <summary>
            /// 查看的内容
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public KeyValuePair<object, object?>[] Values
            {
                get
                {
                    return this.dictionary == null
                        ? Array.Empty<KeyValuePair<object, object?>>()
                        : this.dictionary.ToArray();
                }
            }

            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="target">查看的对象</param>
            public DebugView(DefaultDataCollection target)
            {
                this.dictionary = target.dictionary;
            }
        }
    }
}
