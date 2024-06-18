using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数值序列化为 json 并作为 multipart/form-data 表单的一个文本项
    /// </summary> 
    public class JsonFormDataTextAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
        public override Task OnRequestAsync(ApiParameterContext context)
        {
            var fieldName = context.ParameterName;
            var fieldValue = context.SerializeToJsonString();
            context.HttpContext.RequestMessage.AddFormDataText(fieldName, fieldValue);
            return Task.CompletedTask;
        }
    }
}
