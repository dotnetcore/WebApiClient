
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
    /// 表示x-www-form-urlencoded的http内容
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
            var form = Property
                   .GetProperties(parameter.ParameterType)
                   .Select(p =>
                   {
                       var value = parameter.Value == null ? null : p.GetValue(parameter.Value);
                       var valueString = value == null ? null : value.ToString();
                       var valueEncoded = HttpUtility.UrlEncode(valueString, encoding);
                       return string.Format("{0}={1}", p.Name, valueEncoded);
                   });

            return string.Join("&", form);
        }
    }
}
