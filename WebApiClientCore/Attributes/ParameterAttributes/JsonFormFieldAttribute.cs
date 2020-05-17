using System.Text;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数值序列化为Json并作为x-www-form-urlencoded的字段
    /// </summary>
    public class JsonFormFieldAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public sealed override async Task OnRequestAsync(ApiParameterContext context)
        {
            var json = context.SerializeToJson();
            var fieldName = context.Parameter.Name;
            var fildValue = Encoding.UTF8.GetString(json);
            await context.HttpContext.RequestMessage.AddFormFieldAsync(fieldName, fildValue).ConfigureAwait(false);
        }
    }
}
