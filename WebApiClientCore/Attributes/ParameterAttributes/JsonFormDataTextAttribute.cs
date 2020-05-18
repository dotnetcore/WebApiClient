using System.Text;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数值序列化为Json并作为multipart/form-data表单的一个文本项
    /// </summary> 
    public class JsonFormDataTextAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public sealed override Task OnRequestAsync(ApiParameterContext context)
        {
            var json = context.SerializeToJson();
            var fieldName = context.Parameter.Name;
            var fildValue = Encoding.UTF8.GetString(json);
            context.HttpContext.RequestMessage.AddFormDataText(fieldName, fildValue);

            return Task.CompletedTask;
        }
    }
}
