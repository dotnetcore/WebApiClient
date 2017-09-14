using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示multipart/form-data表单或表单的一个项
    /// </summary>
    public class MulitpartContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 获取http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的属性</param>
        /// <returns></returns>
        protected override HttpContent GetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var multiContent = this.GetMultipartFormContent(context);
            var httpContents = this.GetContentItems(parameter, Encoding.UTF8);
            foreach (var item in httpContents)
            {
                multiContent.Add(item.Content, item.Name);
            }
            return multiContent;
        }

        /// <summary>
        /// 从上下文获取已有MultipartFormDataContent
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        private MultipartFormDataContent GetMultipartFormContent(ApiActionContext context)
        {
            var mulitpartContent = context.RequestMessage.Content as MultipartFormDataContent;
            if (mulitpartContent == null)
            {
                mulitpartContent = new MultipartFormDataContent();
            }
            return mulitpartContent;
        }

        /// <summary>
        /// 获取所有HttpContent项
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private IEnumerable<MulitpartItem> GetContentItems(ApiParameterDescriptor parameter, Encoding encoding)
        {
            if (parameter.IsSimpleType == true)
            {
                var content = this.GetContentSimple(parameter.Name, parameter.Value, encoding);
                return new[] { content };
            }
            else if (parameter.ParameterType.IsArray == true)
            {
                return this.GetContentArray(parameter, encoding);
            }

            return this.GetContentComplex(parameter, encoding);
        }

        /// <summary>
        /// 生成数组的HttpContent项
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private IEnumerable<MulitpartItem> GetContentArray(ApiParameterDescriptor parameter, Encoding encoding)
        {
            var array = parameter.Value as Array;
            if (array == null)
            {
                return Enumerable.Empty<MulitpartItem>();
            }

            return array.Cast<object>().Select(item => this.GetContentSimple(parameter.Name, item, encoding));
        }

        /// <summary>
        /// 生成复杂类型的HttpContent项
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private IEnumerable<MulitpartItem> GetContentComplex(ApiParameterDescriptor parameter, Encoding encoding)
        {
            var instance = parameter.Value;
            return Property
                .GetProperties(parameter.ParameterType)
                .Select(p =>
                {
                    var value = instance == null ? null : p.GetValue(instance);
                    return this.GetContentSimple(p.Name, value, encoding);
                });
        }

        /// <summary>
        /// 生成简单类型的HttpContent项
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private MulitpartItem GetContentSimple(string name, object value, Encoding encoding)
        {
            var valueString = value == null ? null : value.ToString();
            var valueEncoded = HttpUtility.UrlEncode(valueString, encoding);

            return new MulitpartItem
            {
                Name = name,
                Content = new StringContent(valueEncoded, encoding)
            };
        }

        /// <summary>
        /// Mulitpart项
        /// </summary>
        class MulitpartItem
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 内容
            /// </summary>
            public HttpContent Content { get; set; }
        }
    }
}
