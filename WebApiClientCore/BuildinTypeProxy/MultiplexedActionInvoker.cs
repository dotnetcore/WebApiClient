using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示复合的ApiAction执行器
    /// </summary>
    class MultiplexedActionInvoker : IActionInvoker
    {
        /// <summary>
        /// Api描述
        /// </summary>
        private readonly ApiActionDescriptor action;

        /// <summary>
        /// Api执行器
        /// </summary>
        private readonly IActionInvoker actionInvoker;

        /// <summary>
        /// ActionTask的创建委托
        /// 由于ActionTask缓存了ServiceContext，所以不能直接缓存ActionTask的实例
        /// 只能缓存ActionTask实例的创建委托
        /// </summary>
        private readonly Func<IActionInvoker, ServiceContext, object[], object> actionTaskCtor;

        /// <summary>
        /// 复合的ApiAction执行器
        /// </summary>
        /// <param name="apiAction">Api描述</param>
        public MultiplexedActionInvoker(ApiActionDescriptor apiAction)
        {
            this.action = apiAction;

            // (ApiActionDescriptor apiAction)
            var invokerType = typeof(ActionInvoker<>).MakeGenericType(apiAction.Return.DataType.Type);
            this.actionInvoker = Lambda.CreateCtorFunc<ApiActionDescriptor, IActionInvoker>(invokerType)(apiAction);

            // (IActionInvoker invoker, ServiceContext context, object[] arguments)
            var actionTaskType = typeof(ActionTask<>).MakeGenericType(apiAction.Return.DataType.Type);
            this.actionTaskCtor = Lambda.CreateCtorFunc<IActionInvoker, ServiceContext, object[], object>(actionTaskType);
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="arguments">参数值</param>
        /// <returns></returns>
        public object Invoke(ServiceContext context, object[] arguments)
        {
            if (this.action.Return.IsTaskDefinition == true)
            {
                return this.actionInvoker.Invoke(context, arguments);
            }
            else
            {
                return this.actionTaskCtor.Invoke(this.actionInvoker, context, arguments);
            }
        }
    }
}
