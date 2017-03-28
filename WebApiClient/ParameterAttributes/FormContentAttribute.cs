
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebApiClient
{
    /// <summary>
    /// 表示将参数体作为x-www-form-urlencoded请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FormContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 获取http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        protected override HttpContent GetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var encoding = Encoding.UTF8;
            var body = this.GetFormContent(parameter, encoding);
            return new StringContent(body, encoding, "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 生成表单内容
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private string GetFormContent(ApiParameterDescriptor parameter, Encoding encoding)
        {
            if (parameter.IsSimpleType == true)
            {
                return this.GetContentSimple(parameter.Name, parameter.Value, encoding);
            }
            else if (parameter.ParameterType.IsArray == true)
            {
                return this.GetContentArray(parameter, encoding);
            }

            return this.GetContentComplex(parameter, encoding);
        }

        /// <summary>
        /// 生成数组的表单内容
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private string GetContentArray(ApiParameterDescriptor parameter, Encoding encoding)
        {
            var array = parameter.Value as Array;
            if (array == null)
            {
                return null;
            }

            var keyValues = array.Cast<object>().Select(item => this.GetContentSimple(parameter.Name, item, encoding));
            return string.Join("&", keyValues);
        }

        /// <summary>
        /// 生成复杂类型的表单内容
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private string GetContentComplex(ApiParameterDescriptor parameter, Encoding encoding)
        {
            var instance = parameter.Value;
            var keyValues = Property
                .GetProperties(parameter.ParameterType)
                .Select(p =>
                {
                    var value = instance == null ? null : p.GetValue(instance);
                    return this.GetContentSimple(p.Name, value, encoding);
                });

            return string.Join("&", keyValues);
        }

        /// <summary>
        /// 生成简单类型的表单内容
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private string GetContentSimple(string name, object value, Encoding encoding)
        {
            var valueString = value == null ? null : value.ToString();
            var valueEncoded = HttpUtility.UrlEncode(valueString, encoding);
            return string.Format("{0}={1}", name, valueEncoded);
        }
    }
}
