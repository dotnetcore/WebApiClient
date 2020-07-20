using System.Threading.Tasks;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Extensions.JsonRpc;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示Rpc请求的一个参数
    /// </summary> 
    public class JsonRpcParamAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiParameterContext context)
        {
            var parameters = context.Properties.Get<JsonRpcParameters>(typeof(JsonRpcParameters));
            if (parameters == null)
            {
                throw new ApiInvalidConfigException($"请为接口方法{context.ApiAction.Name}修饰{nameof(JsonRpcMethodAttribute)}");
            }

            parameters.Add(context);
            return Task.CompletedTask;
        }
    }
}
