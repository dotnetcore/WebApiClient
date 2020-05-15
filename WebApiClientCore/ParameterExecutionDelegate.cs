using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义参数执行的委托
    /// </summary>
    /// <param name="context">上下文</param>
    /// <returns></returns>
    public delegate Task ParameterExecutionDelegate(ApiParameterContext context);
}
