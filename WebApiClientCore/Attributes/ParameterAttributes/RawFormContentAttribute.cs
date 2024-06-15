using System.Threading.Tasks;
using WebApiClientCore.HttpContents;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示将参数值的已编码原始表单内容作为请求内容
    /// </summary>
    public class RawFormContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 设置参数到 http 请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override async Task SetHttpContentAsync(ApiParameterContext context)
        {
            var content = context.HttpContext.RequestMessage.Content;
            var formContent = await FormContent.ParseAsync(content).ConfigureAwait(false);
            formContent.AddForm(context.ParameterValue?.ToString());
            context.HttpContext.RequestMessage.Content = formContent;
        }
    }
}
