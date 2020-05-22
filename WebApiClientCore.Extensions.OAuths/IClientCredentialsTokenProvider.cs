namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 定义Client身份信息token提供者的接口
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public interface IClientCredentialsTokenProvider<THttpApi> : ITokenProvider
    {
    }
}
