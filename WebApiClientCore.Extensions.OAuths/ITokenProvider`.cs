namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 定义类型别名的token提供者
    /// </summary>
    /// <typeparam name="THttpApi">接口类型</typeparam>
    public interface ITokenProvider<THttpApi> : ITokenProvider
    {
    }
}