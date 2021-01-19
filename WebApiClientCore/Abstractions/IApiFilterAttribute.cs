namespace WebApiClientCore.Abstractions
{
    /// <summary>
    /// 定义Api过滤器修饰特性的行为
    /// </summary>
    public interface IApiFilterAttribute : IApiFilter, IAttributeMultiplable, IAttributeEnable
    {
    }
}