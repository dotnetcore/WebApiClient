using System;
using System.Collections;
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
    /// 表示参数值为multipart/form-data表单或表单的一个项
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
            var httpContent = this.GetHttpContentFromContext(context);
            var mulitItem = this.GetMulitpartItems(parameter, Encoding.UTF8);
            foreach (var item in mulitItem)
            {
                httpContent.Add(item.Content, item.Name);
            }
            return httpContent;
        }

        /// <summary>
        /// 从上下文获取已有MultipartFormDataContent
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        private MultipartFormDataContent GetHttpContentFromContext(ApiActionContext context)
        {
            var httpContent = context.RequestMessage.Content as MultipartFormDataContent;
            if (httpContent == null)
            {
                httpContent = new MultipartFormDataContent();
            }
            return httpContent;
        }

        /// <summary>
        /// 获取所有HttpContent项
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private IEnumerable<MulitpartItem> GetMulitpartItems(ApiParameterDescriptor parameter, Encoding encoding)
        {
            if (parameter.IsSimpleType == true)
            {
                var content = this.SimpleToMulitpartItem(parameter.Name, parameter.Value, encoding);
                return new[] { content };
            }
            else if (parameter.IsEnumerable == true)
            {
                return this.EnumerableToMulitpartItems(parameter, encoding);
            }

            return this.ComplexToMulitpartItems(parameter, encoding);
        }

        /// <summary>
        /// 数组转换为MulitpartItem项
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private IEnumerable<MulitpartItem> EnumerableToMulitpartItems(ApiParameterDescriptor parameter, Encoding encoding)
        {
            var array = parameter.Value as IEnumerable;
            if (array == null)
            {
                return Enumerable.Empty<MulitpartItem>();
            }

            return
                from item in array.Cast<object>()
                select this.SimpleToMulitpartItem(parameter.Name, item, encoding);
        }

        /// <summary>
        /// 复杂类型转换为MulitpartItem项
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private IEnumerable<MulitpartItem> ComplexToMulitpartItems(ApiParameterDescriptor parameter, Encoding encoding)
        {
            var instance = parameter.Value;

            return
                from p in Property.GetProperties(parameter.ParameterType)
                let value = instance == null ? null : p.GetValue(instance)
                select this.SimpleToMulitpartItem(p.Name, value, encoding);
        }

        /// <summary>
        /// 简单类型转换为MulitpartItem项
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private MulitpartItem SimpleToMulitpartItem(string name, object value, Encoding encoding)
        {
            var valueString = value == null ? null : value.ToString();
            var valueEncoded = HttpUtility.UrlEncode(valueString, encoding);
            var httpContent = new StringContent(valueEncoded, encoding);
            return new MulitpartItem(name, httpContent);
        }

        /// <summary>
        /// 表示Mulitpart项
        /// </summary>
        class MulitpartItem
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// 内容
            /// </summary>
            public HttpContent Content { get; private set; }

            /// <summary>
            /// Mulitpart项
            /// </summary>
            /// <param name="name">名称</param>
            /// <param name="content">内容</param>
            public MulitpartItem(string name, HttpContent content)
            {
                this.Name = name;
                this.Content = content;
            }
        }
    }
}
