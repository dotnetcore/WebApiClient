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
    /// 此特性不需要显示声明
    /// 不可继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class PathQueryAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        private string name;

        /// <summary>
        /// 获取或设置当值为null时忽略此参数
        /// </summary>
        public bool IgnoreWhenNull { get; set; }

        /// <summary>
        /// 表示Url路径参数或query参数的特性
        /// </summary>
        public PathQueryAttribute()
        {
        }

        /// <summary>
        /// 表示Url路径参数或query参数的特性
        /// </summary>
        /// <param name="name">名称</param>
        public PathQueryAttribute(string name)
            : this(name, false)
        {
        }

        /// <summary>
        /// 表示Url路径参数或query参数的特性
        /// <param name="ignoreWhenNull">当值为null时忽略此参数</param>
        /// </summary>
        public PathQueryAttribute(bool ignoreWhenNull)
            : this(null, ignoreWhenNull)
        {
        }

        /// <summary>
        /// 表示Url路径参数或query参数的特性
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="ignoreWhenNull">当值为null时忽略此参数</param>
        public PathQueryAttribute(string name, bool ignoreWhenNull)
        {
            this.name = name;
            this.IgnoreWhenNull = ignoreWhenNull;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        public async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (this.IgnoreWhenNull && parameter.Value == null)
            {
                return;
            }

            var uri = context.RequestMessage.RequestUri;
            var url = uri.ToString();
            var relativeUrl = url.Substring(url.IndexOf(uri.AbsolutePath));

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
                var pName = string.IsNullOrEmpty(this.name) ? parameter.Name : this.name;
                return this.GetPathQuerySimple(pathQuery, pName, parameter.Value);
            }

            if (parameter.IsEnumerable == true)
            {
                return this.GetPathQueryEnumerable(pathQuery, parameter);
            }

            return this.GetPathQueryComplex(pathQuery, parameter);
        }

        /// <summary>
        /// 获取新的Path与Query
        /// </summary>
        /// <param name="pathQuery">原始path与query</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        private string GetPathQueryEnumerable(string pathQuery, ApiParameterDescriptor parameter)
        {
            var array = parameter.Value as IEnumerable;
            if (array == null)
            {
                return pathQuery;
            }

            foreach (var item in array)
            {
                var pName = string.IsNullOrEmpty(this.name) ? parameter.Name : this.name;
                pathQuery = this.GetPathQuerySimple(pathQuery, pName, item);
            }
            return pathQuery;
        }

        /// <summary>
        /// 获取新的Path与Query
        /// </summary>
        /// <param name="pathQuery">原始path与query</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        private string GetPathQueryComplex(string pathQuery, ApiParameterDescriptor parameter)
        {
            var instance = parameter.Value;
            var instanceType = parameter.ParameterType;

            var properties = Property.GetProperties(instanceType);
            foreach (var p in properties)
            {
                var value = instance == null ? null : p.GetValue(instance);
                pathQuery = this.GetPathQuerySimple(pathQuery, p.Name , value);
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
        private string GetPathQuerySimple(string pathQuery, string name, object value)
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
