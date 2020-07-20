using System.Collections.Generic;
using System.Linq;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.Extensions.JsonRpc
{
    /// <summary>
    /// 表示JsonRpc参数
    /// </summary>
    class JsonRpcParameters : List<ApiParameterContext>
    {
        /// <summary>
        /// 转换为jsonRpc请求参数
        /// </summary>
        /// <param name="paramsStyle"></param>
        /// <returns></returns>
        public object ToJsonRpcParams(JsonRpcParamsStyle paramsStyle)
        {
            if (paramsStyle == JsonRpcParamsStyle.Array)
            {
                return this.Select(item => item.ParameterValue).ToArray();
            }
            else
            {
                return this.ToDictionary(item => item.Parameter.Name, item => item.ParameterValue);
            }
        } 
    }
}
