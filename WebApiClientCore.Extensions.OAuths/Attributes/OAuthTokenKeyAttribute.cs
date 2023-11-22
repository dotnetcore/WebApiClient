using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 动态应用标识
    /// </summary>
    public class OAuthTokenKeyAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        public override Task OnRequestAsync(ApiParameterContext context)
        {
            return Task.CompletedTask;
        }
    }
}
