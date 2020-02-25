using WebApiClient.Defaults;

namespace WebApiClient
{
    /// <summary>
    /// Represents a Json string
    /// The string is the json text of the Value object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class JsonString<T> : IJsonString
    {
        /// <summary>
        /// Get type value
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Get type value
        /// </summary>
        object IJsonString.Value
        {
            get => this.Value;
        }

        /// <summary>
        /// Json string
        /// </summary>
        /// <param name="value">Type value corresponding to the string</param>
        public JsonString(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// T type implicit conversion
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JsonString<T>(T value)
        {
            return new JsonString<T>(value);
        }

        /// <summary>
        /// Implicit conversion to type T
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
