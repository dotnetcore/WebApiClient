
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将参数值作为x-www-form-urlencoded请求
    /// </summary>
    public class FormContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 获取http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        protected sealed override HttpContent GetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var encoding = Encoding.UTF8;
            var q = from kv in this.GetFormNameValues(parameter)
                    select string.Format("{0}={1}", kv.Key, HttpUtility.UrlEncode(kv.Value, encoding));

            var content = string.Join("&", q);
            return new StringContent(content, encoding, "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 生成表单内容
        /// key1=value1&key2=value2
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        protected virtual IEnumerable<KeyValuePair<string, string>> GetFormNameValues(ApiParameterDescriptor parameter)
        {
            if (parameter.IsSimpleType == true)
            {
                var kv = this.SimpleToNameValue(parameter.Name, parameter.Value);
                return new[] { kv };
            }

            if (parameter.IsDictionaryOfString == true)
            {
                return this.DictionaryToNameValues<string>(parameter);
            }

            if (parameter.IsDictionaryOfObject == true)
            {
                return this.DictionaryToNameValues<object>(parameter);
            }

            if (parameter.IsEnumerable == true)
            {
                return this.EnumerableToNameValues(parameter);
            }

            return this.ComplexToNameValues(parameter);
        }

        /// <summary>
        /// 生成数组的表单内容
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> EnumerableToNameValues(ApiParameterDescriptor parameter)
        {
            var array = parameter.Value as IEnumerable;
            if (array == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return from item in array.Cast<object>()
                   select this.SimpleToNameValue(parameter.Name, item);
        }

        /// <summary>
        /// 生成复杂类型的表单内容
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> ComplexToNameValues(ApiParameterDescriptor parameter)
        {
            var instance = parameter.Value;
            if (parameter == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return
                from p in Property.GetProperties(parameter.ParameterType)
                let value = p.GetValue(instance)
                select this.SimpleToNameValue(p.Name, value);
        }

        /// <summary>
        /// 字典转换为表单
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> DictionaryToNameValues<TValue>(ApiParameterDescriptor parameter)
        {
            var dic = parameter.Value as IDictionary<string, TValue>;
            if (dic == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return from kv in dic select this.SimpleToNameValue(kv.Key, kv.Value);
        }

        /// <summary>
        /// 生成简单类型的表单内容
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private KeyValuePair<string, string> SimpleToNameValue(string name, object value)
        {
            var valueString = value == null ? null : value.ToString();
            return new KeyValuePair<string, string>(name, valueString);
        }
    }
}
