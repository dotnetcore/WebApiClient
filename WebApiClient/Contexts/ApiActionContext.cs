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
    public class ApiActionContext : Disposable
    {
        /// <summary>
        /// 获取httpApi代理类实例
        /// </summary>
        public IHttpApi HttpApi { get; }

        /// <summary>
        /// 获取关联的HttpApiConfig
        /// </summary>
        public HttpApiConfig HttpApiConfig { get; }

        /// <summary>
        /// 获取关联的ApiActionDescriptor
        /// </summary>
        public ApiActionDescriptor ApiActionDescriptor { get; }

        /// <summary>
        /// 获取关联的HttpRequestMessage
        /// </summary>
        public HttpApiRequestMessage RequestMessage { get; }



        /// <summary>
        /// 获取本次请求相关的自定义数据的存储和访问容器
        /// </summary>
        public Tags Tags { get; } = new Tags();

        /// <summary>
        /// 获取请求取消令牌集合
        /// 这些令牌将被连接起来
        /// </summary>
        public IList<CancellationToken> CancellationTokens { get; } = new List<CancellationToken>();



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
            await this.ExecApiAttributesAsync().ConfigureAwait(false);
            await this.ExecFiltersAsync(filter => filter.OnBeginRequestAsync).ConfigureAwait(false);

            try
            {
                await this.ExecRequestAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Exception = ex;
                throw;
            }
            finally
            {
                await this.ExecFiltersAsync(filter => filter.OnEndRequestAsync).ConfigureAwait(false);
            }

            return (TResult)this.Result;
        }

        /// <summary>
        /// 执行Api的所有特性的请求前行为
        /// </summary>
        /// <returns></returns>
        private async Task ExecApiAttributesAsync()
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
        /// 使用或不使用缓存
        /// </summary>
        /// <returns></returns>
        private async Task ExecRequestAsync()
        {
            var apiCache = new ApiCache(this);
            var cacheResult = await apiCache.GetAsync().ConfigureAwait(false);

            if (cacheResult.ResponseMessage != null)
            {
                this.ResponseMessage = cacheResult.ResponseMessage;
                this.Result = await this.ApiActionDescriptor.Return.Attribute.GetTaskResult(this).ConfigureAwait(false);
            }
            else
            {
                await this.ExecHttpRequestAsync().ConfigureAwait(false);
                await apiCache.SetAsync(cacheResult.CacheKey).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 执行http请求
        /// </summary>
        /// <returns></returns>
        private async Task ExecHttpRequestAsync()
        {
            using (var cancellation = this.CreateLinkedTokenSource())
            {
                var completionOption = this.ApiActionDescriptor.Return.DataType.IsHttpResponseWrapper ?
                    HttpCompletionOption.ResponseHeadersRead :
                    HttpCompletionOption.ResponseContentRead;

                this.ResponseMessage = await this.HttpApiConfig.HttpClient
                    .SendAsync(this.RequestMessage, completionOption, cancellation.Token)
                    .ConfigureAwait(false);

                this.Result = await this.ApiActionDescriptor.Return.Attribute.GetTaskResult(this).ConfigureAwait(false);
                ApiValidator.ValidateReturnValue(this.Result, this.HttpApiConfig.UseReturnValuePropertyValidate);
            }
        }

        /// <summary>
        /// 创建取消令牌源
        /// </summary>
        /// <returns></returns>
        private CancellationTokenSource CreateLinkedTokenSource()
        {
            if (this.CancellationTokens.Count == 0)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);
            }
            else
            {
                var tokens = this.CancellationTokens.ToArray();
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
        /// 在nfx下请求完成时会自动Dispose了RequestMessage相关的HttpConent，所以RequestMessage在本方法不会得到Dispose
        /// 但在corefx下，RequestMessage在本方法会得到Dispose
        /// </summary>
        /// <param name="disposing">是否也释放托管资源</param>
        protected override void Dispose(bool disposing)
        {
#if !NET45 && !NET46
            this.RequestMessage.Dispose();
#endif
        }
    }
}
