using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义JsonString的接口
    /// </summary>
    interface IJsonString
    {
        /// <summary>
        /// 获取值
        /// </summary>
        object? Value { get; }

        /// <summary>
        /// 获取值类型
        /// </summary>
        Type ValueType { get; }
    }

    /// <summary>
    /// 表示Json字符串
    /// 该字符串为Value对象的json文本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class JsonString<T> : IJsonString
    {
        /// <summary>
        /// 获取类型值
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// 获取类型值
        /// </summary>
        object? IJsonString.Value => this.Value;

        /// <summary>
        /// 获取值类型
        /// </summary>
        Type IJsonString.ValueType => typeof(T);

        /// <summary>
        /// Json字符串
        /// </summary>
        /// <param name="value">字符串对应的类型值</param>
        public JsonString(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// T类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JsonString<T>(T value)
        {
            return new JsonString<T>(value);
        }

        /// <summary>
        /// 类型隐式转换为T
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator T(JsonString<T> value)
        {
            return value.Value;
        }
    }
}
