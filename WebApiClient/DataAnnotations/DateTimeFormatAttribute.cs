using System;
using System.Reflection;

namespace WebApiClient.DataAnnotations
{
    /// <summary>
    /// DataAnnotation
    /// 表示序列时此属性使用的日期时间格式
    /// 默认适用于JsonFormat和KeyValueFormat
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DateTimeFormatAttribute : DataAnnotationAttribute
    {
        /// <summary>
        /// 获取格式
        /// </summary>
        public string Format { get; }

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

        /// <summary>
        /// 执行特性
        /// </summary>
        /// <param name="member">成员</param>
        /// <param name="annotations">注解信息</param>
        public override void Invoke(MemberInfo member, Annotations annotations)
        {
            annotations.DateTimeFormat = this.Format;
        }
    }
}
