using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示格式化选项
    /// </summary>
    public class FormatOptions
    {
        /// <summary>
        /// 日期时间格式
        /// </summary>
        private string dateTimeFormat;

        /// <summary>
        /// 获取或设置是否使用骆驼命名
        /// 默认为false
        /// </summary>
        public bool UseCamelCase { get; set; }

        /// <summary>
        /// 获取或设置时期时间格式
        /// 默认为本地日期时间格式
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public string DateTimeFormat
        {
            get
            {
                return this.dateTimeFormat;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException();
                }
                this.dateTimeFormat = value;
            }
        }

        /// <summary>
        /// 格式化选项
        /// </summary>
        public FormatOptions()
        {
            this.UseCamelCase = false;
            this.DateTimeFormat = DateTimeFormats.GetLocalDateTimeFormat();
        }

        /// <summary>
        /// 如果新的日期时间格式有变化
        /// 则克隆并使用新的datetimeFormat
        /// </summary>
        /// <param name="datetimeFormat">日期时间格式</param>
        /// <returns></returns>
        public FormatOptions CloneChange(string datetimeFormat)
        {
            if (string.IsNullOrEmpty(datetimeFormat) || string.Equals(this.DateTimeFormat, datetimeFormat))
            {
                return this;
            }

            return new FormatOptions
            {
                DateTimeFormat = datetimeFormat,
                UseCamelCase = this.UseCamelCase
            };
        }

        /// <summary>
        /// 骆驼命名
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static string CamelCase(string name)
        {
            if (string.IsNullOrEmpty(name) == true)
            {
                return name;
            }

            return Regex.Replace(name, @"^[A-Z]+", m =>
            {
                if (m.Success == false)
                {
                    return name;
                }
                if (m.Value.Length > 1)
                {
                    var charArray = m.Value.ToLower().ToCharArray();
                    charArray[charArray.Length - 1] = Char.ToUpper(charArray[charArray.Length - 1]);
                    return new string(charArray);
                }
                return m.Value.ToLower();
            });
        }
    }
}
