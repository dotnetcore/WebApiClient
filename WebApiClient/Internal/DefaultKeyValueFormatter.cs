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
        /// <param name="options">选项</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> Serialize(object model, FormatOptions options)
        {
            if (model == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            if (options == null)
            {
                options = new FormatOptions();
            }

            var dicObj = model as IDictionary<string, object>;
            if (dicObj != null)
            {
                return this.FormatAsDictionary<object>(dicObj, options);
            }

            var dicString = model as IDictionary<string, string>;
            if (dicString != null)
            {
                return this.FormatAsDictionary<string>(dicString, options);
            }

            return this.FormatAsComplex(model, options);
        }

        /// <summary>
        /// 将参数值序列化为键值对
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> Serialize(ApiParameterDescriptor parameter, FormatOptions options)
        {
            if (options == null)
            {
                options = new FormatOptions();
            }

            if (parameter.IsSimpleType == true)
            {
                var kv = this.FormatAsSimple(parameter.Name, parameter.Value, options);
                return new[] { kv };
            }

            if (parameter.IsDictionaryOfString == true)
            {
                var dic = parameter.Value as IDictionary<string, string>;
                return this.FormatAsDictionary<string>(dic, options);
            }

            if (parameter.IsDictionaryOfObject == true)
            {
                var dic = parameter.Value as IDictionary<string, object>;
                return this.FormatAsDictionary<object>(dic, options);
            }

            if (parameter.IsEnumerable == true)
            {
                var enumerable = parameter.Value as IEnumerable;
                return this.ForamtAsEnumerable(parameter.Name, enumerable, options);
            }

            return this.FormatAsComplex(parameter.Value, options);
        }

        /// <summary>
        /// 数组为键值对
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="enumerable">值</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> ForamtAsEnumerable(string name, IEnumerable enumerable, FormatOptions options)
        {
            if (enumerable == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return from item in enumerable.Cast<object>()
                   select this.FormatAsSimple(name, item, options);
        }

        /// <summary>
        /// 复杂类型为键值对
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> FormatAsComplex(object instance, FormatOptions options)
        {
            if (instance == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return
                from p in KeyValueProperty.GetProperties(instance.GetType())
                where p.IsSupportGet && p.IgnoreSerialized == false
                let value = p.GetValue(instance)
                let opt = options.CloneChange(p.DateTimeFormat)
                select this.FormatAsSimple(p.Name, value, opt);
        }

        /// <summary>
        /// 字典转换为键值对
        /// </summary>
        /// <param name="dic">字典</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> FormatAsDictionary<TValue>(IDictionary<string, TValue> dic, FormatOptions options)
        {
            if (dic == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }
            return from kv in dic select this.FormatAsSimple(kv.Key, kv.Value, options);
        }

        /// <summary>
        /// 简单类型为键值对
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        private KeyValuePair<string, string> FormatAsSimple(string name, object value, FormatOptions options)
        {
            if (options.UseCamelCase == true)
            {
                name = FormatOptions.CamelCase(name);
            }

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
            var dateTime = ((DateTime)value).ToString(options.DateTimeFormat, DateTimeFormatInfo.InvariantInfo);
            return new KeyValuePair<string, string>(name, dateTime);
        }
    }
}
