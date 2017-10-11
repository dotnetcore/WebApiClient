using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 默认参数值序列化工具
    /// </summary>
    class DefaultKeyValueFormatter : IKeyValueFormatter
    {
        /// <summary>
        /// 将参数值序列化为键值对
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> Serialize(ApiParameterDescriptor parameter)
        {
            if (parameter.IsSimpleType == true)
            {
                var kv = this.FormatAsSimple(parameter.Name, parameter.Value);
                return new[] { kv };
            }

            if (parameter.IsDictionaryOfString == true)
            {
                return this.FormatAsDictionary<string>(parameter);
            }

            if (parameter.IsDictionaryOfObject == true)
            {
                return this.FormatAsDictionary<object>(parameter);
            }

            if (parameter.IsEnumerable == true)
            {
                return this.ForamtAsEnumerable(parameter);
            }

            return this.FormatAsComplex(parameter);
        }

        /// <summary>
        /// 数组为键值对
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> ForamtAsEnumerable(ApiParameterDescriptor parameter)
        {
            var array = parameter.Value as IEnumerable;
            if (array == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return from item in array.Cast<object>()
                   select this.FormatAsSimple(parameter.Name, item);
        }

        /// <summary>
        /// 复杂类型为键值对
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> FormatAsComplex(ApiParameterDescriptor parameter)
        {
            var instance = parameter.Value;
            if (instance == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return
                from p in Property.GetProperties(parameter.ParameterType)
                let value = p.GetValue(instance)
                select this.FormatAsSimple(p.Name, value);
        }

        /// <summary>
        /// 字典转换为键值对
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> FormatAsDictionary<TValue>(ApiParameterDescriptor parameter)
        {
            var dic = parameter.Value as IDictionary<string, TValue>;
            if (dic == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return from kv in dic select this.FormatAsSimple(kv.Key, kv.Value);
        }

        /// <summary>
        /// 简单类型为键值对
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private KeyValuePair<string, string> FormatAsSimple(string name, object value)
        {
            var valueString = value == null ? null : value.ToString();
            return new KeyValuePair<string, string>(name, valueString);
        }
    }
}
