using WebApiClient.Defaults;

namespace WebApiClient
{
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
        object IJsonString.Value
        {
            get => this.Value;
        }

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
            if (value == null)
            {
                return default(T);
            }
            return value.Value;
        }
    }
}
