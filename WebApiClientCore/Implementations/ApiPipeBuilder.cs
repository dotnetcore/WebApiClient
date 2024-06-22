using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示应用程序创建者
    /// </summary>
    public class ApiPipeBuilder
    {
        private readonly RequestDelegate fallbackHandler;
        private readonly List<Func<RequestDelegate, RequestDelegate>> middlewares = new();

        /// <summary>
        /// 应用程序创建者
        /// </summary>
        /// <param name="fallbackHandler">回退处理者</param>
        public ApiPipeBuilder(RequestDelegate fallbackHandler)
        {
            this.fallbackHandler = fallbackHandler;
        }

        /// <summary>
        /// 创建处理应用请求的委托
        /// </summary>
        /// <returns></returns>
        public RequestDelegate Build()
        {
            var handler = fallbackHandler;
            for (var i = middlewares.Count - 1; i >= 0; i--)
            {
                handler = middlewares[i](handler);
            }
            return handler;
        }


        /// <summary>
        /// 使用中间件
        /// </summary>  
        /// <param name="middleware"></param>
        /// <returns></returns>
        public ApiPipeBuilder Use(Func<ApiRequestContext, RequestDelegate, Task<ApiResponseContext>> middleware)
        {
            return Use(next => request => middleware(request, next));
        }

        /// <summary>
        /// 使用中间件
        /// </summary>
        /// <param name="middleware"></param>
        /// <returns></returns>
        public ApiPipeBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            middlewares.Add(middleware);
            return this;
        }
    }
}