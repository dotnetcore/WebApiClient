using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
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
        /// <param name="parameter">特性关联的参数</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <returns></returns>
        protected override async Task SetHttpContentAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var httpContent = context.RequestMessage.Content as UrlEncodedContent;
            if (httpContent == null)
            {
                httpContent = new UrlEncodedContent();
                await httpContent.AddHttpContentAsync(context.RequestMessage.Content).ConfigureAwait(false);
            }

            var form = parameter.ToString();
            await httpContent.AddRawFormAsync(form).ConfigureAwait(false);
            context.RequestMessage.Content = httpContent;
        }
    }
}
