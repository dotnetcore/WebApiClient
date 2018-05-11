using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClient.Contexts
{
    /// <summary>
    /// 表示请求Api的上下文
    /// </summary>
    public class ApiActionContext
    {
        /// <summary>
        /// 自定义数据的存储和访问容器
        /// </summary>
        private Tags tags;

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
        /// 获取关联的HttpApiConfig
        /// </summary>
        public HttpApiConfig HttpApiConfig { get; internal set; }

        /// <summary>
        /// 获取关联的ApiActionDescriptor
        /// </summary>
        public ApiActionDescriptor ApiActionDescriptor { get; internal set; }

        /// <summary>
        /// 获取关联的HttpRequestMessage
        /// </summary>
        public HttpApiRequestMessage RequestMessage { get; internal set; }

        /// <summary>
        /// 获取关联的HttpResponseMessage
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; internal set; }

        /// <summary>
        /// 获取调用Api得到的结果
        /// </summary>
        public object Result { get; internal set; }

        /// <summary>
        /// 获取调用Api产生的异常
        /// </summary>
        public Exception Exception { get; internal set; }


        /// <summary>
        /// 准备请求数据
        /// </summary>
        /// <returns></returns>
        internal async Task PrepareRequestAsync()
        {
            var apiAction = this.ApiActionDescriptor;
            foreach (var actionAttribute in apiAction.Attributes)
            {
                await actionAttribute.BeforeRequestAsync(this);
            }

            foreach (var parameter in apiAction.Parameters)
            {
                foreach (var parameterAttribute in parameter.Attributes)
                {
                    await parameterAttribute.BeforeRequestAsync(this, parameter);
                }
            }
        }

        /// <summary>
        /// 执行请求
        /// 返回是否执行成功
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> ExecRequestAsync()
        {
            try
            {
                var apiAction = this.ApiActionDescriptor;
                var client = this.HttpApiConfig.HttpClient;

                this.ResponseMessage = await client.SendAsync(this.RequestMessage);
                this.Result = await apiAction.Return.Attribute.GetTaskResult(this);
                return true;
            }
            catch (Exception ex)
            {
                this.Exception = ex;
                return false;
            }
        }

        /// <summary>
        /// 执行所有过滤器
        /// </summary>
        /// <param name="funcSelector">方法选择</param>
        /// <returns></returns>
        internal async Task ExecFiltersAsync(Func<IApiActionFilter, Func<ApiActionContext, Task>> funcSelector)
        {
            foreach (var filter in this.HttpApiConfig.GlobalFilters)
            {
                await funcSelector(filter)(this);
            }

            foreach (var filter in this.ApiActionDescriptor.Filters)
            {
                await funcSelector(filter)(this);
            }
        }
    }
}
