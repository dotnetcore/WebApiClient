using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 请求委托
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public delegate Task<ApiResponseContext> RequestDelegate(ApiRequestContext request);
}
