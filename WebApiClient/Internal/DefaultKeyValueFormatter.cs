using System;
using System.Collections;
using System.Collections.Generic;
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
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> Serialize(object model)
        {
            if (model == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            var dicObj = model as IDictionary<string, object>;
            if (dicObj != null)
            {
                return this.FormatAsDictionary<object>(dicObj);
            }

            var dicString = model as IDictionary<string, string>;
            if (dicString != null)
            {
                return this.FormatAsDictionary<string>(dicString);
            }

            return this.FormatAsComplex(model);
        }

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
                var dic = parameter.Value as IDictionary<string, string>;
                return this.FormatAsDictionary<string>(dic);
            }

            if (parameter.IsDictionaryOfObject == true)
            {
                var dic = parameter.Value as IDictionary<string, object>;
                return this.FormatAsDictionary<object>(dic);
            }

            if (parameter.IsEnumerable == true)
            {
                var enumerable = parameter.Value as IEnumerable;
                return this.ForamtAsEnumerable(parameter.Name, enumerable);
            }

            return this.FormatAsComplex(parameter.Value);
        }

        /// <summary>
        /// 数组为键值对
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="enumerable">值</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> ForamtAsEnumerable(string name, IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return from item in enumerable.Cast<object>()
                   select this.FormatAsSimple(name, item);
        }

        /// <summary>
        /// 复杂类型为键值对
        /// </summary>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> FormatAsComplex(object instance)
        {
            if (instance == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return
                from p in KeyValueProperty.GetProperties(instance.GetType())
                where p.IsSupportGet && p.IsKeyValueIgnore == false
                let value = p.GetValue(instance)
                select this.FormatAsSimple(p.Name, value);
        }

        /// <summary>
        /// 字典转换为键值对
        /// </summary>
        /// <param name="dic">字典</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> FormatAsDictionary<TValue>(IDictionary<string, TValue> dic)
        {
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
