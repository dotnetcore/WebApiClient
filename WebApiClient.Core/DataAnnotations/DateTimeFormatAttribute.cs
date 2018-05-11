using System;

namespace WebApiClient.DataAnnotations
{
    /// <summary>
    /// DataAnnotation
    /// 表示序列时此属性使用的日期时间格式
    /// 默认适用于JsonFormat和KeyValueFormat
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class DateTimeFormatAttribute : DataAnnotationAttribute
    {
        /// <summary>
        /// 获取格式
        /// </summary>
        public string Format { get; private set; }

        /// <summary>
        /// 日期时间格式
        /// </summary>
        /// <param name="format">格式</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DateTimeFormatAttribute(string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                throw new ArgumentNullException(nameof(format));
            }
            this.Format = format;
        }
    }
}
