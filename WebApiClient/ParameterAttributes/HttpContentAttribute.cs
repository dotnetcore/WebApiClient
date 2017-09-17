
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示参数为HttpContent或派生类型的特性
    /// 此类型的参数可以不用注明HttpContentAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class HttpContentAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        async Task IApiParameterAttribute.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var method = context.RequestMessage.Method;
            if (method == HttpMethod.Get || method == HttpMethod.Head)
            {
                return;
            }

            if (parameter.IsHttpContent == true)
            {
                context.RequestMessage.Content = parameter.Value as HttpContent;
            }
            else
            {
                context.RequestMessage.Content = this.GetHttpContent(context, parameter);
            }
            await TaskExtend.CompletedTask;
        }

        /// <summary>
        /// 获取http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的属性</param>
        /// <returns></returns>
        protected virtual HttpContent GetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            return parameter.Value as HttpContent;
        }

        /// <summary>
        /// 格式化参数
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="formater">格式化工具</param>
        /// <param name="encoding">编码</param>        
        /// <returns></returns>
        protected string FormatParameter(ApiParameterDescriptor parameter, IStringFormatter formater, Encoding encoding)
        {
            if (parameter.Value == null)
            {
                return null;
            }

            if (parameter.ParameterType == typeof(string))
            {
                return parameter.Value.ToString();
            }
            return formater.Serialize(parameter.Value, encoding);
        }

        /// <summary>
        /// 格式化参数为键值对
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        protected IEnumerable<KeyValuePair<string, string>> FormatParameter(ApiParameterDescriptor parameter)
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
            if (parameter == null)
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
        /// <param name="parameter"></param>
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
