using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示属性在JsonFormatter或KeyValueFormatter序列化时使用的日期时间格式
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class DateTimeFormatAttribute : Attribute
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
