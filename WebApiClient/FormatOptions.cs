using System;
using System.Globalization;

namespace WebApiClient
{
    /// <summary>
    /// Represents formatting options
    /// </summary>
    public class FormatOptions
    {
        /// <summary>
        /// Date time format
        /// </summary>
        private string datetimeFormat;

        /// <summary>
        /// Gets or sets whether to use camel naming when serializing    
        /// Default is false
        /// </summary>
        public bool UseCamelCase { get; set; }

        /// <summary>
        /// Gets or sets whether to ignore serialization of null value attributes
        /// Default is false
        /// </summary>
        public bool IgnoreNullProperty { get; set; }

        /// <summary>
        /// Gets or sets the format used by the serialized DateTime type
        /// Default is local datetime format
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public string DateTimeFormat
        {
            get
            {
                if (this.datetimeFormat == null)
                {
                    this.datetimeFormat = DateTimeFormats.LocalDateTimeFormat;
                }
                return this.datetimeFormat;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(DateTimeFormat));
                }
                this.datetimeFormat = value;
            }
        }

        /// <summary>
        /// When datetimeFormat is not null and changes
        /// Then clone and use the new datetimeFormat
        /// </summary>
        /// <param name="datetimeFormat">Date time format</param>
        /// <returns></returns>
        public FormatOptions CloneChange(string datetimeFormat)
        {
            if (string.IsNullOrEmpty(datetimeFormat) == true)
            {
                return this;
            }

            if (datetimeFormat.Equals(this.DateTimeFormat) == true)
            {
                return this;
            }

            return new FormatOptions
            {
                DateTimeFormat = datetimeFormat,
                IgnoreNullProperty = this.IgnoreNullProperty,
                UseCamelCase = this.UseCamelCase
            };
        }

        /// <summary>
        /// Format time as text
        /// </summary>
        /// <param name="datetime">time</param>
        /// <returns></returns>
        public string FormatDateTime(DateTime? datetime)
        {
            if (datetime.HasValue == false)
            {
                return null;
            }
            return datetime.Value.ToString(this.DateTimeFormat, DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Camel naming
        /// </summary>
        /// <param name="name">name</param>
        /// <returns></returns>
        public static string CamelCase(string name)
        {
            if (string.IsNullOrEmpty(name) || char.IsUpper(name[0]) == false)
            {
                return name;
            }

            var charArray = name.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                if (i == 1 && char.IsUpper(charArray[i]) == false)
                {
                    break;
                }

                var hasNext = (i + 1 < charArray.Length);
                if (i > 0 && hasNext && !char.IsUpper(charArray[i + 1]))
                {
                    if (char.IsSeparator(charArray[i + 1]))
                    {
                        charArray[i] = char.ToLowerInvariant(charArray[i]);
                    }
                    break;
                }
                charArray[i] = char.ToLowerInvariant(charArray[i]);
            }
            return new string(charArray);
        }
    }
}
