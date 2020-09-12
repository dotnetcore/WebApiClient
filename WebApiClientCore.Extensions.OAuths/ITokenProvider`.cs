namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 定义指定接口的token提供者的接口
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public interface ITokenProvider<THttpApi> : ITokenProvider
    {
    }
}