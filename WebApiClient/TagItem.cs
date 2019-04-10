using System;
using System.Diagnostics;

namespace WebApiClient
{
    /// <summary>
    /// 表示Tags的一个数据项
    /// </summary>
    [DebuggerDisplay("HasValue = {HasValue}")]
    public struct TagItem
    {
        /// <summary>
        /// 获取值
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 获取是否有值
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// 获取值是否为NULL
        /// </summary>
        public bool IsNullValue
        {
            get
            {
                return this.Value == null;
            }
        }

        /// <summary>
        /// 获取没有值的TagItem
        /// </summary>
        public static TagItem NoValue { get; } = new TagItem();

        /// <summary>
        /// 创建有值的数据项
        /// </summary>
        /// <param name="value">数据</param>
        public TagItem(object value)
        {
            this.Value = value;
            this.HasValue = true;
        }

        /// <summary>
        /// 强制转换为指定类型
        /// 为null则返回目标类型的默认值
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <returns></returns>
        public T As<T>()
        {
            return this.As(default(T));
        }

        /// <summary>
        /// 将值强制转换为指定类型
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public T As<T>(T defaultValue)
        {
            return this.IsNullValue ? defaultValue : (T)this.Value;
        }

        /// <summary>
        /// 将值转换为int
        /// </summary>
        /// <returns></returns>
        public int AsInt32()
        {
            return this.As<int>();
        }

        /// <summary>
        /// 将值转换为bool
        /// </summary>
        /// <returns></returns>
        public bool AsBoolean()
        {
            return this.As<bool>();
        }

        /// <summary>
        /// 将值转换为时间
        /// </summary>
        /// <returns></returns>
        public DateTime AsDateTime()
        {
            return this.As<DateTime>();
        }

        /// <summary>
        /// 将值转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value?.ToString();
        }
    }
}
