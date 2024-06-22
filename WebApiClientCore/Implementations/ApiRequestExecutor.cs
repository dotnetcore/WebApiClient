﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 请求上下文执行器
    /// </summary>
    static class ApiRequestExecutor
    {
        /// <summary>
        /// 创建请求委托
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static RequestDelegate Build(ApiRequestContext request)
        {
            var builder = new ApiPipeBuilder(async request =>
            {
                // TODO 这里要缓存 requestAbortedLinker
                using var requestAbortedLinker = new CancellationTokenLinker(request.HttpContext.CancellationTokens);
                return await ApiRequestSender.SendAsync(request, requestAbortedLinker.Token).ConfigureAwait(false);
            });

            builder.Use(next => async request =>
            {
                await HandleRequestAsync(request).ConfigureAwait(false);
                var response = await next(request).ConfigureAwait(false);
                await HandleResponseAsync(response).ConfigureAwait(false);

                // TODO 在这里释放 requestAbortedLinker 才正确

                return response;
            });

            // GlobalFilter
            foreach (var filter in request.HttpContext.HttpApiOptions.GlobalFilters)
            {
                builder.Use(filter.ExecuteAsync);
            }

            // FilterAttribute
            foreach (var filter in request.ActionDescriptor.FilterAttributes)
            {
                builder.Use(filter.ExecuteAsync);
            }

            return builder.Build();
        }


        /// <summary>
        /// 处理请求上下文
        /// </summary>
        /// <returns></returns>
        private static async Task HandleRequestAsync(ApiRequestContext context)
        {
            // 参数验证
            var validateProperty = context.HttpContext.HttpApiOptions.UseParameterPropertyValidate;
            foreach (var parameter in context.ActionDescriptor.Parameters)
            {
                var parameterValue = context.Arguments[parameter.Index];
                DataValidator.ValidateParameter(parameter, parameterValue, validateProperty);
            }

            // action特性请求前执行
            foreach (var attr in context.ActionDescriptor.Attributes)
            {
                await attr.OnRequestAsync(context).ConfigureAwait(false);
            }

            // 参数特性请求前执行
            foreach (var parameter in context.ActionDescriptor.Parameters)
            {
                var ctx = new ApiParameterContext(context, parameter);
                foreach (var attr in parameter.Attributes)
                {
                    await attr.OnRequestAsync(ctx).ConfigureAwait(false);
                }
            }

            // Return特性请求前执行
            foreach (var @return in context.ActionDescriptor.Return.Attributes)
            {
                await @return.OnRequestAsync(context).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 处理响应上下文
        /// </summary>
        /// <returns></returns>
        private static async Task HandleResponseAsync(ApiResponseContext context)
        {
            // Return特性请求后执行
            var returns = context.ActionDescriptor.Return.Attributes.GetEnumerator();
            while (context.ResultStatus == ResultStatus.None && returns.MoveNext())
            {
                try
                {
                    await returns.Current.OnResponseAsync(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    context.Exception = ex;
                }
            }

            // 结果验证
            if (context.ResultStatus == ResultStatus.HasResult &&
                context.ActionDescriptor.Return.DataType.IsRawType == false &&
                context.HttpContext.HttpApiOptions.UseReturnValuePropertyValidate)
            {
                try
                {
                    DataValidator.ValidateReturnValue(context.Result);
                }
                catch (Exception ex)
                {
                    context.Exception = ex;
                }
            }
        }

        /// <summary>
        /// 表示CancellationToken链接器
        /// </summary>
        private readonly struct CancellationTokenLinker : IDisposable
        {
            /// <summary>
            /// 链接产生的 tokenSource
            /// </summary>
            private readonly CancellationTokenSource? tokenSource;

            /// <summary>
            /// 获取 token
            /// </summary>
            public CancellationToken Token { get; }

            /// <summary>
            /// CancellationToken链接器
            /// </summary>
            /// <param name="tokenList"></param>
            public CancellationTokenLinker(IList<CancellationToken> tokenList)
            {
                if (IsNoneCancellationToken(tokenList))
                {
                    this.tokenSource = null;
                    this.Token = CancellationToken.None;
                }
                else
                {
                    this.tokenSource = CancellationTokenSource.CreateLinkedTokenSource(tokenList.ToArray());
                    this.Token = this.tokenSource.Token;
                }
            }

            /// <summary>
            /// 是否为None的CancellationToken
            /// </summary>
            /// <param name="tokenList"></param>
            /// <returns></returns>
            private static bool IsNoneCancellationToken(IList<CancellationToken> tokenList)
            {
                var count = tokenList.Count;
                return (count == 0) || (count == 1 && tokenList[0] == CancellationToken.None);
            }

            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                this.tokenSource?.Dispose();
            }
        }
    }
}
