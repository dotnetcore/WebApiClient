
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
            var body = this.GetFormContent(parameter, encoding);
            return new StringContent(body, encoding, "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 生成表单内容
        /// key1=value1&key2=value2
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        protected virtual string GetFormContent(ApiParameterDescriptor parameter, Encoding encoding)
        {
            if (parameter.IsSimpleType == true)
            {
                return this.SimpleToForm(parameter.Name, parameter.Value, encoding);
            }

            if (parameter.IsDictionaryOfString == true)
            {
                return this.DictionaryToForm<string>(parameter, encoding);
            }

            if (parameter.IsDictionaryOfObject == true)
            {
                return this.DictionaryToForm<object>(parameter, encoding);
            }

            if (parameter.IsEnumerable == true)
            {
                return this.EnumerableToForm(parameter, encoding);
            }

            return this.ComplexToForm(parameter, encoding);
        }

        /// <summary>
        /// 生成数组的表单内容
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private string EnumerableToForm(ApiParameterDescriptor parameter, Encoding encoding)
        {
            var array = parameter.Value as IEnumerable;
            if (array == null)
            {
                return null;
            }

            var q = from item in array.Cast<object>()
                    select this.SimpleToForm(parameter.Name, item, encoding);

            return string.Join("&", q);
        }

        /// <summary>
        /// 生成复杂类型的表单内容
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private string ComplexToForm(ApiParameterDescriptor parameter, Encoding encoding)
        {
            var instance = parameter.Value;
            if (parameter == null)
            {
                return null;
            }

            var q = from p in Property.GetProperties(parameter.ParameterType)
                    let value = p.GetValue(instance)
                    select this.SimpleToForm(p.Name, value, encoding);

            return string.Join("&", q);
        }

        /// <summary>
        /// 字典转换为表单
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private string DictionaryToForm<TValue>(ApiParameterDescriptor parameter, Encoding encoding)
        {
            var dic = parameter.Value as IDictionary<string, TValue>;
            if (dic == null)
            {
                return null;
            }

            var q = from kv in dic
                    select this.SimpleToForm(kv.Key, kv.Value, encoding);

            return string.Join("&", q);
        }

        /// <summary>
        /// 生成简单类型的表单内容
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private string SimpleToForm(string name, object value, Encoding encoding)
        {
            var valueString = value == null ? null : value.ToString();
            var valueEncoded = HttpUtility.UrlEncode(valueString, encoding);
            return string.Format("{0}={1}", name, valueEncoded);
        }
    }
}
