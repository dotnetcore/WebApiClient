namespace WebApiClientCore
{
    /// <summary>
    /// 表示Api请求的上下文
    /// </summary>
    public class ApiRequestContext
    {
        /// <summary>
        /// 获取http上下文
        /// </summary>
        public HttpContext HttpContext { get; }

        /// <summary>
        /// 获取关联的ApiAction描述
        /// </summary>
        public ApiActionDescriptor ApiAction { get; }

        /// <summary>
        /// 获取请求参数值
        /// </summary>
        public object?[] Arguments { get; }

        /// <summary>
        /// 获取自定义数据的存储和访问容器
        /// </summary>
        public DataCollection Properties { get; }

        /// <summary>
        /// 请求Api的上下文
        /// </summary> 
        /// <param name="httpContext"></param> 
        /// <param name="apiAction"></param>
        /// <param name="arguments"></param>
        public ApiRequestContext(HttpContext httpContext, ApiActionDescriptor apiAction, object?[] arguments)
            : this(httpContext, apiAction, arguments, new DataCollection())
        {
        }

        /// <summary>
        /// 请求Api的上下文
        /// </summary> 
        /// <param name="httpContext"></param> 
        /// <param name="apiAction"></param>
        /// <param name="arguments"></param>
        /// <param name="properties"></param> 
        protected ApiRequestContext(HttpContext httpContext, ApiActionDescriptor apiAction, object?[] arguments, DataCollection properties)
        {
            this.HttpContext = httpContext;
            this.ApiAction = apiAction;
            this.Arguments = arguments;
            this.Properties = properties;
        }
    }
}
