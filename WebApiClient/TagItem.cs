using System;
using System.Diagnostics;

namespace WebApiClient
{
    /// <summary>
    /// A data item representing Tags
    /// </summary>
    [DebuggerDisplay("HasValue = {HasValue}")]
    public struct TagItem
    {
        /// <summary>
        /// Get value
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Has Value?
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// Whether the value is NULL
        /// </summary>
        public bool IsNullValue
        {
            get => this.Value == null;
        }

        /// <summary>
        /// Get TagItem without value
        /// </summary>
        public static TagItem NoValue { get; } = new TagItem();

        /// <summary>
        /// Create valued data items
        /// </summary>
        /// <param name="value">data</param>
        public TagItem(object value)
        {
            this.Value = value;
            this.HasValue = true;
        }

        /// <summary>
        /// Cast to specified type
        /// Null returns the default value of the target type
        /// </summary>
        /// <typeparam name="T">Specified type</typeparam>
        /// <returns></returns>
        public T As<T>()
        {
            return this.As(default(T));
        }

        /// <summary>
        /// Cast value to specified type
        /// </summary>
        /// <typeparam name="T">Specified type</typeparam>
        /// <param name="defaultValue">Defaults</param>
        /// <returns></returns>
        public T As<T>(T defaultValue)
        {
            return this.IsNullValue ? defaultValue : (T)this.Value;
        }

        /// <summary>
        /// Convert value to int
        /// </summary>
        /// <returns></returns>
        public int AsInt32()
        {
            return this.As<int>();
        }

        /// <summary>
        /// Convert value to bool
        /// </summary>
        /// <returns></returns>
        public bool AsBoolean()
        {
            return this.As<bool>();
        }

        /// <summary>
        /// Convert value to DateTime
        /// </summary>
        /// <returns></returns>
        public DateTime AsDateTime()
        {
            return this.As<DateTime>();
        }

        /// <summary>
        /// Convert value to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value?.ToString();
        }
    }
}
