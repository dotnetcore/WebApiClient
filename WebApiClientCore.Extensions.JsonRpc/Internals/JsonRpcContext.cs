using System.Collections.Generic;
using System.Linq;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.Extensions.JsonRpc
{
    /// <summary>
    /// 表示JsonRpc上下文
    /// </summary>
    class JsonRpcContext
    {
        /// <summary>
        /// 获取或设置媒体类型
        /// </summary>
        public string? MediaType { get; set; }

        /// <summary>
        /// 获取或设置方法名
        /// </summary>
        public string Method { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置参数风格
        /// </summary>
        public JsonRpcParamsStyle ParamsStyle { get; set; } = JsonRpcParamsStyle.Array;

        /// <summary>
        /// 获取或设置参数列表
        /// </summary>

        public List<ApiParameterContext> Parameters = new List<ApiParameterContext>();

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        public void SetHttpContent(ApiRequestContext context)
        {
            var jsonRpcContent = new JsonRpcContent(this.MediaType);
            context.HttpContext.RequestMessage.Content = jsonRpcContent;

            var @params = this.ParamsStyle == JsonRpcParamsStyle.Array
                ? (object)this.Parameters.Select(item => item.ParameterValue).ToArray()
                : (object)this.Parameters.ToDictionary(item => item.Parameter.Name, item => item.ParameterValue);

            var jsonRpcRequest = new JsonRpcRequest
            {
                Method = this.Method,
                Params = @params,
            };

            var options = context.HttpContext.HttpApiOptions.JsonSerializeOptions;
            var jsonSerializer = context.HttpContext.ServiceProvider.GetJsonSerializer();
            jsonSerializer.Serialize(jsonRpcContent, jsonRpcRequest, options);
        }
    }
}
