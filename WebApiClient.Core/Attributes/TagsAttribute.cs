using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示上下文Tags数据写入的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class TagsAttribute : ApiActionAttribute, IApiParameterAttribute
    {
        /// <summary>
        /// key
        /// </summary>
        private readonly string key;

        /// <summary>
        /// 值
        /// </summary>
        private readonly object value;

        /// <summary>
        /// 请求前将参数值写入到上下文的Tags
        /// </summary>
        /// <param name="key">Tags的key</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TagsAttribute(string key)
        : this(key, null)
        {
        }

        /// <summary>
        /// 请求前将指定的值写入到上下文的Tags
        /// </summary>
        /// <param name="key">Tags的key</param>
        /// <param name="value">固定的值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TagsAttribute(string key, object value)
        {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
            this.value = value;
        }

        /// <summary>
        /// 参数请求前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        Task IApiParameterAttribute.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            context.Tags.Set(this.key, parameter.Value);
            return ApiTask.CompletedTask;
        }

        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            context.Tags.Set(this.key, this.value);
            return ApiTask.CompletedTask;
        }
    }
}
