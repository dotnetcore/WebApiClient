using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数值作为请求头   
    /// 对于复杂类型的参数值，将拆解作为多个Header
    /// </summary>
    public class HeadersAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// 获取或设置是否将请求名的_转换为-
        /// 默认为true
        /// </summary>
        public bool UnderlineToMinus { get; set; } = true;

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiParameterContext context)
        {
            foreach (var item in context.SerializeToKeyValues())
            {
                if (string.IsNullOrEmpty(item.Value) == false)
                {
                    var name = this.UnderlineToMinus ? item.Key.Replace("_", "-") : item.Key;
                    context.HttpContext.RequestMessage.Headers.TryAddWithoutValidation(name, item.Value);
                }
            }
            return Task.CompletedTask;
        }
    }
}
