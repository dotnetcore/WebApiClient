using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.JsonRpc;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示Json-Rpc请求方法
    /// </summary>
    public class JsonRpcMethodAttribute : JsonReturnAttribute, IApiActionAttribute, IApiFilterAttribute
    {
        /// <summary>
        /// Post请求特性
        /// </summary>
        private HttpPostAttribute postAttribute = new HttpPostAttribute();

        /// <summary>
        /// 获取JsonRpc的方法名称
        /// 为null则使用声明的方法名
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// 获取或设置JsonRpc的参数风格
        /// 默认为JsonRpcParamsStyle.Array
        /// </summary>
        public JsonRpcParamsStyle ParamsStyle { get; set; } = JsonRpcParamsStyle.Array;

        /// <summary>
        /// 获取或设置提交的Content-Type
        /// 默认为application/json-rpc 
        /// </summary>
        public string ContentType { get; set; } = JsonRpcContent.MediaType;

        /// <summary>
        /// 获取或设置JsonRpc的路径
        /// 可以为空、相对路径或绝对路径
        /// </summary>
        public string? Path
        {
            get => this.postAttribute.Path;
            set => this.postAttribute = new HttpPostAttribute(value);
        }

        /// <summary>
        /// 获取顺序排序的索引
        /// </summary>
        public override int OrderIndex => this.postAttribute.OrderIndex;

        /// <summary>
        /// Json-Rpc请求方法
        /// </summary>
        public JsonRpcMethodAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// Json-Rpc请求方法
        /// </summary>
        /// <param name="method">JsonRpc的方法名称</param>
        public JsonRpcMethodAttribute(string? method)
            : base()
        {
            this.Method = method;
        }


        /// <summary>
        /// Action请求前
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <returns></returns>
        Task IApiActionAttribute.OnRequestAsync(ApiRequestContext context)
        {
            var jsonRpcContext = new JsonRpcContext
            {
                MediaType = this.ContentType,
                Method = this.Method ?? context.ApiAction.Name,
                ParamsStyle = this.ParamsStyle
            };

            context.Properties.Set(typeof(JsonRpcContext), jsonRpcContext);
            return this.postAttribute.OnRequestAsync(context);
        }

        /// <summary>
        /// Filter请求前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task IApiFilterAttribute.OnRequestAsync(ApiRequestContext context)
        {
            context.Properties.Get<JsonRpcContext>(typeof(JsonRpcContext)).SetHttpContent(context);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Filter响应后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task IApiFilterAttribute.OnResponseAsync(ApiResponseContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 不验证响应的ContentType
        /// </summary>
        /// <param name="responseContentType"></param>
        /// <returns></returns>
        protected override bool IsMatchAcceptContentType(MediaTypeHeaderValue? responseContentType)
        {
            return true;
        }
    }
}
