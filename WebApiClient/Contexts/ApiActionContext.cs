using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient.Contexts
{
    /// <summary>
    /// 表示请求Api的上下文
    /// </summary>
    public class ApiActionContext : IDisposable
    {
        /// <summary>
        /// 自定义数据的存储和访问容器
        /// </summary>
        private Tags tags;

        /// <summary>
        /// 请求取消令牌集合
        /// </summary>
        private IList<CancellationToken> cancellationTokens;

        /// <summary>
        /// 获取本次请求相关的自定义数据的存储和访问容器
        /// </summary>
        public Tags Tags
        {
            get
            {
                if (this.tags == null)
                {
                    this.tags = new Tags();
                }
                return this.tags;
            }
        }

        /// <summary>
        /// 获取请求取消令牌集合
        /// 这些令牌将被连接起来
        /// </summary>
        public IList<CancellationToken> CancellationTokens
        {
            get
            {
                if (this.cancellationTokens == null)
                {
                    this.cancellationTokens = new List<CancellationToken>();
                }
                return this.cancellationTokens;
            }
        }

        /// <summary>
        /// 获取httpApi代理类实例
        /// </summary>
        public IHttpApi HttpApi { get; private set; }

        /// <summary>
        /// 获取关联的HttpApiConfig
        /// </summary>
        public HttpApiConfig HttpApiConfig { get; private set; }

        /// <summary>
        /// 获取关联的ApiActionDescriptor
        /// </summary>
        public ApiActionDescriptor ApiActionDescriptor { get; private set; }

        /// <summary>
        /// 获取关联的HttpRequestMessage
        /// </summary>
        public HttpApiRequestMessage RequestMessage { get; private set; }



        /// <summary>
        /// 获取关联的HttpResponseMessage
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; protected set; }

        /// <summary>
        /// 获取调用Api得到的结果
        /// </summary>
        public object Result { get; protected set; }

        /// <summary>
        /// 获取调用Api产生的异常
        /// </summary>
        public Exception Exception { get; protected set; }


        /// <summary>
        /// 请求Api的上下文
        /// </summary>
        /// <param name="httpApi">httpApi代理类实例</param>
        /// <param name="httpApiConfig">关联的HttpApiConfig</param>
        /// <param name="apiActionDescriptor">关联的ApiActionDescriptor</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiActionContext(IHttpApi httpApi, HttpApiConfig httpApiConfig, ApiActionDescriptor apiActionDescriptor)
        {
            this.HttpApi = httpApi;
            this.HttpApiConfig = httpApiConfig ?? throw new ArgumentNullException(nameof(httpApiConfig));
            this.ApiActionDescriptor = apiActionDescriptor ?? throw new ArgumentNullException(nameof(apiActionDescriptor));
            this.RequestMessage = new HttpApiRequestMessage { RequestUri = httpApiConfig.HttpHost };
        }

        /// <summary>
        /// 从HttpApiConfig.ServiceProvider获取服务实例
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns></returns>
        public T GetService<T>()
        {
            if (this.HttpApiConfig.ServiceProvider == null)
            {
                return default(T);
            }
            return (T)this.HttpApiConfig.ServiceProvider.GetService(typeof(T));
        }

        /// <summary>
        /// 执行Api方法
        /// </summary>
        /// <returns></returns>
        public virtual async Task<TResult> ExecuteActionAsync<TResult>()
        {
            await this.PrepareRequestAsync().ConfigureAwait(false);
            await this.ExecFiltersAsync(filter => filter.OnBeginRequestAsync).ConfigureAwait(false);

            try
            {
                this.Result = await this.ExecRequestAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Exception = ex;
                throw this.Exception;
            }
            finally
            {
                await this.ExecFiltersAsync(filter => filter.OnEndRequestAsync).ConfigureAwait(false);
            }

            return (TResult)this.Result;
        }

        /// <summary>
        /// 准备请求数据
        /// </summary>
        /// <returns></returns>
        private async Task PrepareRequestAsync()
        {
            var apiAction = this.ApiActionDescriptor;
            var validateProperty = this.HttpApiConfig.UseParameterPropertyValidate;

            foreach (var parameter in apiAction.Parameters)
            {
                ApiValidator.ValidateParameter(parameter, validateProperty);
            }

            foreach (var actionAttribute in apiAction.Attributes)
            {
                await actionAttribute.BeforeRequestAsync(this).ConfigureAwait(false);
            }

            foreach (var parameter in apiAction.Parameters)
            {
                foreach (var parameterAttribute in parameter.Attributes)
                {
                    await parameterAttribute.BeforeRequestAsync(this, parameter).ConfigureAwait(false);
                }
            }

            await apiAction.Return.Attribute.BeforeRequestAsync(this).ConfigureAwait(false);
        }

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <returns></returns>
        private async Task<object> ExecRequestAsync()
        {
            using (var cancellation = this.CreateLinkedTokenSource())
            {
                var cacheAttribute = this.ApiActionDescriptor.Cache;
                var cacheProvider = this.HttpApiConfig.ResponseCacheProvider;

                var cacheKey = default(string);
                var cacheResult = ResponseCacheResult.NoValue;
                var cacheEnable = cacheAttribute != null && cacheProvider != null;

                if (cacheEnable == true)
                {
                    cacheKey = await cacheAttribute.GetCacheKeyAsync(this).ConfigureAwait(false);
                    cacheResult = await cacheProvider.GetAsync(this, cacheKey).ConfigureAwait(false);
                }

                if (cacheResult.HasValue == true)
                {
                    this.ResponseMessage = cacheResult.Value.ToResponseMessage(this.RequestMessage, cacheProvider.Name);
                }
                else
                {
                    var completionOption = this.ApiActionDescriptor.Return.DataType.IsHttpResponseWrapper ?
                        HttpCompletionOption.ResponseHeadersRead :
                        HttpCompletionOption.ResponseContentRead;

                    this.ResponseMessage = await this.HttpApiConfig.HttpClient
                        .SendAsync(this.RequestMessage, completionOption, cancellation.Token)
                        .ConfigureAwait(false);

                    if (cacheEnable == true)
                    {
                        var cacheEntry = await ResponseCacheEntry.FromResponseMessageAsync(this.ResponseMessage).ConfigureAwait(false);
                        await cacheProvider.SetAsync(this, cacheKey, cacheEntry, cacheAttribute.Expiration).ConfigureAwait(false);
                    }
                }

                var result = await this.ApiActionDescriptor.Return.Attribute
                    .GetTaskResult(this)
                    .ConfigureAwait(false);

                ApiValidator.ValidateReturnValue(result, this.HttpApiConfig.UseReturnValuePropertyValidate);
                return result;
            }
        }

        /// <summary>
        /// 创建取消令牌源
        /// </summary>
        /// <returns></returns>
        private CancellationTokenSource CreateLinkedTokenSource()
        {
            if (this.cancellationTokens == null || this.cancellationTokens.Count == 0)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);
            }
            else
            {
                var tokens = this.cancellationTokens.ToArray();
                return CancellationTokenSource.CreateLinkedTokenSource(tokens);
            }
        }

        /// <summary>
        /// 执行所有过滤器
        /// </summary>
        /// <param name="funcSelector">方法选择</param>
        /// <returns></returns>
        private async Task ExecFiltersAsync(Func<IApiActionFilter, Func<ApiActionContext, Task>> funcSelector)
        {
            foreach (var filter in this.HttpApiConfig.GlobalFilters)
            {
                await funcSelector(filter)(this).ConfigureAwait(false);
            }

            foreach (var filter in this.ApiActionDescriptor.Filters)
            {
                await funcSelector(filter)(this).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.RequestMessage.Content?.Dispose();
        }
    }
}
