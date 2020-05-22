namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 定义用户名密码身份信息token提供者的接口
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public interface IPasswordCredentialsTokenProvider<THttpApi> : ITokenProvider
    {
    }
}
