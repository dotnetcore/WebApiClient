using System;
using System.Diagnostics;

namespace WebApiClient
{
    /// <summary>
    /// 表示Tags的一个数据项
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public struct TagItem
    {
        /// <summary>
        /// 值
        /// </summary>
        private readonly object value;

        /// <summary>
        /// 获取值是否为NULL
        /// </summary>
        public bool IsNull
        {
            get
            {
                return this.value == null;
            }
        }

        /// <summary>
        /// 获取值
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// ITag的数据项
        /// </summary>
        /// <param name="value">数据</param>
        public TagItem(object value)
        {
            this.value = value;
        }

        /// <summary>
        /// 强制转换为指定类型
        /// 为null则返回目标类型的默认值
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <returns></returns>
        public T As<T>()
        {
            return this.As<T>(default(T));
        }

        /// <summary>
        /// 强制转换为指定类型
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public T As<T>(T defaultValue)
        {
            return this.IsNull ? defaultValue : (T)this.value;
        }

        /// <summary>
        /// 转换为int
        /// </summary>
        /// <returns></returns>
        public int AsInt32()
        {
            return this.As<Int32>();
        }

        /// <summary>
        /// 转换为bool
        /// </summary>
        /// <returns></returns>
        public bool AsBoolean()
        {
            return this.As<Boolean>();
        }

        /// <summary>
        /// 转换为时间
        /// </summary>
        /// <returns></returns>
        public DateTime AsDateTime()
        {
            return this.As<DateTime>();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value?.ToString();
        }
    }
}
