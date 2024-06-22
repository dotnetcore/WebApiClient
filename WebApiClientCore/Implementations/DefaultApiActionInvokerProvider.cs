using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示ApiActionInvoker提供者
    /// </summary>
    public class DefaultApiActionInvokerProvider : IApiActionInvokerProvider
    {
        private readonly IOptionsMonitor<HttpApiOptions> httpApiOptionsMonitor;

        /// <summary>
        /// ApiActionInvoker提供者
        /// </summary>
        /// <param name="httpApiOptionsMonitor"></param>
        public DefaultApiActionInvokerProvider(IOptionsMonitor<HttpApiOptions> httpApiOptionsMonitor)
        {
            this.httpApiOptionsMonitor = httpApiOptionsMonitor;
        }

        /// <summary>
        /// 创建Action执行器
        /// </summary>
        /// <param name="actionDescriptor">Action描述</param>
        /// <returns></returns>
        public ApiActionInvoker CreateActionInvoker(ApiActionDescriptor actionDescriptor)
        {
            var actionInvoker = this.CreateDefaultActionInvoker(actionDescriptor);
            if (actionDescriptor.Return.ReturnType.IsInheritFrom<Task>() == false)
            {
                if (actionInvoker is IITaskReturnConvertable conversable)
                {
                    actionInvoker = conversable.ToITaskReturnActionInvoker();
                }
            }
            return actionInvoker;
        }

        /// <summary>
        /// 创建DefaultApiActionInvoker类型或其子类型的实例
        /// </summary>
        /// <param name="actionDescriptor">Action描述</param>
        /// <returns></returns> 
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof(DefaultApiActionInvoker<>))]
        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050", Justification = "类型已使用DynamicDependency来阻止被裁剪")]
        protected virtual ApiActionInvoker CreateDefaultActionInvoker(ApiActionDescriptor actionDescriptor)
        {
            var resultType = actionDescriptor.Return.DataType.Type;
            var invokerType = typeof(DefaultApiActionInvoker<>).MakeGenericType(resultType);
            return invokerType.CreateInstance<ApiActionInvoker>(actionDescriptor, this.httpApiOptionsMonitor);
        }
    }
}
