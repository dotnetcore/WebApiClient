using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示Http请求Header的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class HeaderAttribute : ApiActionAttribute, IApiParameterAttribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        private readonly string name;

        /// <summary>
        /// 值 
        /// </summary>
        private readonly string value;

        /// <summary>
        /// 获取本类型是否允许在接口与方法上重复
        /// </summary>
        public override bool AllowMultiple
        {
            get
            {
                return true;
            }
        }


        /// <summary>
        /// 将参数值设置到Header
        /// 参数值为null则删除此Header项
        /// </summary>
        /// <param name="name">header名称</param>
        public HeaderAttribute(HttpRequestHeader name)
            : this(name.ToString(), null)
        {
        }


        /// <summary>
        /// 将参数值设置到Header
        /// 参数值为null则删除此Header项
        /// </summary>
        /// <param name="name">header名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HeaderAttribute(string name)
            : this(name, null)
        {
        }


        /// <summary>
        /// 将指定值设置到Header
        /// value为null则删除此Header项
        /// </summary>
        /// <param name="name">header名称</param>
        /// <param name="value">header值</param>
        public HeaderAttribute(HttpRequestHeader name, string value)
            : this(name.ToString(), value)
        {
        }

        /// <summary>
        /// 将指定值设置到Header
        /// value为null则删除此Header项
        /// </summary>
        /// <param name="name">header名称</param>
        /// <param name="value">header值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HeaderAttribute(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            var header = context.RequestMessage.Headers;
            header.Remove(this.name);
            if (this.value != null)
            {
                header.TryAddWithoutValidation(this.name, this.value);
            }
            return TaskExtend.CompletedTask;
        }

        /// <summary>
        /// http请求之前
        /// 值从参数过来
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        Task IApiParameterAttribute.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var header = context.RequestMessage.Headers;
            header.Remove(this.name);
            if (parameter.Value != null)
            {
                header.TryAddWithoutValidation(this.name, parameter.Value.ToString());
            }
            return TaskExtend.CompletedTask;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} = {1}", this.name, this.value);
        }
    }
}
