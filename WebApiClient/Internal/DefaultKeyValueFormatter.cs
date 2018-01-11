using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 默认参数值序列化工具
    /// </summary>
    class DefaultKeyValueFormatter : IKeyValueFormatter
    {
        /// <summary>
        /// 序列化模型对象为键值对
        /// </summary>
        /// <param name="model">对象</param>
        /// <param name="datetimeFormat">日期时间格式</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> Serialize(object model, string datetimeFormat)
        {
            if (model == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            var dicObj = model as IDictionary<string, object>;
            if (dicObj != null)
            {
                return this.FormatAsDictionary<object>(dicObj, datetimeFormat);
            }

            var dicString = model as IDictionary<string, string>;
            if (dicString != null)
            {
                return this.FormatAsDictionary<string>(dicString, datetimeFormat);
            }

            return this.FormatAsComplex(model, datetimeFormat);
        }

        /// <summary>
        /// 将参数值序列化为键值对
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="datetimeFormat">时期格式日期时间格式</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> Serialize(ApiParameterDescriptor parameter, string datetimeFormat)
        {
            if (parameter.IsSimpleType == true)
            {
                var kv = this.FormatAsSimple(parameter.Name, parameter.Value, datetimeFormat);
                return new[] { kv };
            }

            if (parameter.IsDictionaryOfString == true)
            {
                var dic = parameter.Value as IDictionary<string, string>;
                return this.FormatAsDictionary<string>(dic, datetimeFormat);
            }

            if (parameter.IsDictionaryOfObject == true)
            {
                var dic = parameter.Value as IDictionary<string, object>;
                return this.FormatAsDictionary<object>(dic, datetimeFormat);
            }

            if (parameter.IsEnumerable == true)
            {
                var enumerable = parameter.Value as IEnumerable;
                return this.ForamtAsEnumerable(parameter.Name, enumerable, datetimeFormat);
            }

            return this.FormatAsComplex(parameter.Value, datetimeFormat);
        }

        /// <summary>
        /// 数组为键值对
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="enumerable">值</param>
        /// <param name="datetimeFormat">日期时间格式</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> ForamtAsEnumerable(string name, IEnumerable enumerable, string datetimeFormat)
        {
            if (enumerable == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return from item in enumerable.Cast<object>()
                   select this.FormatAsSimple(name, item, datetimeFormat);
        }

        /// <summary>
        /// 复杂类型为键值对
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="datetimeFormat">日期时间格式</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> FormatAsComplex(object instance, string datetimeFormat)
        {
            if (instance == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return
                from p in KeyValueProperty.GetProperties(instance.GetType())
                where p.IsSupportGet && p.IsKeyValueIgnore == false
                let value = p.GetValue(instance)
                let format = p.DateTimeFormat == null ? datetimeFormat : p.DateTimeFormat
                select this.FormatAsSimple(p.Name, value, format);
        }

        /// <summary>
        /// 字典转换为键值对
        /// </summary>
        /// <param name="dic">字典</param>
        /// <param name="datetimeFormat">日期时间格式</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> FormatAsDictionary<TValue>(IDictionary<string, TValue> dic, string datetimeFormat)
        {
            if (dic == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }
            return from kv in dic select this.FormatAsSimple(kv.Key, kv.Value, datetimeFormat);
        }

        /// <summary>
        /// 简单类型为键值对
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="datetimeFormat">日期时间格式，null则ISO 8601</param>
        /// <returns></returns>
        private KeyValuePair<string, string> FormatAsSimple(string name, object value, string datetimeFormat)
        {
            if (value == null)
            {
                return new KeyValuePair<string, string>(name, null);
            }

            var isDateTime = value is DateTime;
            if (isDateTime == false)
            {
                return new KeyValuePair<string, string>(name, value.ToString());
            }

            // 时间格式转换
            if (string.IsNullOrEmpty(datetimeFormat) == true)
            {
                datetimeFormat = DateTimeFormats.ISO8601_WithMillisecond;
            }
            var dateTime = ((DateTime)value).ToString(datetimeFormat, DateTimeFormatInfo.InvariantInfo);
            return new KeyValuePair<string, string>(name, dateTime);
        }
    }
}
