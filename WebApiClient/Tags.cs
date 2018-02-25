using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace WebApiClient
{
    /// <summary>
    /// 表示自定义数据的存储和访问容器
    /// 线程安全类型
    /// </summary>
    [DebuggerDisplay("Id = {Id}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public sealed class Tags
    {
        /// <summary>
        /// 同步锁
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// 数据字典
        /// </summary>
        private readonly Lazy<Dictionary<string, object>> lazy;

        /// <summary>
        /// 定义数据的存储和访问容器
        /// </summary>
        public Tags()
        {
            this.lazy = new Lazy<Dictionary<string, object>>(() => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), true);
        }

        /// <summary>
        /// 获取或设置唯一标识符
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public TagItem this[string key]
        {
            get
            {
                return this.Get(key);
            }
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public TagItem Get(string key)
        {
            lock (this.syncRoot)
            {
                this.lazy.Value.TryGetValue(key, out object value);
                return new TagItem(value);
            }
        }

        /// <summary>
        /// 删除键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if (this.lazy.IsValueCreated == false)
            {
                return false;
            }

            lock (this.syncRoot)
            {
                return this.lazy.Value.Remove(key);
            }
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Set(string key, object value)
        {
            lock (this.syncRoot)
            {
                this.lazy.Value[key] = value;
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
            private IEnumerable<KeyValuePair<string, object>> value;

            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="target">查看的对象</param>
            public DebugView(Tags target)
            {
                this.value = target.lazy.IsValueCreated ? target.lazy.Value : null;
            }

            /// <summary>
            /// 查看的内容
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public KeyValuePair<string, object>[] Values
            {
                get
                {
                    if (this.value == null)
                    {
                        return new KeyValuePair<string, object>[0];
                    }
                    return this.value.ToArray();
                }
            }
        }
    }
}
