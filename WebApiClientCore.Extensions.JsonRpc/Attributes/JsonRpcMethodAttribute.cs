using System.Threading.Tasks;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Extensions.JsonRpc;
using WebApiClientCore.Serialization;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示Json-Rpc请求方法
    /// </summary>
    public class JsonRpcMethodAttribute : HttpPostAttribute, IApiFilterAttribute
    {
        /// <summary>
        /// JsonRpc的方法名称
        /// </summary>
        private readonly string? method;

        /// <summary>
        /// 获取或设置提交的Content-Type
        /// 默认为application/json-rpc 
        /// </summary>
        public string ContentType { get; set; } = JsonRpcContent.MediaType;

        /// <summary>
        /// 获取或设置JsonRpc的参数风格
        /// 默认为JsonRpcParamsStyle.Array
        /// </summary>
        public JsonRpcParamsStyle ParamsStyle { get; set; } = JsonRpcParamsStyle.Array;

        /// <summary>
        /// 获取过滤器是否启用
        /// </summary>
        bool IAttributeEnable.Enable => true;

        /// <summary>
        /// Json-Rpc请求方法
        /// </summary>
        public JsonRpcMethodAttribute()
            : this(method: null, path: null)
        {
        }

        /// <summary>
        /// Json-Rpc请求方法
        /// </summary>
        /// <param name="method">JsonRpc的方法名称</param>
        public JsonRpcMethodAttribute(string? method)
            : this(method, path: null)
        {
        }

        /// <summary>
        /// Json-Rpc请求方法
        /// </summary>
        /// <param name="method">JsonRpc的方法名称</param>
        /// <param name="path">JsonRpc路径</param>
        public JsonRpcMethodAttribute(string? method, string? path)
            : base(path)
        {
            this.method = method;
        }

        /// <summary>
        /// Action请求前
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            context.Properties.Set(typeof(JsonRpcParameters), new JsonRpcParameters());
            return base.OnRequestAsync(context);
        }

        /// <summary>
        /// Filter请求前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task IApiFilter.OnRequestAsync(ApiRequestContext context)
        {
            var parameters = context.Properties.Get<JsonRpcParameters>(typeof(JsonRpcParameters));
            if (parameters == null)
            {
                throw new ApiInvalidConfigException($"无法获取{nameof(JsonRpcParameters)}属性");
            }

            var jsonRpcRequest = new JsonRpcRequest
            {
                Method = this.method ?? context.ApiAction.Name,
                Params = parameters.ToJsonRpcParams(this.ParamsStyle),
            };

            var jsonRpcContent = new JsonRpcContent(this.ContentType);
            context.HttpContext.RequestMessage.Content = jsonRpcContent;
            var options = context.HttpContext.HttpApiOptions.JsonSerializeOptions;          
            JsonBufferSerializer.Serialize(jsonRpcContent, jsonRpcRequest, options);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Filter响应后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task IApiFilter.OnResponseAsync(ApiResponseContext context)
        {
            return Task.CompletedTask;
        }
    }
}
