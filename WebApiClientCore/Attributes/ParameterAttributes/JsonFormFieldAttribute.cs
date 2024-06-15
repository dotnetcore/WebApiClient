using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数值序列化为 json 并作为 x-www-form-urlencoded 的字段
    /// </summary>
    public class JsonFormFieldAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
        public override async Task OnRequestAsync(ApiParameterContext context)
        {
            var json = context.SerializeToJson();
            var fieldName = context.ParameterName;
            var fieldValue = Encoding.UTF8.GetString(json);
            await context.HttpContext.RequestMessage.AddFormFieldAsync(fieldName, fieldValue).ConfigureAwait(false);
        }
    }
}
