using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示参数值序列化为Json并作为multipart/form-data表单的一个文本项
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class JsonMulitpartTextAttribute : Attribute, IApiParameterAttribute, IIgnoreWhenNullable
    {
        /// <summary>
        /// 获取或设置当值为null是否忽略提交
        /// 默认为false
        /// </summary>
        public bool IgnoreWhenNull { get; set; }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (this.IsIgnoreWith(parameter) == false)
            {
                var options = context.HttpApiConfig.FormatOptions;
                var json = context.HttpApiConfig.JsonFormatter.Serialize(parameter.Value, options);
                var fieldName = parameter.Name;
                context.RequestMessage.AddMulitpartText(fieldName, json);
            }
            return ApiTask.CompletedTask;
        }
    }
}
