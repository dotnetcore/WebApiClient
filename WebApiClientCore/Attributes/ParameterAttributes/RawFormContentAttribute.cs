using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示将参数值的已编码原始表单内容作为请求内容
    /// </summary>
    public class RawFormContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="HttpApiInvalidOperationException"></exception>
        /// <returns></returns>
        protected override async Task SetHttpContentAsync(ApiParameterContext context)
        {
            var form = context.ParameterValue?.ToString();
            var fromContent = await FormContent.FromHttpContentAsync(context.RequestMessage.Content).ConfigureAwait(false);
            await fromContent.AddRawFormAsync(form).ConfigureAwait(false);
            context.RequestMessage.Content = fromContent;
        }
    }
}
