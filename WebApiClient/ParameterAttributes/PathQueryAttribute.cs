using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示Url路径参数或query参数的特性
    /// 此特性不需要显示声明
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class PathQueryAttribute : ApiParameterAttribute
    {
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
        /// <param name="ignoreWhenNull">当值为null时忽略此参数</param>
        /// </summary>
        public PathQueryAttribute(bool ignoreWhenNull)
        {
            this.IgnoreWhenNull = ignoreWhenNull;
        }


        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        public override async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (this.IgnoreWhenNull && parameter.Value == null)
            {
                return;
            }

            var uri = context.RequestMessage.RequestUri;
            var pathQuery = this.GetPathQuery(uri.LocalPath + uri.Query, parameter);
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
                return this.GetPathQuerySimple(pathQuery, parameter.Name, parameter.Value);
            }

            if (parameter.ParameterType.IsArray == true)
            {
                return this.GetPathQueryArray(pathQuery, parameter);
            }

            return this.GetPathQueryComplex(pathQuery, parameter);
        }

        /// <summary>
        /// 获取新的Path与Query
        /// </summary>
        /// <param name="pathQuery">原始path与query</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        private string GetPathQueryArray(string pathQuery, ApiParameterDescriptor parameter)
        {
            var array = parameter.Value as Array;
            if (array == null)
            {
                return pathQuery;
            }

            foreach (var item in array)
            {
                pathQuery = this.GetPathQuerySimple(pathQuery, parameter.Name, item);
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
                pathQuery = this.GetPathQuerySimple(pathQuery, p.Name, value);
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
