using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示执行者描述器
    /// </summary>
    class InvokerDescriptor
    {
        /// <summary>
        /// Api描述
        /// </summary>
        public ApiActionDescriptor Action { get; }

        /// <summary>
        /// Api执行器
        /// </summary>
        public IActionInvoker ActionInvoker { get; }

        /// <summary>
        /// ActionTask的创建委托
        /// 由于ActionTask缓存了ServiceContext，所以不能直接缓存ActionTask的实例
        /// 只能缓存ActionTask实例的创建委托
        /// </summary>
        public Func<ApiActionDescriptor, ServiceContext, object[], object> ActionTaskCtor { get; }

        /// <summary>
        /// 执行者描述器
        /// </summary>
        /// <param name="apiAction">Api描述</param>
        public InvokerDescriptor(ApiActionDescriptor apiAction)
        {
            this.Action = apiAction;

            // (ApiActionDescriptor apiAction)
            var invokerType = typeof(ActionInvoker<>).MakeGenericType(apiAction.Return.DataType.Type);
            this.ActionInvoker = Lambda.CreateCtorFunc<ApiActionDescriptor, IActionInvoker>(invokerType)(apiAction);

            // (ApiActionDescriptor apiAction, ServiceContext context, object[] arguments)
            var actionTaskType = typeof(ActionTask<>).MakeGenericType(apiAction.Return.DataType.Type);
            this.ActionTaskCtor = Lambda.CreateCtorFunc<ApiActionDescriptor, ServiceContext, object[], object>(actionTaskType);
        }
    }
}
