using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        private StMultipartFormDataContent GetHttpContentFromContext(ApiActionContext context)
        {
            var httpContent = context.RequestMessage.Content as StMultipartFormDataContent;
            if (httpContent == null)
            {
                httpContent = new StMultipartFormDataContent();
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

            if (parameter.IsDictionaryOfObject == true)
            {
                return this.DictionaryToMulitpartItems<object>(parameter, encoding);
            }

            if (parameter.IsDictionaryOfString == true)
            {
                return this.DictionaryToMulitpartItems<string>(parameter, encoding);
            }

            if (parameter.IsEnumerable == true)
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
        /// 字典转换为MulitpartItem项
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private IEnumerable<MulitpartItem> DictionaryToMulitpartItems(IDictionary<string, object> dic, Encoding encoding)
        {
            if (dic == null)
            {
                return Enumerable.Empty<MulitpartItem>();
            }

            return from kv in dic
                   select this.SimpleToMulitpartItem(kv.Key, kv.Value, encoding);
        }

        /// <summary>
        /// 字典转换为MulitpartItem项
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private IEnumerable<MulitpartItem> DictionaryToMulitpartItems<TValue>(ApiParameterDescriptor parameter, Encoding encoding)
        {
            var dic = parameter.Value as IDictionary<string, TValue>;
            if (dic == null)
            {
                return Enumerable.Empty<MulitpartItem>();
            }

            return from kv in dic
                   select this.SimpleToMulitpartItem(kv.Key, kv.Value, encoding);
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
            if (instance == null)
            {
                return Enumerable.Empty<MulitpartItem>();
            }

            return
                from p in Property.GetProperties(parameter.ParameterType)
                let value = p.GetValue(instance)
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
            var valueString = value == null ? string.Empty : value.ToString();
            var valueEncoded = HttpUtility.UrlEncode(valueString, encoding);
            var httpContent = new StringContent(valueEncoded);
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
            /// 文本内容
            /// </summary>
            public StringContent Content { get; private set; }

            /// <summary>
            /// Mulitpart项
            /// </summary>
            /// <param name="name">名称</param>
            /// <param name="content">内容</param>
            public MulitpartItem(string name, StringContent content)
            {
                this.Name = name;
                this.Content = content;
            }
        }
    }
}
