using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将参数值作为url路径参数或query参数的特性
    /// 支持单一值类型如string、int、guid、枚举等，以及他们的可空类型或集合
    /// 支持POCO类型、IDictionaryOf(string,string)类型、IDictionaryOf(string,object)类型
    /// 此特性不需要在参数上显式注明
    /// 不可继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class PathQueryAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// 表示Url路径参数或query参数的特性
        /// </summary>
        public PathQueryAttribute()
        {
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        public async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var uri = context.RequestMessage.RequestUri;
            var url = uri.ToString();
            var relativeUrl = url.Substring(url.IndexOf(uri.AbsolutePath)).TrimEnd('&', '?');

            var pathQuery = this.GetPathQuery(relativeUrl, parameter);
            context.RequestMessage.RequestUri = new Uri(uri, pathQuery);
            await TaskExtend.CompletedTask;
        }

        /// <summary>
        /// 获取新的Path与Query
        /// </summary>
        /// <param name="pathQuery">原始path与query</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        private string GetPathQuery(string pathQuery, ApiParameterDescriptor parameter)
        {
            if (parameter.IsSimpleType == true)
            {
                return this.GetSimplePathQuery(pathQuery, parameter.Name, parameter.Value);
            }

            if (parameter.IsDictionaryOfObject == true)
            {
                return this.GetDictionaryPathQuery<object>(pathQuery, parameter);
            }

            if (parameter.IsDictionaryOfString == true)
            {
                return this.GetDictionaryPathQuery<string>(pathQuery, parameter);
            }

            if (parameter.IsEnumerable == true)
            {
                return this.GetEnumerablePathQuery(pathQuery, parameter);
            }

            return this.GetComplexPathQuery(pathQuery, parameter);
        }

        /// <summary>
        /// 获取新的Path与Query
        /// </summary>
        /// <param name="pathQuery">原始path与query</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        private string GetDictionaryPathQuery<TValue>(string pathQuery, ApiParameterDescriptor parameter)
        {
            var dic = parameter.Value as IDictionary<string, TValue>;
            if (dic == null)
            {
                return pathQuery;
            }

            foreach (var kv in dic)
            {
                pathQuery = this.GetSimplePathQuery(pathQuery, kv.Key, kv.Value);
            }
            return pathQuery;
        }

        /// <summary>
        /// 获取新的Path与Query
        /// </summary>
        /// <param name="pathQuery">原始path与query</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        private string GetEnumerablePathQuery(string pathQuery, ApiParameterDescriptor parameter)
        {
            var array = parameter.Value as IEnumerable;
            if (array == null)
            {
                return pathQuery;
            }

            foreach (var item in array)
            {
                pathQuery = this.GetSimplePathQuery(pathQuery, parameter.Name, item);
            }
            return pathQuery;
        }

        /// <summary>
        /// 获取新的Path与Query
        /// </summary>
        /// <param name="pathQuery">原始path与query</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        private string GetComplexPathQuery(string pathQuery, ApiParameterDescriptor parameter)
        {
            var instance = parameter.Value;
            var instanceType = parameter.ParameterType;

            var properties = Property.GetProperties(instanceType);
            foreach (var p in properties)
            {
                var value = instance == null ? null : p.GetValue(instance);
                pathQuery = this.GetSimplePathQuery(pathQuery, p.Name, value);
            }
            return pathQuery;
        }

        /// <summary>
        /// 获取新的Path与Query
        /// </summary>
        /// <param name="pathQuery">原始path与query</param>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private string GetSimplePathQuery(string pathQuery, string name, object value)
        {
            var valueString = value == null ? string.Empty : value.ToString();
            var regex = new Regex("{" + name + "}", RegexOptions.IgnoreCase);

            if (regex.IsMatch(pathQuery) == true)
            {
                return regex.Replace(pathQuery, valueString);
            }

            var keyValue = string.Format("{0}={1}", name, valueString);
            var concat = pathQuery.Contains('?') ? "&" : "?";
            return pathQuery + concat + keyValue;
        }
    }
}
