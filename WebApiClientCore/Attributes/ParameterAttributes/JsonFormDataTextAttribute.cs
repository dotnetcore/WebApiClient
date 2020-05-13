using System;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数值序列化为Json并作为multipart/form-data表单的一个文本项
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class JsonFormDataTextAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public Task BeforeRequestAsync(ApiParameterContext context)
        {
            var json = context.SerializeToJson();
            var fieldName = context.Parameter.Name;
            var fildValue = Encoding.UTF8.GetString(json);
            context.RequestMessage.AddFormDataText(fieldName, fildValue);

            return Task.CompletedTask;
        }
    }
}
